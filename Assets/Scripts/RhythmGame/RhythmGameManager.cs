using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class RhythmGameManager : MonoBehaviour
{
    public static RhythmGameManager Instance { get; private set; }

    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI turnBackText;
    public TextMeshProUGUI startLineText;
    public TextMeshProUGUI gameStatusText;

    [Header("게임 상태")]
    public int score = 0;
    public bool isGameStarted = false;
    public bool isGameCleared = false;

    [Header("설정")]
    public float prepareTime = 3f; // 타임라인과 맞춰야함.

    [Header("타임라인")]
    public PlayableDirector timeline;
    private bool ended = false;

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

    void Start()
    {
        InitGame();
    }

    void InitGame()
    {
        score = 0;
        isGameStarted = false;
        isGameCleared = false;

        scoreText.text = $"Score: {score}";
        StartCoroutine(GameStartCountdown());
    }

    // 타임라인 종료
    private void Update()
    {
        if (!ended && timeline != null && timeline.state != PlayState.Playing)
        {
            ended = true;
            GameClear();
        }
    }

    // 준비시간 코루틴
    IEnumerator GameStartCountdown()
    {
        float timer = prepareTime;

        while (timer > 0)
        {
            gameStatusText.text = $"{Mathf.Ceil(timer)}!";
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
        if (!isGameStarted || isGameCleared) return;

        score += amount;
        scoreText.text = $"Score: {score}";
    }

    // 게임 종료시 메소드. 현재 타임라인 끝나면 자동 종료. 게임 취소의 경우엔 어떻게?
    public void GameClear()
    {
        if (isGameCleared) return;

        isGameCleared = true;
        gameStatusText.text = "Game Clear!";
    }
} // end class
