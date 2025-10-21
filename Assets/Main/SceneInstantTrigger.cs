using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTrigger : MonoBehaviour
{
    [System.Serializable]
    public class SceneData
    {
        public string sceneName;       // 전환할 씬 이름
        public Image targetImage;      // 현재 켜져있는 이미지
        public Image changedImage;     // 켜질 이미지
        public GameObject triggerBox;  // 트리거 감지용 오브젝트
    }

    [Header("씬-이미지 매칭 데이터")]
    public SceneData[] sceneDatas;

    [Header("전환 딜레이 (초)")]
    public float delayBeforeSceneLoad = 2f;

    [Header("게임 시작 시 초기 화면")]
    public Image initialImage; // 시작 시 보여줄 이미지
    public float initialDelay = 3f;

    private bool hasTriggered = false; // 먼저 밟은 발판만 인식
    private bool isInitialDelay = true; // 초기 딜레이 동안 트리거 비활성

    private void Start()
    {
        // 초기 이미지 켜기
        if (initialImage != null)
            initialImage.gameObject.SetActive(true);

        // 초기 딜레이 코루틴 시작
        StartCoroutine(InitialDelayCoroutine());

        // 씬-트리거 세팅
        foreach (var data in sceneDatas)
        {
            if (data.triggerBox != null)
            {
                var childTrigger = data.triggerBox.AddComponent<SceneTriggerChild>();
                childTrigger.Initialize(this, data);
            }

            // 바뀌는 이미지 초기화
            if (data.changedImage != null)
                data.changedImage.gameObject.SetActive(false);
        }
    }

    private IEnumerator InitialDelayCoroutine()
    {
        yield return new WaitForSeconds(initialDelay);

        // 초기 이미지 끄기
        if (initialImage != null)
            initialImage.gameObject.SetActive(false);

        isInitialDelay = false;
        Debug.Log("[SceneTrigger] 초기 딜레이 종료, 트리거 활성화");
    }

    public void TriggerSceneChange(SceneData data)
    {
        if (hasTriggered || isInitialDelay) return; // 초기 딜레이 동안 무시
        hasTriggered = true;

        if (data.targetImage != null && data.changedImage != null)
        {
            Debug.Log($"[SceneTrigger] '{data.sceneName}' 트리거 감지됨 — 이미지 전환 후 씬 이동 예정");

            data.targetImage.gameObject.SetActive(false);
            data.changedImage.gameObject.SetActive(true);

            StartCoroutine(LoadSceneAfterDelay(data.sceneName));
        }
        else
        {
            Debug.LogWarning($"[SceneTrigger] '{data.sceneName}' 이미지 참조가 비어 있습니다!");
        }
    }

    private IEnumerator LoadSceneAfterDelay(string sceneName)
    {
        yield return new WaitForSeconds(delayBeforeSceneLoad);
        Debug.Log($"[SceneTrigger] 씬 전환: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }

    public bool IsInitialDelayActive()
    {
        return isInitialDelay;
    }
}

// 콜라이더 감지용 클래스
public class SceneTriggerChild : MonoBehaviour
{
    private SceneTrigger parentTrigger;
    private SceneTrigger.SceneData data;

    public void Initialize(SceneTrigger parent, SceneTrigger.SceneData sceneData)
    {
        parentTrigger = parent;
        data = sceneData;

        Collider col = GetComponent<Collider>();
        if (col == null)
            col = gameObject.AddComponent<BoxCollider>();
        col.isTrigger = true;

        Debug.Log($"[SceneTriggerChild] '{gameObject.name}' 트리거 설정 완료");
    }

    private void OnTriggerEnter(Collider other)
    {
        // 초기 딜레이 동안 충돌 무시
        if (parentTrigger.IsInitialDelayActive()) return;

        Debug.Log($"[SceneTriggerChild] '{gameObject.name}'이(가) '{other.name}'과 충돌함");
        parentTrigger.TriggerSceneChange(data);
    }
}
