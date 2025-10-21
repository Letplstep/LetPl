using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTrigger : MonoBehaviour
{
    [System.Serializable]
    public class SceneData
    {
        public string sceneName;
        public Image targetImage;
        public Image changedImage;
        public GameObject triggerBox;
    }

    [Header("렛플 버튼 누르면 나머지 씬 이동 게임 버튼 활성화")]
    public static UnityEvent OnLetpleTriggered = new UnityEvent();

    [Header("씬-이미지 매칭 데이터")]
    public SceneData[] sceneDatas;

    [Header("전환 딜레이 (초)")]
    public float delayBeforeSceneLoad = 1f;

    [Header("트리거 유지 시간 (초)")]
    public float triggerHoldTime = 3f;

    private bool hasTriggered = false;
    private SceneData activeSceneData = null;
    private Coroutine activeHoldCoroutine = null;

    private void Start()
    {
        foreach (var data in sceneDatas)
        {
            if (data.triggerBox != null)
            {
                var childTrigger = data.triggerBox.AddComponent<SceneTriggerChild>();
                childTrigger.Initialize(this, data, triggerHoldTime);

                Collider col = data.triggerBox.GetComponent<Collider>();
                if (col != null)
                    col.enabled = false;
            }

            if (data.changedImage != null)
            {
                data.targetImage.gameObject.SetActive(false);
                data.changedImage.gameObject.SetActive(false);
            }
        }
    }

    public void OnTriggerEnterScene(SceneData data)
    {
        if (hasTriggered) return;

        CancelActiveTimer();

        activeSceneData = data;

        if (data.changedImage != null)
        {
            data.changedImage.gameObject.SetActive(true);
        }

        activeHoldCoroutine = StartCoroutine(HoldTimer(data));

        Debug.Log($"[SceneTrigger] '{data.sceneName}' 진입 - {triggerHoldTime}초 타이머 시작");
    }

    public void OnTriggerExitScene(SceneData data)
    {
        if (hasTriggered) return;
        if (activeSceneData != data) return;

        CancelActiveTimer();

        if (data.changedImage != null)
        {
            data.changedImage.gameObject.SetActive(false);
        }

        if (data.targetImage != null)
        {
            data.targetImage.gameObject.SetActive(true);
        }

        Debug.Log($"[SceneTrigger] '{data.sceneName}' 이탈 - 타이머 취소");
    }

    private void CancelActiveTimer()
    {
        if (activeHoldCoroutine != null)
        {
            StopCoroutine(activeHoldCoroutine);
            activeHoldCoroutine = null;
        }

        if (activeSceneData != null)
        {
            if (activeSceneData.changedImage != null)
            {
                activeSceneData.changedImage.gameObject.SetActive(false);
            }
        }

        activeSceneData = null;
    }

    private IEnumerator HoldTimer(SceneData data)
    {
        yield return new WaitForSeconds(triggerHoldTime);

        Debug.Log($"[SceneTrigger] {triggerHoldTime}초 유지 완료 - 씬 전환");
        LoadScene(data);
    }

    public void LoadScene(SceneData data)
    {
        if (hasTriggered) return;
        hasTriggered = true;

        Debug.Log($"[SceneTrigger] '{data.sceneName}' 씬 전환 시작");

        if (data.targetImage != null && data.changedImage != null)
        {
            data.targetImage.gameObject.SetActive(true);
            data.changedImage.gameObject.SetActive(true);
        }

        StartCoroutine(LoadSceneAfterDelay(data.sceneName));
    }

    public void TriggerSceneBtnActive()
    {
        Debug.Log("[SceneTrigger] 렛플 트리거 - 모든 게임 버튼 활성화");

        foreach (var data in sceneDatas)
        {
            if (data.targetImage != null)
            {
                data.targetImage.gameObject.SetActive(true);
            }

            if (data.triggerBox != null)
            {
                Collider col = data.triggerBox.GetComponent<Collider>();
                if (col != null)
                    col.enabled = true;
            }
        }
    }

    private IEnumerator LoadSceneAfterDelay(string sceneName)
    {
        yield return new WaitForSeconds(delayBeforeSceneLoad);
        Debug.Log($"[SceneTrigger] 씬 전환: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }
}

public class SceneTriggerChild : MonoBehaviour
{
    private SceneTrigger parentTrigger;
    private SceneTrigger.SceneData data;
    private float holdTime;

    public void Initialize(SceneTrigger parent, SceneTrigger.SceneData sceneData, float triggerHoldTime)
    {
        parentTrigger = parent;
        data = sceneData;
        holdTime = triggerHoldTime;

        Collider col = GetComponent<Collider>();
        if (col == null)
            col = gameObject.AddComponent<BoxCollider>();

        col.isTrigger = true;
        Debug.Log($"[SceneTriggerChild] '{gameObject.name}' 트리거 설정 완료");
    }

    private void OnTriggerEnter(Collider other)
    {
        parentTrigger.OnTriggerEnterScene(data);
    }

    private void OnTriggerExit(Collider other)
    {
        parentTrigger.OnTriggerExitScene(data);
    }
}