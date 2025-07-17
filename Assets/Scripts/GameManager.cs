using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // UI
    public Text player1ScoreText;
    public Text player2ScoreText;
    public Text timerText;

    public GameObject resultPanel;
    public Text resultText;

    // ���� �ð� ���� ����
    public float gameTime = 60f;
    private float currentTime;
    private bool isGameOver = false;

    // ����/��� Ÿ�� ���� �ð�
    public float attackTileLifetime = 5f;
    public float defenseTileLifetime = 5f;

    // �Ϲ� ��忡�� ���Ǵ� ���� ���� Ÿ��
    private Tile player1AttackTile;
    private Tile player2AttackTile;

    private Coroutine player1AttackCoroutine;
    private Coroutine player2AttackCoroutine;

    // �ǹ�Ÿ�� ���� �� �ǹ� ���� Ÿ�� ����Ʈ
    private bool isFeverTimeStarted = false;
    public List<Tile> feverAttackTilesP1 = new List<Tile>();
    public List<Tile> feverAttackTilesP2 = new List<Tile>();

    // ����
    private int player1Score = 0;
    private int player2Score = 0;

    private void Awake()
    {
        Instance = this; // �̱���
    }

    private void Start()
    {
        // �ʱ�ȭ
        currentTime = gameTime;
        resultPanel.SetActive(false);
        player1Score = 0;
        player2Score = 0;
        UpdateScoreUI();

        // �� �÷��̾� ���� Ÿ�� ����
        SetNewAttackTileForPlayer(TileOwner.Player1);
        SetNewAttackTileForPlayer(TileOwner.Player2);
    }

    private void Update()
    {
        if (isGameOver) return;

        // �ð� ����
        currentTime -= Time.deltaTime;
        if (currentTime < 0) currentTime = 0;

        UpdateTimerText();

        // �ǹ�Ÿ�� ���� ���� ���� �ð��� 5�� ������ ��
        if (!isFeverTimeStarted && currentTime <= 5f)
        {
            isFeverTimeStarted = true;
            StartFeverTime();
        }

        // ���� ����
        if (currentTime <= 0)
        {
            EndGame();
        }
    }

    void UpdateTimerText()
    {
        int seconds = Mathf.CeilToInt(currentTime);
        timerText.text = $"Time: {seconds}s";
    }

    private void UpdateScoreUI()
    {
        // ���� �ؽ�Ʈ ����
        if (player1ScoreText != null)
            player1ScoreText.text = $"P1 ����: {player1Score}";
        if (player2ScoreText != null)
            player2ScoreText.text = $"P2 ����: {player2Score}";
    }

    private void StartFeverTime()
    {
        // ��� Ÿ�� �ʱ�ȭ
        Tile[] allTiles = FindObjectsOfType<Tile>();
        foreach (Tile tile in allTiles)
        {
            tile.ClearAttackTile();
            tile.ClearDefenseTile();
            tile.SetOwner(tile.territoryOwner); // ���� ���� �ʱ�ȭ
        }

        // �� �÷��̾��� ���� �� 8������ �ǹ� ���� Ÿ�Ϸ� ����
        feverAttackTilesP1 = CreateFeverAttackTiles(TileOwner.Player1, 8);
        feverAttackTilesP2 = CreateFeverAttackTiles(TileOwner.Player2, 8);
    }

    private List<Tile> CreateFeverAttackTiles(TileOwner owner, int count)
    {
        List<Tile> selectedTiles = new List<Tile>();
        Tile[] allTiles = FindObjectsOfType<Tile>();
        List<Tile> candidates = new List<Tile>();

        // ���ǿ� �´� �ĺ� Ÿ�� �Ÿ�
        foreach (Tile tile in allTiles)
        {
            if (tile.owner == owner && tile.territoryOwner == owner &&
                !tile.isAttackTile && !tile.isDefenseTile)
                candidates.Add(tile);
        }

        // �������� count�� Ÿ�� ����
        for (int i = 0; i < count && candidates.Count > 0; i++)
        {
            Tile chosen = candidates[Random.Range(0, candidates.Count)];
            chosen.SetAsAttackTile(owner);
            selectedTiles.Add(chosen);
            candidates.Remove(chosen);
        }

        return selectedTiles;
    }

    // �ǹ�Ÿ�� ���� �ܺο��� ���� ����
    public bool IsFeverTime => isFeverTimeStarted;

    public void AddScore(TileOwner owner)
    {
        // ���� �߰�
        if (owner == TileOwner.Player1) player1Score++;
        else if (owner == TileOwner.Player2) player2Score++;

        UpdateScoreUI();
    }

    public void RespawnFeverAttackTile(TileOwner owner, Tile previousTile)
    {
        // �ǹ� Ÿ�� ���� �� ����Ʈ���� ����
        List<Tile> currentList = owner == TileOwner.Player1 ? feverAttackTilesP1 : feverAttackTilesP2;
        previousTile.ClearAttackTile();
        currentList.Remove(previousTile);

        // �ĺ� Ÿ�� �� ���� ����Ʈ�� ���� Ÿ�ϸ� ���
        Tile[] allTiles = FindObjectsOfType<Tile>();
        List<Tile> candidates = new List<Tile>();
        foreach (Tile t in allTiles)
        {
            if (t.owner == owner && t.territoryOwner == owner &&
                !t.isAttackTile && !t.isDefenseTile &&
                t != previousTile && !currentList.Contains(t))
            {
                candidates.Add(t);
            }
        }

        if (candidates.Count > 0)
        {
            Tile newTile = candidates[Random.Range(0, candidates.Count)];
            newTile.SetAsAttackTile(owner);
            currentList.Add(newTile);
        }
    }

    public void SetNewAttackTileForPlayer(TileOwner playerOwner, Tile excludeTile = null)
    {
        // �ǹ�Ÿ���̸� �Ϲ� ���� Ÿ�� ���� �� ��
        if (isFeverTimeStarted) return;

        Tile[] allTiles = FindObjectsOfType<Tile>();

        // ���� Ÿ�� Ŭ����
        if (playerOwner == TileOwner.Player1 && player1AttackTile != null)
        {
            player1AttackTile.ClearAttackTile();
            if (player1AttackCoroutine != null) StopCoroutine(player1AttackCoroutine);
        }

        if (playerOwner == TileOwner.Player2 && player2AttackTile != null)
        {
            player2AttackTile.ClearAttackTile();
            if (player2AttackCoroutine != null) StopCoroutine(player2AttackCoroutine);
        }

        // �ĺ� Ÿ�� ����
        List<Tile> candidates = new List<Tile>();
        foreach (Tile t in allTiles)
        {
            if (!t.isAttackTile && !t.isDefenseTile &&
                t.owner == playerOwner && t.territoryOwner == playerOwner &&
                t != excludeTile)
            {
                candidates.Add(t);
            }
        }

        // ������ Ÿ�� ����
        if (candidates.Count > 0)
        {
            Tile selected = candidates[Random.Range(0, candidates.Count)];
            selected.SetAsAttackTile(playerOwner);

            if (playerOwner == TileOwner.Player1)
            {
                player1AttackTile = selected;
                player1AttackCoroutine = StartCoroutine(AttackTileTimer(selected, playerOwner));
            }
            else if (playerOwner == TileOwner.Player2)
            {
                player2AttackTile = selected;
                player2AttackCoroutine = StartCoroutine(AttackTileTimer(selected, playerOwner));
            }
        }
    }

    IEnumerator AttackTileTimer(Tile tile, TileOwner owner)
    {
        // ���� �ð� �� �ڵ����� ���� Ÿ�� �缳��
        float elapsed = 0f;
        while (elapsed < attackTileLifetime)
        {
            if (!tile.isAttackTile)
                yield break;

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (tile != null && tile.isAttackTile)
        {
            tile.ClearAttackTile();

            // �ǹ�Ÿ���� �ƴ� ��쿡�� ���� ���� Ÿ�� ����
            if (!isFeverTimeStarted)
                SetNewAttackTileForPlayer(owner, tile);
        }
    }

    public void StartDefenseTimer(Tile tile, TileOwner attacker)
    {
        StartCoroutine(DefenseTileTimer(tile, attacker));
    }

    IEnumerator DefenseTileTimer(Tile tile, TileOwner attacker)
    {
        tile.ShowDefenseTimerUI();

        float timer = defenseTileLifetime;
        while (timer > 0f)
        {
            if (!tile.isDefenseTile)
            {
                tile.HideDefenseTimerUI();
                yield break;
            }

            int secondsLeft = Mathf.CeilToInt(timer);
            tile.UpdateDefenseTimerUI(secondsLeft);

            timer -= Time.deltaTime;
            yield return null;
        }

        if (tile.isDefenseTile)
        {
            tile.ClearDefenseTile();
            tile.SetOwner(attacker);

            // ��� ���� �� �����ڿ��� ����
            if (attacker == TileOwner.Player1) player1Score++;
            else if (attacker == TileOwner.Player2) player2Score++;

            UpdateScoreUI();
            tile.HideDefenseTimerUI();
        }
    }

    private void EndGame()
    {
        isGameOver = true;

        if (resultPanel != null && resultText != null)
        {
            resultPanel.SetActive(true);

            // ���� ����
            if (player1Score > player2Score)
                resultText.text = "Player 1 �¸�";
            else if (player2Score > player1Score)
                resultText.text = "Player 2 �¸�";
            else
                resultText.text = "���";
        }

        Time.timeScale = 0f; // ���� ����
    }
}