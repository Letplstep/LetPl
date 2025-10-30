using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public Image tutorialImage;           // 튜토리얼 이미지를 표시할 UI Image
    public Sprite[] tutorialSprites;      // 튜토리얼용 스프라이트 배열
    public float displayTime = 2f;        // 각 이미지 표시 시간(초)

    private int currentIndex = 0;

    private void Start()
    {
        if (tutorialSprites.Length == 0 || tutorialImage == null)
        {
            Debug.LogWarning("튜토리얼 이미지 또는 스프라이트 배열이 비어있음!");
            StartGame(); // 스프라이트 없으면 바로 게임 시작
            return;
        }

        tutorialImage.gameObject.SetActive(true);
        currentIndex = 0;
        tutorialImage.sprite = tutorialSprites[currentIndex];

        // 게임 일시정지
        Time.timeScale = 0f;

        // 자동 진행 코루틴 시작 (실시간)
        StartCoroutine(AutoNextTutorial());
    }

    IEnumerator AutoNextTutorial()
    {
        while (currentIndex < tutorialSprites.Length)
        {
            yield return new WaitForSecondsRealtime(displayTime);

            currentIndex++;
            if (currentIndex < tutorialSprites.Length)
            {
                tutorialImage.sprite = tutorialSprites[currentIndex];
            }
            else
            {
                // 마지막 이미지까지 다 보여줬으면 게임 시작
                StartGame();
            }
        }
    }

    private void StartGame()
    {
        tutorialImage.gameObject.SetActive(false);
        Time.timeScale = 1f; // 게임 재개
    }
}