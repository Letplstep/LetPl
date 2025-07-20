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

    [Header("���� ����")]
    public int score = 0;
    public bool isGameStarted = false;
    public bool isGameCleared = false;

    [Header("����")]
    public float prepareTime = 3f; // Ÿ�Ӷ��ΰ� �������.

    [Header("Ÿ�Ӷ���")]
    public PlayableDirector timeline;
    private bool ended = false;

    void Awake()
    {
        // �̱��� �ν��Ͻ� ����
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // �ߺ� �ν��Ͻ� ����
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

    // Ÿ�Ӷ��� ����
    private void Update()
    {
        if (!ended && timeline != null && timeline.state != PlayState.Playing)
        {
            ended = true;
            GameClear();
        }
    }

    // �غ�ð� �ڷ�ƾ
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

    // ���� �߰�
    public void AddScore(int amount)
    {
        if (!isGameStarted || isGameCleared) return;

        score += amount;
        scoreText.text = $"Score: {score}";
    }

    // ���� ����� �޼ҵ�. ���� Ÿ�Ӷ��� ������ �ڵ� ����. ���� ����� ��쿣 ���?
    public void GameClear()
    {
        if (isGameCleared) return;

        isGameCleared = true;
        gameStatusText.text = "Game Clear!";
    }
} // end class
