using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIImageAnimator : MonoBehaviour
{
    public Image targetImage;            // �ִϸ��̼� ����� UI Image
    public List<Sprite> frames;          // Sprite ��Ʈ���� �ڸ� �����ӵ�
    public float frameRate = 10f;        // �ʴ� �� ������

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
        // �ִϸ��̼� ���� �� �ʿ��� �߰� ������ �ִٸ� ���⿡ �ۼ�
    }
}
