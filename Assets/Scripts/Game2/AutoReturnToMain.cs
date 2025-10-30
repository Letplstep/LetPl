using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoReturnToMain : MonoBehaviour
{
    [Header("리셋 설정")]
    [SerializeField] private float noPlayerTimeLimit = 30f;  // 아무도 없을 때 대기 시간
    [SerializeField] private string mainSceneName;

    private float noPlayerTimer = 0f;
    private int playerCount = 0;

    private void Update()
    {
        //플레이어가 없을 때만 타이머 증가
        if (playerCount <= 0)
        {
            noPlayerTimer += Time.deltaTime;

            if (noPlayerTimer >= noPlayerTimeLimit)
            {
                Debug.Log("플레이어 감지 없음 → 메인씬으로 복귀");
                SceneManager.LoadScene(mainSceneName);
            }
        }
        else
        {
            //누군가 감지되면 타이머 초기화
            noPlayerTimer = 0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerCount++;
            Debug.Log($"플레이어 감지됨 (+1) 현재: {playerCount}");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerCount--;
            if (playerCount < 0) playerCount = 0;
            Debug.Log($"플레이어 나감 (-1) 현재: {playerCount}");
        }
    }

    //개발용 디버그 시각화
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.2f);
        Gizmos.DrawCube(transform.position, transform.localScale);
    }
}