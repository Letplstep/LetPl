using UnityEngine;
using UnityEngine.UI;

public class TimerSliderWithCapAuto : MonoBehaviour
{
    public Slider slider;          // Slider 컴포넌트
    public Image fillImage;        // Fill 이미지 (Simple 타입)

    public float totalTime = 10f;  // 타이머 총 시간
    public float fadeThreshold = 0.05f; // 마지막 몇 %에서 서서히 사라짐

    private float elapsed = 0f;

    void Start()
    {
        if (slider == null || fillImage == null)
        {
            Debug.LogError("Slider와 Fill 연결 필요!");
            enabled = false;
            return;
        }

        slider.maxValue = 1f;
        slider.value = 1f; // 시작 = 풀 게이지
    }

    void Update()
    {
        if (elapsed < totalTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / totalTime;

            // Slider value 감소 (1 → 0)
            slider.value = Mathf.Lerp(1f, 0f, t);

            // 마지막 fadeThreshold 구간에서 Fill 알파 조정
            if (slider.value <= fadeThreshold)
            {
                float alpha = Mathf.InverseLerp(0f, fadeThreshold, slider.value);
                Color c = fillImage.color;
                c.a = alpha;
                fillImage.color = c;
            }
            else
            {
                Color c = fillImage.color;
                c.a = 1f;
                fillImage.color = c;
            }
        }
    }
}