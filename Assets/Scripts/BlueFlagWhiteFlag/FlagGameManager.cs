using System.Collections;
using System.Collections.Generic;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using static UnityEditor.ShaderData;

public class FlagGameManager : MonoBehaviour
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

    private float gameEndTime;   // 게임 종료 시각

    void Awake()
    {
        // 싱글톤 인스턴스 설정
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // 중복 인스턴스 제거
        }
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
    }

    private void Start()
    {
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

        gameEndTime = Time.time + totalPlayTime;
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

            int pickIndex;
            do
            {
                pickIndex = Random.Range(0, patternList.Count);
            } while (pickIndex == lastPatternIndex && patternList.Count > 1);

            FlagPatternData pattern = patternList[pickIndex];
            lastPatternIndex = pickIndex;
            currentPattern = pattern;


            // 새 패턴 시작: 상태 초기화
            patternCleared = false;
            for (int i = 0; i < 4; i++) currentPersons[i] = 0;

            // UI 갱신
            if (patternImage != null)
            {
                patternImage.sprite = pattern.sprite;
                patternImage.enabled = (pattern.sprite != null);
            }

            // 새 패턴 시작할때마다 현재 패드위에 올라간 사람 강제 반영
            for (int i = 0; i < pads.Count; i++)
                pads[i]?.ForceNotify();

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

            // 카운트 다운 이미지 보여주기
            patternImage.sprite = sprite3;
            yield return new WaitForSeconds(1f);

            patternImage.sprite = sprite2;
            yield return new WaitForSeconds(1f);

            patternImage.sprite = sprite1;
            yield return new WaitForSeconds(1f);


        }

        Debug.Log("[Game] 종료");

        EndGame();
    }

    // --- 게임 종료 처리: 패턴 숨기고 텍스트로 "게임 종료!" ---
    private void EndGame()
    {
        isGameRunning = false;
        isGameCleared = true;

        SetPatternVisible(false);
        ShowStatus("게임 종료!", true);

        Debug.Log("[Game] 종료");
    }


    // FlagPad가 호출: 해당 발판의 현재 인원 갱신
    public void OnPadPeopleChanged(FlagType type, int persons)
    {
        //if (patternCleared) return;
        //if (currentPattern == null) return;

        Debug.Log($"[PadEvent] {type} 현재 {persons}명 | 패턴: {currentPattern.name}");

        // 1) 현재 인원 업데이트
        currentPersons[(int)type] = Mathf.Max(0, persons);

        // 요구된 발판들만 확인(불필요 발판은 신경 안 씀)
        foreach (var req in currentPattern.requirements)
        {
            int cur = currentPersons[(int)req.flag];
            Debug.Log($"현재 발판 : {req.flag}의 인원 {cur}");
            if (cur < req.persons) return; // 아직 미달 → 대기
        }

        // 이미 성공 처리된 패턴이면 무시
        if (patternCleared) return;


        // 여기까지 오면 모든 조건 만족
        Debug.Log("[Pattern OK] 성공!");

        // TODO: 성공 처리(점수/이펙트/다음 패턴 등)
        patternCleared = true;
        correctCount += 1;
        Debug.Log($"[Pattern OK] 성공! 누적 점수: {correctCount}");

        AudioManager.Instance.PlayHitSFX();

    }
} // end class
