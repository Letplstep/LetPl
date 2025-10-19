using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SimpleFillTimer : MonoBehaviour
{
    [Header("타이머 설정")]
    public float totalSeconds = 10f;
    public bool autoStart = false;  // 여기 false로 바꾸기! 튜토리얼 끝난 후에 시작해야 하므로

    [Header("UI 설정")]
    public Image fillImage; // Fill만 있는 이미지
    public Text failText;   // 화면 중앙에 큰 실패 문구

    [Header("인버트 옵션")]
    public bool isInverted = false;

    [Header("씬 전환")]
    [Tooltip("씬 이름을 직접 입력하세요.")]
    public string mainSceneName; // SceneAsset 대신 string 사용
    public float delayBeforeLoad = 3f;

    [Header("애니메이션 설정")]
    public float failTextAnimDuration = 1f;
    public float failTextScale = 1.5f;

    [Header("튜토리얼 슬라이드쇼")]
    public TutorialSlideshow tutorialSlideshow; // 에디터에서 할당하세요

    private float remainingTime;
    private bool isRunning = false;
    private bool hasFinished = false; // 성공/실패 중복 방지

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
            // 튜토리얼 완료 이벤트 구독
            tutorialSlideshow.OnSlideshowFinished += OnTutorialFinished;
        }
        else
        {
            if (autoStart)
                StartTimer();
        }
    }

    private void OnTutorialFinished()
    {
        Debug.Log("튜토리얼 종료 감지: 타이머 시작");
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
            failText.transform.localScale = Vector3.one; // 원래 크기로 초기화
        }

        Time.timeScale = 1f;
    }

    // ✅ 성공 처리용
    public void OnSuccess()
    {
        if (hasFinished) return;

        isRunning = false;
        hasFinished = true;
        Debug.Log("게임 성공! 메인 씬으로 이동");
        StartCoroutine(LoadMainSceneAfterDelay());
    }

    private void ShowFailMessage()
    {
        if (failText != null)
        {
            failText.text = "실패!";
            failText.gameObject.SetActive(true);
            failText.color = new Color(failText.color.r, failText.color.g, failText.color.b, 0f); // 투명으로 시작

            StartCoroutine(FailTextAnimation());
        }

        hasFinished = true;
        Debug.Log("타이머 종료! 게임 멈춤");
        Time.timeScale = 0f;

        StartCoroutine(LoadMainSceneAfterDelay());
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
            elapsed += Time.unscaledDeltaTime;
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
