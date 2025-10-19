using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialSlideshow : MonoBehaviour
{
    [Header("Target UI")]
    public Image image;                 // Sprite 표시할 UI Image

    [Header("Slides")]
    public List<Sprite> sprites;        // 보여줄 스프라이트 목록

    [Header("Playback")]
    public float interval = 2f;         // 슬라이드 간 간격(초)
    public bool hideAfter = true;       // 끝난 후 오브젝트 숨길지 여부

    // 슬라이드쇼 완료 시 호출할 이벤트(구독 가능)
    public System.Action OnSlideshowFinished;

    void Start()
    {
        // 게임 시작하자마자 슬라이드쇼 실행
        StartCoroutine(PlayCoroutine());
    }

    public IEnumerator PlayCoroutine()
    {
        if (image == null || sprites == null || sprites.Count == 0)
        {
            Debug.LogWarning("TutorialSlideshow: image 또는 sprites가 설정되지 않았습니다.");
            if (hideAfter) gameObject.SetActive(false);

            // 이벤트 호출
            OnSlideshowFinished?.Invoke();
            yield break;
        }

        image.gameObject.SetActive(true);

        for (int i = 0; i < sprites.Count; i++)
        {
            image.sprite = sprites[i];
            yield return new WaitForSeconds(interval);
        }

        // 슬라이드쇼 끝난 후 처리
        image.enabled = false;
        if (hideAfter) gameObject.SetActive(false);

        // 슬라이드쇼 완료 이벤트 호출
        OnSlideshowFinished?.Invoke();
    }
}
