using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneImageTrigger : MonoBehaviour
{
    [System.Serializable]
    public class SceneImagePair
    {
        public string sceneName;       // 전환할 씬 이름
        public Image targetImage;      // 바꿀 이미지
        public Image changedImage;     // 교체할 Image
    }

    [Header("씬-이미지 매칭 설정")]
    public SceneImagePair[] sceneImagePairs;

    [Header("전환 지연 시간 (초)")]
    public float delayBeforeSceneChange = 2f;

    private bool isTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isTriggered) return;
        isTriggered = true;

        // 콜라이더가 닿는 순간 이미지 변경
        foreach (var pair in sceneImagePairs)
        {
            if (pair.targetImage != null && pair.changedImage != null)
            {
                pair.targetImage.sprite = pair.changedImage.sprite;
                pair.targetImage.color = pair.changedImage.color;
            }
        }

        // 지연 시간 후 씬 전환
        StartCoroutine(LoadMatchedSceneAfterDelay());
    }

    private IEnumerator LoadMatchedSceneAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeSceneChange);

        if (sceneImagePairs.Length > 0 && !string.IsNullOrEmpty(sceneImagePairs[0].sceneName))
        {
            SceneManager.LoadScene(sceneImagePairs[0].sceneName);
        }
    }
}
