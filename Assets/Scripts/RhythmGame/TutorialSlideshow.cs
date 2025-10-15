using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialSlideshow : MonoBehaviour
{
    [Header("Target UI")]
    public Image image;                 // Sprite 표시용

    [Header("Slides")]
    public List<Sprite> sprites;        // 보여줄 이미지들

    [Header("Playback")]
    public float interval = 2f;         // 슬라이드 간격(초)
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
            yield return new WaitForSeconds(interval);
        }

        // 숨기기
        image.enabled = false;
        gameObject.SetActive(false);
    }
}
