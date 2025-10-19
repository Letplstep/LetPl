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

    private bool hasTriggered = false; // ✅ 먼저 밟은 발판만 인식

    private void Start()
    {
        foreach (var data in sceneDatas)
        {
            if (data.triggerBox != null)
            {
                var childTrigger = data.triggerBox.AddComponent<SceneTriggerChild>();
                childTrigger.Initialize(this, data);
            }

            if (data.changedImage != null)
                data.changedImage.gameObject.SetActive(false);
        }
    }

    public void TriggerSceneChange(SceneData data)
    {
        if (hasTriggered) return; // 이미 발판 밟았으면 무시
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
        Debug.Log($"[SceneTriggerChild] '{gameObject.name}'이(가) '{other.name}'과 충돌함");
        parentTrigger.TriggerSceneChange(data);
    }
}
