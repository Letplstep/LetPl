using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIImageAnimator : MonoBehaviour
{
    public Image targetImage;            // 애니메이션 출력할 UI Image
    public List<Sprite> frames;          // Sprite 시트에서 자른 프레임들
    public float frameRate = 10f;        // 초당 몇 프레임

    private void Start()
    {
        if (targetImage != null && frames.Count > 0)
        {
            StartCoroutine(PlayAnimationOnce());
        }
    }

    IEnumerator PlayAnimationOnce()
    {
        for (int i = 0; i < frames.Count; i++)
        {
            targetImage.sprite = frames[i];
            yield return new WaitForSeconds(1f / frameRate);
        }
        // 애니메이션 종료 후 필요한 추가 동작이 있다면 여기에 작성
    }
}
