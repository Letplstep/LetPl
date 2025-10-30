using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class XRGameManager : MonoBehaviour
{
    public static XRGameManager Instance;

    [Header("Scores")]
    public int greenScore = 0;
    public int pinkScore = 0;

    [Header("Bases")]
    public Transform greenBase;
    public Transform pinkBase;

    [Header("UI")]
    public TextMeshProUGUI greenScoreText;
    public TextMeshProUGUI pinkScoreText;
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI resultText;
    public Slider timerSlider; // (선택) 타이머 슬라이더가 있으면 연결

    [Header("Rounds")]
    public int maxRounds = 3;
    private int currentRound = 1;
    private bool roundActive = false;
    private float roundTime = 30f;
    private float currentTime;
    private Coroutine timerCoroutine;

    [Header("Coin Spawner")]
    public CoinSpawner coinSpawner;

    [Header("Audio")]
    public AudioClip bgmClip; 
    private AudioSource bgmSource;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.playOnAwake = false;
        bgmSource.spatialBlend = 0f; // 2D 사운드
        bgmSource.volume = 0.5f; // 필요시 조절 가능
    }

    // Start에서 한 프레임 대기 후 코인 미리 리스폰 -> 그 다음 라운드 시작
    private IEnumerator Start()
    {
        //  BGM 재생
        if (bgmClip != null)
        {
            bgmSource.clip = bgmClip;
            bgmSource.Play();
        }
        else
        {
            Debug.LogWarning("[XRGameManager] BGM 클립이 설정되지 않았습니다.");
        }

        ResetScores();
        UpdateScoreUI();

        // 한 프레임 기다려 UI 레이아웃 안정화
        yield return null;

        if (coinSpawner != null)
            yield return StartCoroutine(coinSpawner.RespawnAllCoinsCoroutine());
        else
            Debug.LogWarning("[XRGameManager] coinSpawner가 할당되지 않았습니다.");

        StartRound();
    }

    private void ResetScores()
    {
        greenScore = 0;
        pinkScore = 0;
        UpdateScoreUI();
    }

    public void AddScore(TeamColor team, int amount)
    {
        if (!roundActive) return;

        if (team == TeamColor.Green) greenScore += amount;
        else pinkScore += amount;

        UpdateScoreUI();
        Debug.Log($"[XRGameManager] 점수 변경 - Green:{greenScore} Pink:{pinkScore}");

        EndRound(false); // 점수 획득 시 라운드 종료
    }

    public void ExpandBase(TeamColor team)
    {
        float baseGrowth = 0.5f;
        if (team == TeamColor.Green && greenBase != null) greenBase.localScale += Vector3.one * baseGrowth;
        else if (team == TeamColor.Pink && pinkBase != null) pinkBase.localScale += Vector3.one * baseGrowth;
    }

    private void UpdateScoreUI()
    {
        if (greenScoreText != null) greenScoreText.text = $"Green: {greenScore}";
        if (pinkScoreText != null) pinkScoreText.text = $"Pink: {pinkScore}";
    }

    private void UpdateRoundUI()
    {
        if (roundText != null) roundText.text = $"Round {currentRound}/{maxRounds}";
    }

    private void UpdateTimerUI()
    {
        if (timerText != null) timerText.text = $"Time: {Mathf.CeilToInt(currentTime)}s";
        if (timerSlider != null) timerSlider.value = currentTime / roundTime;
    }

    // 라운드 시작: 타이머만 시작 (코인은 NextRoundRoutine 또는 Start에서 미리 리스폰)
    private void StartRound()
    {
        Debug.Log($"[XRGameManager] StartRound - Round {currentRound}");
        roundActive = true;
        currentTime = roundTime;
        UpdateRoundUI();
        // 슬라이더와 타이머 UI 강제 초기화
        if (timerSlider != null) timerSlider.value = 1f;

        UpdateTimerUI();

        if (timerCoroutine != null) StopCoroutine(timerCoroutine);
        timerCoroutine = StartCoroutine(RoundTimerRoutine());
    }

    private IEnumerator RoundTimerRoutine()
    {
        while (currentTime > 0f && roundActive)
        {
            // 매 프레임 감소
            currentTime -= Time.deltaTime;
            currentTime = Mathf.Max(currentTime, 0f); // 음수 방지

            UpdateTimerUI();

            yield return null; // 다음 프레임까지 대기
        }

        if (roundActive)
        {
            Debug.Log($"[XRGameManager] Round {currentRound} 시간 만료 (무승부)");
            EndRound(true);
        }
    }

    private void EndRound(bool isDraw)
    {
        if (!roundActive) return;

        roundActive = false;
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }

        if (isDraw)
        {
            if (roundText != null) roundText.text = $"Round {currentRound} 무승부!";
        }
        else
        {
            Debug.Log($"[XRGameManager] Round {currentRound} 종료 (득점)");
        }

        if (currentRound < maxRounds)
            StartCoroutine(NextRoundRoutine());
        else
            ShowFinalResult();
    }

    private IEnumerator NextRoundRoutine()
    {
        if (roundText != null) roundText.text += "\n5초 후 다음 라운드 시작";
        Debug.Log($"[XRGameManager] NextRoundRoutine: waiting 5s (end of Round {currentRound})");
        yield return new WaitForSeconds(5f);

        // 라운드 카운트 증가
        currentRound++;

        // 기지 크기 초기화
        if (greenBase != null) greenBase.localScale = Vector3.one;
        if (pinkBase != null) pinkBase.localScale = Vector3.one;



        // 다음 라운드 코인 미리 리스폰 (UI layout 준비 보장 위해 코루틴으로 실행하고 기다림)
        if (coinSpawner != null)
            yield return StartCoroutine(coinSpawner.RespawnAllCoinsCoroutine());

        // 그리고 타이머 시작
        StartRound();
    }

    private void ShowFinalResult()
    {
        string result;
        if (greenScore > pinkScore) result = "Green Team 승리!";
        else if (pinkScore > greenScore) result = "Pink Team 승리!";
        else result = "무승부!";

        if (resultText != null) resultText.text = $"게임 종료!\n{result}";

        Debug.Log($"[XRGameManager] {result}");
    }
}