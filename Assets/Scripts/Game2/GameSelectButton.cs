//using UnityEngine;
//using UnityEngine.SceneManagement;
//using System.Collections;

//public class GameSelectButton : MonoBehaviour
//{
//    [SerializeField] private string sceneName = "Game2"; // 이동할 씬 이름
//    [SerializeField] private float changeDelay = 0.1f;          // 충돌 후 교체까지 대기 시간
//    [SerializeField] private float afterChangeDelay = 2f;     // 교체 후 씬 전환까지 대기 시간
//    [SerializeField] private GameObject newSpriteObject;      // 하이라키에 있는 교체할 스프라이트 오브젝트

//    private bool isTriggered = false;

//    private void OnTriggerEnter(Collider other)
//    {
//        if (isTriggered) return;
//        isTriggered = true;

//        StartCoroutine(ChangeObjectAndLoadScene());
//    }

//    private IEnumerator ChangeObjectAndLoadScene()
//    {
//        yield return new WaitForSeconds(changeDelay);

//        // 기존 오브젝트 비활성화 대신 렌더러만 끄기 (코루틴 유지)
//        var renderer = GetComponent<SpriteRenderer>();
//        if (renderer != null)
//            renderer.enabled = false;

//        if (newSpriteObject != null)
//            newSpriteObject.SetActive(true);

//        yield return new WaitForSeconds(afterChangeDelay);

//        SceneManager.LoadScene(sceneName);
//    }
//}
//using UnityEngine;
//using UnityEngine.SceneManagement;
//using UnityEngine.UI;
//using System.Collections;

//public class GameSelectButton : MonoBehaviour
//{
//    [Header("씬 이동 설정")]
//    [SerializeField] private string sceneName = "Game2";
//    [SerializeField] private float spriteChangeDelay = 0.2f;  // 스프라이트 교체 속도
//    [SerializeField] private float afterChangeDelay = 1f;    // 스프라이트 교체 후 대기
//    [SerializeField] private GameObject newSpriteObject;

//    [Header("페이드 아웃 설정")]
//    [SerializeField] private Image fadeImage;        // 기존 캔버스에 추가한 검은색 이미지
//    [SerializeField] private float fadeDuration = 0.7f;

//    private bool isTriggered = false;

//    private void OnTriggerEnter(Collider other)
//    {
//        if (isTriggered) return;
//        isTriggered = true;
//        StartCoroutine(HandleButtonPress());
//    }

//    private IEnumerator HandleButtonPress()
//    {
//        // 1. 스프라이트 교체
//        yield return StartCoroutine(ChangeSpriteQuickly());

//        // 2. 잠깐 대기
//        yield return new WaitForSeconds(afterChangeDelay);

//        // 3. 페이드 아웃 후 씬 전환
//        if (fadeImage != null)
//            yield return StartCoroutine(FadeOutAndLoadScene());
//        else
//            SceneManager.LoadScene(sceneName);
//    }

//    private IEnumerator ChangeSpriteQuickly()
//    {
//        var renderer = GetComponent<SpriteRenderer>();
//        if (renderer != null)
//            renderer.enabled = false;

//        if (newSpriteObject != null)
//            newSpriteObject.SetActive(true);

//        yield return new WaitForSeconds(spriteChangeDelay);
//    }

//    private IEnumerator FadeOutAndLoadScene()
//    {
//        if (fadeImage != null)
//        {
//            fadeImage.gameObject.SetActive(true);       // 이미지 활성화
//            fadeImage.transform.SetAsLastSibling();     // Canvas에서 최상위로 이동
//            fadeImage.raycastTarget = true;             // UI 클릭 차단 (선택사항)

//            Color color = fadeImage.color;
//            color.a = 0f;
//            fadeImage.color = color;

//            float timer = 0f;

//            while (timer < fadeDuration)
//            {
//                timer += Time.deltaTime;
//                color.a = Mathf.Clamp01(timer / fadeDuration);  // 0 → 1
//                fadeImage.color = color;
//                yield return null;
//            }

//            color.a = 1f;
//            fadeImage.color = color;
//        }
//        SceneManager.LoadScene(sceneName);
//    }
//}
//using UnityEngine;
//using UnityEngine.SceneManagement;
//using UnityEngine.UI;
//using System.Collections;

//public class GameSelectButton : MonoBehaviour
//{
//    [Header("씬 이동 설정")]
//    [SerializeField] private string sceneName = "Game2";
//    [SerializeField] private float spriteChangeDelay = 0.2f;  // 스프라이트 교체 속도
//    [SerializeField] private float afterChangeDelay = 0.3f;     // 스프라이트 교체 후 대기
//    [SerializeField] private GameObject newSpriteObject;

//    [Header("페이드 아웃 설정")]
//    //[SerializeField] private Image fadeImage;                 // 기존 캔버스에 추가한 검은색 이미지
//    //[SerializeField] private float fadeDuration = 0.5f;

//    private bool isTriggered = false;

//    private void OnTriggerEnter(Collider other)
//    {
//        if (isTriggered) return;
//        isTriggered = true;
//        StartCoroutine(HandleButtonPress());
//    }

//    private IEnumerator HandleButtonPress()
//    {
//        // 스프라이트 빠르게 교체
//        var renderer = GetComponent<SpriteRenderer>();
//        if (renderer != null)
//            renderer.enabled = false;

//        if (newSpriteObject != null)
//            newSpriteObject.SetActive(true);

//        yield return new WaitForSeconds(spriteChangeDelay);

//        // 잠깐 대기
//        yield return new WaitForSeconds(afterChangeDelay);

//        SceneManager.LoadScene(sceneName);
//    }
//}
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class GameSelectButton : MonoBehaviour
{
    [Header("씬 이동 설정")]
    [SerializeField] private string sceneName = "Game2";
    [SerializeField] private float spriteChangeDelay = 0.2f;  // 스프라이트 교체 속도
    [SerializeField] private float afterChangeDelay = 0.3f;   // 스프라이트 교체 후 대기
    [SerializeField] private GameObject newSpriteObject;

    [Header("BGM 설정")]
    [SerializeField] private AudioClip bgmClip;
    [SerializeField] private float bgmVolume = 0.8f;

    private bool isTriggered = false;


    private void Start()
    {
        if (bgmClip != null)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.clip = bgmClip;
            source.loop = true;
            source.Play();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isTriggered) return;
        isTriggered = true;
        StartCoroutine(HandleButtonPress());
    }

    private IEnumerator HandleButtonPress()
    {
        // 스프라이트 빠르게 교체
        var renderer = GetComponent<SpriteRenderer>();
        if (renderer != null)
            renderer.enabled = false;

        if (newSpriteObject != null)
            newSpriteObject.SetActive(true);

        yield return new WaitForSeconds(spriteChangeDelay);

        // 잠깐 대기
        yield return new WaitForSeconds(afterChangeDelay);

        // 씬 전환
        SceneManager.LoadScene(sceneName);
    }
}