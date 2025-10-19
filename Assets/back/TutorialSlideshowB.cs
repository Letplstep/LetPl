using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialSlideshowB : MonoBehaviour
{
    [Header("Target UI")]
    public Image image;                 // 출력할 이미지

    [Header("Slides")]
    public List<Sprite> sprites;        // 슬라이드용 스프라이트 리스트

    [Header("Playback")]
    public float interval = 2f;         // 슬라이드 간 시간(초)
    public bool hideAfter = true;       // 끝난 뒤 숨길지 여부

    public IEnumerator PlayCoroutine()
    {
        if (image == null || sprites == null || sprites.Count == 0)
        {
            Debug.LogWarning("TutorialSlideshowB: image 또는 sprites가 할당되지 않았습니다.");
            if (hideAfter) gameObject.SetActive(false);
            yield break;
        }

        image.gameObject.SetActive(true);

        for (int i = 0; i < sprites.Count; i++)
        {
            image.sprite = sprites[i];
            yield return new WaitForSeconds(interval);
        }

        image.enabled = false;
        gameObject.SetActive(false);
    }
}
