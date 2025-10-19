using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SimpleFillTimer : MonoBehaviour
{
    [Header("타이머 설정")]
    public float totalSeconds = 10f;
    public bool autoStart = false;  // 튜토리얼 끝난 후 시작

    [Header("UI 설정")]
    public Image fillImage; // Fill만 있는 이미지
    public Text failText;   // 화면 중앙 실패 문구

    [Header("인버트 옵션")]
    public bool isInverted = false;

    [Header("씬 전환")]
    [Tooltip("씬 이름을 직접 입력하세요.")]
    public string mainSceneName;
    public float delayBeforeLoad = 3f;

    [Header("애니메이션 설정")]
    public float failTextAnimDuration = 1f;
    public float failTextScale = 1.5f;

    [Header("튜토리얼 슬라이드쇼")]
    public TutorialSlideshow tutorialSlideshow;

    private float remainingTime;
    private bool isRunning = false;
    private bool hasFinished = false;

    [Header("게임 클리어/실패 패널")]
    public GameClearPanel gameClearPanelPrefab;

    void Start()
    {
        if (fillImage == null)
        {
            Debug.LogError("SimpleFillTimer: fillImage가 할당되지 않았습니다!");
            return;
        }

        if (failText != null)
            failText.gameObject.SetActive(false);

        remainingTime = totalSeconds;

        if (tutorialSlideshow != null)
        {
            tutorialSlideshow.OnSlideshowFinished += OnTutorialFinished;
        }
        else if (autoStart)
        {
            StartTimer();
        }
    }

    private void OnTutorialFinished()
    {
        Debug.Log("튜토리얼 종료: 타이머 시작");
        StartTimer();
    }

    void Update()
    {
        if (!isRunning) return;

        remainingTime -= Time.deltaTime;
        if (remainingTime < 0) remainingTime = 0;

        float t = Mathf.InverseLerp(0, totalSeconds, remainingTime);
        fillImage.fillAmount = isInverted ? 1f - t : t;

        if (remainingTime <= 0 && !hasFinished)
        {
            isRunning = false;
            ShowFailMessage();
        }
    }

    public void StartTimer()
    {
        remainingTime = totalSeconds;
        isRunning = true;
        hasFinished = false;

        if (failText != null)
        {
            failText.gameObject.SetActive(false);
            failText.transform.localScale = Vector3.one;
        }

        Time.timeScale = 1f;
    }

    public void OnSuccess()
    {
        if (hasFinished) return;

        isRunning = false;
        hasFinished = true;
        Debug.Log("게임 성공! 메인 씬 이동");

        if (gameClearPanelPrefab != null)
        {
            gameClearPanelPrefab.ShowGameClearPanel(true); // 성공 패널
        }

        StartCoroutine(LoadMainSceneAfterDelay());
    }

private void ShowFailMessage()
{
    hasFinished = true;
    Debug.Log("타이머 종료: 실패");

    // 1️⃣ 실패 패널 먼저 띄우기
    if (gameClearPanelPrefab != null)
    {
        gameClearPanelPrefab.ShowGameClearPanel(false);
    }

    // 2️⃣ 실패 텍스트 애니메이션 시작
    if (failText != null)
    {
        failText.text = "실패!";
        failText.gameObject.SetActive(true);
        failText.color = new Color(failText.color.r, failText.color.g, failText.color.b, 0f);
        StartCoroutine(FailTextAnimation());
    }

    // 3️⃣ 텍스트와 패널이 보인 후에 시간을 멈추도록 코루틴에서 약간 지연
    StartCoroutine(StopTimeAfterDelay());
    StartCoroutine(LoadMainSceneAfterDelay());
}

// 시간이 멈추는 코루틴
private IEnumerator StopTimeAfterDelay()
{
    yield return new WaitForSecondsRealtime(2f); // 0.1초 정도 패널이 먼저 뜨도록 딜레이
    Time.timeScale = 0f;
}

    private IEnumerator FailTextAnimation()
    {
        float elapsed = 0f;
        Vector3 startScale = Vector3.one;
        Vector3 targetScale = Vector3.one * failTextScale;
        Color startColor = new Color(failText.color.r, failText.color.g, failText.color.b, 0f);
        Color targetColor = new Color(failText.color.r, failText.color.g, failText.color.b, 1f);

        while (elapsed < failTextAnimDuration)
        {
            elapsed += Time.unscaledDeltaTime; // unscaledTime 사용
            float t = Mathf.Clamp01(elapsed / failTextAnimDuration);

            failText.color = Color.Lerp(startColor, targetColor, t);
            failText.transform.localScale = Vector3.Lerp(startScale, targetScale, t);

            yield return null;
        }

        failText.color = targetColor;
        failText.transform.localScale = targetScale;
    }

    private IEnumerator LoadMainSceneAfterDelay()
    {
        yield return new WaitForSecondsRealtime(delayBeforeLoad);

        Time.timeScale = 1f;

        if (!string.IsNullOrEmpty(mainSceneName))
        {
            SceneManager.LoadScene(mainSceneName);
        }
        else
        {
            Debug.LogWarning("메인 씬 이름이 비어있습니다!");
        }
    }

    private void OnDestroy()
    {
        if (tutorialSlideshow != null)
        {
            tutorialSlideshow.OnSlideshowFinished -= OnTutorialFinished;
        }
    }
}
