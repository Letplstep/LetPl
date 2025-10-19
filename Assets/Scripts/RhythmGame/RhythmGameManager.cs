using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class RhythmGameManager : MonoBehaviour
{
    public static RhythmGameManager Instance { get; private set; }

    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI turnBackText;
    public TextMeshProUGUI startLineText;
    public TextMeshProUGUI gameStatusText;
    public GameClearPanel gameClearPanel;

    [Header("게임 상태")]
    public int score = 0;
    public bool isGameStarted = false;
    public bool isGameCleared = false;

    [Header("설정")]
    public float prepareTime = 3f; // 타임라인과 맞춰야함.

    [Header("타임라인")]
    public PlayableDirector timeline;
    private bool ended = false;

    [Header("튜토리얼")]
    public TutorialSlideshow tutorial;



    void Awake()
    {
        // 싱글톤 인스턴스 설정
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // 중복 인스턴스 제거
        }
    }

    IEnumerator Start()
    {
        // 튜토리얼 먼저
        if (tutorial != null)
            yield return StartCoroutine(tutorial.PlayCoroutine());

        isGameStarted = false;
        isGameCleared = false;
    }

    // 타임라인 종료
    private void Update()
    {
        if (!ended && timeline != null && timeline.state != PlayState.Playing)
        {
            ended = true;
            isGameCleared = true;
            GameClear();
        }
    }

    public void OnGameStatusStart()
    {
        StartCoroutine(GameStartCountdown());
    }

    // 준비시간 코루틴
    IEnumerator GameStartCountdown()
    {
        float timer = prepareTime;

        while (timer > 0)
        {
            gameStatusText.text = $"{Mathf.Ceil(timer)}";
            timer -= Time.deltaTime;
            yield return null;
        }

        gameStatusText.text = "Game Start!";
        yield return new WaitForSeconds(1f);
        gameStatusText.text = "";
        isGameStarted = true;
    }

    // 점수 추가
    public void AddScore(int amount)
    {
        score += amount;
        Debug.Log("Score 점수 : " + score);
        scoreText.text = $"{score}점";
    }

    // 게임 종료시 메소드. 현재 타임라인 끝나면 자동 종료. 게임 취소의 경우엔 어떻게?
    public void GameClear()
    {
        // 251019 점수 100점 넘으면 성공
        if (score >= 100)
        {
            gameClearPanel.ShowGameClearPanel(true);
        }
        else
        {
            gameClearPanel.ShowGameClearPanel(false);
        }

        // 5초 후 자동으로 메인씬으로 이동
        StartCoroutine(ReturnToMainSceneAfterDelay(5f));
    }

    private IEnumerator ReturnToMainSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("Main"); // "Main" 씬 이름과 정확히 일치해야 함
    }

    // 게임 재시작
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
} // end class
