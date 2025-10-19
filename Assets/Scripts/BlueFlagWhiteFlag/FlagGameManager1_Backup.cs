using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FlagGameManager1_Backup : MonoBehaviour
{
    public static FlagGameManager Instance { get; private set; }

    [Header("UI")]
    public Image patternImage;
    public Slider sliderTimer;
    public TextMeshProUGUI textGameStatus;
 
    [Header("게임 상태")]
    public float totalPlayTime = 45f;   // 전체 게임 시간
    public bool isGameStarted = false;
    public bool isGameCleared = false;
    private bool isGameRunning = false;

    [Header("준비 시간")]
    public float prepareTime = 3f; // 패턴 제시 후 대기 시간
    public float patternShowTime = 3f;  // 패턴 표시 시간
    public float countdownStepTime = 1f;  // 3,2,1 각각 표시 시간

    [Header("패턴 데이터 풀")]
    public List<FlagPatternData> patternList = new List<FlagPatternData>();
    private FlagPatternData currentPattern;
    private bool patternCleared = false;   // 현재 패턴이 성공 처리되었는지                 
    private int[] currentPersons = new int[4]; // 발판별 현재 인원 저장 (0=BlueUp, 1=BlueDown, 2=WhiteUp, 3=WhiteDown)    
    private int lastPatternIndex = -1; // 마지막으로 뽑은 패턴 인덱스 (없음 = -1)

    [Header("카운트다운 스프라이트 (인스펙터에 연결)")]
    public Sprite sprite3;
    public Sprite sprite2;
    public Sprite sprite1;

    [Header("맞은 개수")]
    public int correctCount = 0;

    [Header("패드 가지고 있기")]
    [SerializeField] private List<FlagPad> pads = new List<FlagPad>();

    [Header("청기백기 패턴 사운드 틀 오디오소스")]
    private AudioSource audioSource;

    [Header("튜토리얼")]
    public FlagTutorialShlideshow tutorial;

    [Header("스코어")]
    public int score = 5;   // 패턴 1회 성공 시 얻는 점수
    public int goalScore = 30;       // 클리어 기준 점수
    public int totalScore = 0;         // 현재 누적 점수(= correctCount * pointsPerPattern)
    public GameClearPanel gameClearPanel;

    void Awake()
    {
        //// 싱글톤 인스턴스 설정
        //if (Instance == null)
        //{
        //    Instance = this;
        //}
        //else
        //{
        //    Destroy(gameObject); // 중복 인스턴스 제거
        //}
    }

    private void SetPatternVisible(bool on)
    {
        if (patternImage)
        {
            patternImage.enabled = on;
        }
    }

    private void ShowStatus(string msg, bool on = true)
    {
        if (!textGameStatus) return;

        textGameStatus.text = msg;
        textGameStatus.enabled = on;
        textGameStatus.gameObject.SetActive(true);
    }

    private IEnumerator Start()
    {
        // 오디오 소스 연결
        audioSource = GetComponent<AudioSource>();

        // 튜토리얼 먼저
        yield return StartCoroutine(tutorial.PlayCoroutine());

        // 시작 시엔 패턴 이미지는 숨기고 상태 텍스트만 보이도록
        SetPatternVisible(false);

        if (sliderTimer)
        {
            sliderTimer.minValue = 0f;
            sliderTimer.maxValue = 100f;
            sliderTimer.value = 100f; // 초기값
        }

        StartCoroutine(StartSequence());
    }

    public void StartGame()
    {
        if (isGameRunning) return;

        isGameRunning = true;
        isGameStarted = true;
        isGameCleared = false;

        // 스코어 초기화
        correctCount = 0;
        totalScore = 0;

        StartCoroutine(GameLoop());
    }

    // --- 시작 시퀀스: 3,2,1, Game Start! → 게임 루프 시작 ---
    private IEnumerator StartSequence()
    {
        // 카운트다운 텍스트
        ShowStatus("3");
        yield return new WaitForSeconds(1f);

        ShowStatus("2");
        yield return new WaitForSeconds(1f);

        ShowStatus("1");
        yield return new WaitForSeconds(1f);

        ShowStatus("Game Start!");
        yield return new WaitForSeconds(1f); // 살짝 보여주기

        // 텍스트 숨기고 패턴 이미지 보이기 + 게임 시작
        ShowStatus("", false);
        SetPatternVisible(true);
        StartGame(); // 실제 게임 루프 시작
    }

    private IEnumerator GameLoop()
    {
        float endTime = Time.time + totalPlayTime;

        while (Time.time < endTime)
        {
            // 1) 랜덤 패턴 선택 & 이미지 표시
            //FlagPatternData pattern = patternList[Random.Range(0, patternList.Count)];
            //currentPattern = pattern;

            // 패턴 리스트가 비었을 수도 있으니 가드
            if (patternList == null || patternList.Count == 0)
            {
                Debug.LogError("[GameLoop] patternList가 비었습니다.");
                yield return null;
                continue; // 혹은 EndGame(); break;
            }

            // --- 1) 패턴 선택: 직전과 같은 패턴은 피함 ---
            int pickIndex;
            do
            {
                pickIndex = Random.Range(0, patternList.Count);
            } while (pickIndex == lastPatternIndex && patternList.Count > 1);

            // 선택된 패턴을 참조 및 ‘마지막 사용 패턴 인덱스’ 갱신
            FlagPatternData pattern = patternList[pickIndex];
            lastPatternIndex = pickIndex;

            // --- 3) 현재 패턴 확정 및 UI/SFX 갱신 ---
            currentPattern = pattern;

            // --- 2) 패턴 시작 전 상태 초기화 ---
            // 이번 라운드에서 이미 성공 처리했는지, 패턴이 클리어 되었는지 플래그 리셋
            //  // currentPersons 배열(각 발판 위 인원) 안전 초기화
            successHandled = false;
            patternCleared = false;
            if (currentPersons == null || currentPersons.Length != 4)
                currentPersons = new int[4];
            for (int i = 0; i < currentPersons.Length; i++) currentPersons[i] = 0;

            // 씬의 실제 발판(FlagPad)들에서 ‘현재 인원’을 읽어와 초기값으로 반영
            // (새 패턴 시작 시점의 베이스라인 세팅)
            for (int i = 0; i < pads.Count; i++)
            {
                if (pads[i] != null)
                {
                    currentPersons[(int)pads[i].flagType] = pads[i].Persons;
                }
            }

            // UI 갱신, 패턴 오디오 재생
            if (patternImage != null)
            {
                patternImage.sprite = pattern.sprite;
                patternImage.enabled = (pattern.sprite != null);
            }

            if (audioSource != null)
            {
                audioSource.PlayOneShot(pattern.patternSFX);
            }

            // 패턴 설정 직후 즉시 체크 (이미 조건 만족 시 바로 성공 처리)
            // CheckPatternSuccess();

            // 초기 패턴 시작 시 패드 상태가 반영되지 않아서 currentPersons가 엉뚱한 값으로 남
            // 새 패턴 시작할때마다 현재 패드위에 올라간 사람 강제 반영
            // 1016 테스트 => 없애니까 버그는 안나는데 패턴이 여러번 씹히는 경우가 있음. 
            //for (int i = 0; i < pads.Count; i++)
            //    pads[i]?.ForceNotify();

            //// 2) 3초간 패턴 이미지 보여주기
            //yield return new WaitForSeconds(patternShowTime);

            // --- 패턴 3초 동안 슬라이더 3→0 ---
            float timer = patternShowTime;
            while (timer > 0f)
            {
                if (patternCleared) break; // 성공 시 즉시 다음 패턴으로

                timer -= Time.deltaTime;
                if (sliderTimer)
                {
                    float ratio = Mathf.Clamp01(timer / patternShowTime); // 1 → 0
                    sliderTimer.value = ratio * sliderTimer.maxValue;      // 100 → 0
                }

                yield return null;
            }

            // --- 카운트다운 (슬라이더는 항상 100 유지) ---
            if (sliderTimer) sliderTimer.value = sliderTimer.maxValue;
        }

        //  Debug.Log("[Game] 종료");

        EndGame();

    }


    //// --- 게임 종료 처리: 패턴 숨기고 텍스트로 "게임 종료!" ---
    private void EndGame()
    {
        isGameRunning = false;
        isGameCleared = true;

        SetPatternVisible(false);
        // ShowStatus("Game Clear!", true);

        // 20251019 TODO : 화면 클리어 패널 띄우고 5초 뒤에 메인씬으로 자동이동. 
        bool cleared = (correctCount * score) >= goalScore;
        gameClearPanel.ShowGameClearPanel(cleared);

        // 5초 후 자동으로 메인씬으로 이동
        StartCoroutine(ReturnToMainSceneAfterDelay(5f));

        //  Debug.Log("[Game] 종료");
    }

    private IEnumerator ReturnToMainSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("Main"); // "Main" 씬 이름과 정확히 일치해야 함
    }

    private bool successHandled = false; // 이번 패턴에서 성공 처리 이미 했는지

    // FlagPad가 호출: 해당 발판의 현재 인원 갱신
    public void OnPadPeopleChanged(FlagType type, int persons)
    {
        if (!isGameRunning) return;
        if (currentPattern == null) return;
        if (patternCleared) return;

        // 현재 인원만 업데이트
        currentPersons[(int)type] = Mathf.Max(0, persons);

        // 요구된 발판들만 확인(불필요 발판은 신경 안 씀)
        foreach (var req in currentPattern.requirements)
        {
            int cur = currentPersons[(int)req.flag];
            //  Debug.Log($"현재 발판 : {req.flag}의 인원 {cur}");
            if (cur < req.persons) return; // 아직 미달 → 대기
        }

        // 이미 성공 처리된 패턴이면 무시
        if (successHandled) return;
        successHandled = true;        // 내가 선점!
        if (patternCleared) return;

        // 여기까지 오면 모든 조건 만족
        //   Debug.Log("[Pattern OK] 성공!");

        // 성공 처리(점수/이펙트/다음 패턴 등)
        patternCleared = true;
        correctCount += 1;

        // 점수 갱신
        totalScore = correctCount * score;

        Debug.Log($"[Pattern OK] 성공! 누적 점수: {correctCount}");

        // AudioManager.Instance.PlayHitSFX();

    }


    //// 패턴 성공 체크 로직 분리
    //private void CheckPatternSuccess()
    //{
    //    if (currentPattern == null) return;
    //    if (patternCleared) return;
    //    if (successHandled) return;
    //    if (currentPattern.requirements == null) return;

    //    // 모든 요구사항 체크
    //    foreach (var req in currentPattern.requirements)
    //    {
    //        int cur = currentPersons[(int)req.flag];
    //        if (cur < req.persons) return; // 미달이면 리턴
    //    }

    //    // 여기까지 오면 성공
    //    successHandled = true;
    //    patternCleared = true;
    //    correctCount += 1;
    //    Debug.Log($"[Pattern OK] 성공! 누적 점수: {correctCount}");
    //    AudioManager.Instance.PlayHitSFX();
    //}
} // end class
