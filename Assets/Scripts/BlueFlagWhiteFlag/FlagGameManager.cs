using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FlagGameManager : MonoBehaviour
{
    public static FlagGameManager Instance { get; private set; }

    [Header("UI")]
    public Image patternImage;
    public Slider sliderTimer;
    public TextMeshProUGUI textGameStatus;

    [Header("게임 상태")]
    public float totalPlayTime = 45f;
    public bool isGameStarted = false;
    public bool isGameCleared = false;
    private bool isGameRunning = false;

    [Header("준비 시간")]
    public float prepareTime = 3f;
    public float patternShowTime = 2.5f;
    public float countdownStepTime = 1f;

    [Header("패턴 데이터 풀")]
    public List<FlagPatternData> patternList = new List<FlagPatternData>();
    private FlagPatternData currentPattern;

    private bool isWaitingForAnswer = false;
    private bool patternCleared = false;

    private int[] currentPersons = new int[4];
    private int lastPatternIndex = -1;

    [Header("카운트다운 스프라이트")]
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
    public int score = 5;
    public int goalScore = 30;
    public int totalScore = 0;
    public GameClearPanel gameClearPanel;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
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
        textGameStatus.gameObject.SetActive(true);
    }

    private IEnumerator Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (tutorial != null)
        {
            yield return StartCoroutine(tutorial.PlayCoroutine());
        }

        SetPatternVisible(false);

        if (sliderTimer)
        {
            sliderTimer.minValue = 0f;
            sliderTimer.maxValue = 100f;
            sliderTimer.value = 100f;
        }

        StartCoroutine(StartSequence());
    }

    private IEnumerator StartSequence()
    {
        ShowStatus("3");
        yield return new WaitForSeconds(1f);

        ShowStatus("2");
        yield return new WaitForSeconds(1f);

        ShowStatus("1");
        yield return new WaitForSeconds(1f);

        ShowStatus("Game Start!");
        yield return new WaitForSeconds(1f);

        ShowStatus("", false);
        SetPatternVisible(true);
        StartGame();
    }

    public void StartGame()
    {
        if (isGameRunning) return;

        isGameRunning = true;
        isGameStarted = true;
        isGameCleared = false;

        correctCount = 0;
        totalScore = 0;

        foreach (var pad in pads)
        {
            if (pad != null)
            {
                pad.ResetPad();
            }
        }

        StartCoroutine(GameLoop());
    }

    private IEnumerator GameLoop()
    {
        float endTime = Time.time + totalPlayTime;

        while (Time.time < endTime)
        {
            if (patternList == null || patternList.Count == 0)
            {
                Debug.LogError("[GameLoop] patternList가 비었습니다.");
                yield return new WaitForSeconds(1f);
                continue;
            }

            yield return StartCoroutine(StartNewPattern());

            float timer = patternShowTime;

            while (timer > 0f && !patternCleared)
            {
                timer -= Time.deltaTime;

                if (sliderTimer)
                {
                    float ratio = Mathf.Clamp01(timer / patternShowTime);
                    sliderTimer.value = ratio * sliderTimer.maxValue;
                }

                yield return null;
            }

            if (patternCleared)
            {
                Debug.Log($"[패턴 성공] 점수: {totalScore}");
            }
            else
            {
                Debug.Log($"[시간 초과] 다음 패턴으로");
            }

            yield return new WaitForSeconds(0.2f);

            if (sliderTimer) sliderTimer.value = sliderTimer.maxValue;
        }

        EndGame();
    }

    private IEnumerator StartNewPattern()
    {
        int pickIndex;
        do
        {
            pickIndex = UnityEngine.Random.Range(0, patternList.Count);
        } while (pickIndex == lastPatternIndex && patternList.Count > 1);

        FlagPatternData pattern = patternList[pickIndex];
        lastPatternIndex = pickIndex;
        currentPattern = pattern;

        patternCleared = false;
        isWaitingForAnswer = false;

        if (currentPersons == null || currentPersons.Length != 4)
            currentPersons = new int[4];

        for (int i = 0; i < currentPersons.Length; i++)
            currentPersons[i] = 0;

        if (patternImage != null)
        {
            patternImage.sprite = pattern.sprite;
            patternImage.enabled = (pattern.sprite != null);
        }

        if (audioSource != null && pattern.patternSFX != null)
        {
            audioSource.PlayOneShot(pattern.patternSFX);
        }

        Debug.Log($"[새 패턴 시작] {pattern.name}");

        yield return new WaitForSeconds(0.1f);

        foreach (var pad in pads)
        {
            if (pad != null)
            {
                pad.SyncToManager();
            }
        }

        isWaitingForAnswer = true;

        CheckPatternSuccess();  // 이 한 줄 추가!

        yield return null;
    }

    public void OnPadPeopleChanged(FlagType type, int persons)
    {
        if (!isGameRunning) return;
        if (currentPattern == null) return;
        if (!isWaitingForAnswer) return;
        if (patternCleared) return;

        currentPersons[(int)type] = Mathf.Max(0, persons);

        Debug.Log($"[발판 체크] {type}: {persons}명 (대기중: {isWaitingForAnswer}, 클리어: {patternCleared})");

        CheckPatternSuccess();
    }

    public void SyncPadState(FlagType type, int persons)
    {
        currentPersons[(int)type] = Mathf.Max(0, persons);
        Debug.Log($"[발판 동기화] {type}: {persons}명 (체크 안함)");
    }

    private void CheckPatternSuccess()
    {
        if (patternCleared) return;
        if (!isWaitingForAnswer) return;

        bool[] requiredFlags = new bool[4];
        foreach (var req in currentPattern.requirements)
        {
            requiredFlags[(int)req.flag] = true;
        }

        for (int i = 0; i < 4; i++)
        {
            if (!requiredFlags[i] && currentPersons[i] > 0)
            {
                Debug.Log($"[오답] {(FlagType)i} 발판에 올라가면 안됩니다!");
                return;
            }
        }

        foreach (var req in currentPattern.requirements)
        {
            int cur = currentPersons[(int)req.flag];

            if (cur < req.persons)
            {
                return;
            }
        }

        OnPatternSuccess();
    }

    private void OnPatternSuccess()
    {
        if (patternCleared) return;

        patternCleared = true;
        isWaitingForAnswer = false;

        correctCount++;
        totalScore = correctCount * score;

        Debug.Log($"[패턴 성공] 누적: {correctCount}회, 점수: {totalScore}");

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayHitSFX();
        }
    }

    private void EndGame()
    {
        isGameRunning = false;
        isGameCleared = true;
        isWaitingForAnswer = false;

        SetPatternVisible(false);

        bool cleared = totalScore >= goalScore;

        Debug.Log($"[게임 종료] 최종 점수: {totalScore}/{goalScore} (클리어: {cleared})");

        if (gameClearPanel != null)
        {
            gameClearPanel.ShowGameClearPanel(cleared);
        }

        StartCoroutine(ReturnToMainSceneAfterDelay(5f));
    }

    private IEnumerator ReturnToMainSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("Main");
    }
}