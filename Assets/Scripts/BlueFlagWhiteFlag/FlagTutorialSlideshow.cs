using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlagTutorialShlideshow : MonoBehaviour
{
    [Header("Target UI")]
    public Image image;                 // Sprite 표시용

    [Header("Slides")]
    public List<Sprite> sprites;        // 보여줄 이미지들

    [Header("Playback")]
    public float interval = 2f;         // 슬라이드 간격(초)
    public float step3Interval = 0.5f; // step 3 간격은 0.5f
    public bool hideAfter = true;       // 끝나면 오브젝트 숨김

   
    public IEnumerator PlayCoroutine()
    {
        if (image == null || sprites == null || sprites.Count == 0)
        {
            Debug.LogWarning("CustomTutorialSlideshow: image 또는 sprites가 비어있습니다.");
            if (hideAfter) gameObject.SetActive(false);
            yield break;
        }

        image.gameObject.SetActive(true);

        for (int i = 0; i < sprites.Count; i++)
        {
            image.sprite = sprites[i];

            // i가 2(세 번째 이미지)부터는 0.5초로
            float wait = (i >= 2) ? step3Interval : interval;
            yield return new WaitForSeconds(wait);
        }

        // 숨기기
        image.enabled = false;
        gameObject.SetActive(false);
    }
}
