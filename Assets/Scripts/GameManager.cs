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

    // 게임 시간 관련 변수
    public float gameTime = 60f;
    private float currentTime;
    private bool isGameOver = false;

    // 공격/방어 타일 유지 시간
    public float attackTileLifetime = 5f;
    public float defenseTileLifetime = 5f;

    // 일반 모드에서 사용되는 현재 공격 타일
    private Tile player1AttackTile;
    private Tile player2AttackTile;

    private Coroutine player1AttackCoroutine;
    private Coroutine player2AttackCoroutine;

    // 피버타임 여부 및 피버 공격 타일 리스트
    private bool isFeverTimeStarted = false;
    public List<Tile> feverAttackTilesP1 = new List<Tile>();
    public List<Tile> feverAttackTilesP2 = new List<Tile>();

    // 점수
    private int player1Score = 0;
    private int player2Score = 0;

    private void Awake()
    {
        Instance = this; // 싱글톤
    }

    private void Start()
    {
        // 초기화
        currentTime = gameTime;
        resultPanel.SetActive(false);
        player1Score = 0;
        player2Score = 0;
        UpdateScoreUI();

        // 각 플레이어 공격 타일 지정
        SetNewAttackTileForPlayer(TileOwner.Player1);
        SetNewAttackTileForPlayer(TileOwner.Player2);
    }

    private void Update()
    {
        if (isGameOver) return;

        // 시간 감소
        currentTime -= Time.deltaTime;
        if (currentTime < 0) currentTime = 0;

        UpdateTimerText();

        // 피버타임 시작 조건 남은 시간이 5초 이하일 때
        if (!isFeverTimeStarted && currentTime <= 5f)
        {
            isFeverTimeStarted = true;
            StartFeverTime();
        }

        // 게임 종료
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
        // 점수 텍스트 갱신
        if (player1ScoreText != null)
            player1ScoreText.text = $"P1 점수: {player1Score}";
        if (player2ScoreText != null)
            player2ScoreText.text = $"P2 점수: {player2Score}";
    }

    private void StartFeverTime()
    {
        // 모든 타일 초기화
        Tile[] allTiles = FindObjectsOfType<Tile>();
        foreach (Tile tile in allTiles)
        {
            tile.ClearAttackTile();
            tile.ClearDefenseTile();
            tile.SetOwner(tile.territoryOwner); // 영역 색상 초기화
        }

        // 각 플레이어의 영역 중 8개씩을 피버 공격 타일로 지정
        feverAttackTilesP1 = CreateFeverAttackTiles(TileOwner.Player1, 8);
        feverAttackTilesP2 = CreateFeverAttackTiles(TileOwner.Player2, 8);
    }

    private List<Tile> CreateFeverAttackTiles(TileOwner owner, int count)
    {
        List<Tile> selectedTiles = new List<Tile>();
        Tile[] allTiles = FindObjectsOfType<Tile>();
        List<Tile> candidates = new List<Tile>();

        // 조건에 맞는 후보 타일 거름
        foreach (Tile tile in allTiles)
        {
            if (tile.owner == owner && tile.territoryOwner == owner &&
                !tile.isAttackTile && !tile.isDefenseTile)
                candidates.Add(tile);
        }

        // 무작위로 count개 타일 선택
        for (int i = 0; i < count && candidates.Count > 0; i++)
        {
            Tile chosen = candidates[Random.Range(0, candidates.Count)];
            chosen.SetAsAttackTile(owner);
            selectedTiles.Add(chosen);
            candidates.Remove(chosen);
        }

        return selectedTiles;
    }

    // 피버타임 여부 외부에서 접근 가능
    public bool IsFeverTime => isFeverTimeStarted;

    public void AddScore(TileOwner owner)
    {
        // 점수 추가
        if (owner == TileOwner.Player1) player1Score++;
        else if (owner == TileOwner.Player2) player2Score++;

        UpdateScoreUI();
    }

    public void RespawnFeverAttackTile(TileOwner owner, Tile previousTile)
    {
        // 피버 타일 제거 및 리스트에서 제거
        List<Tile> currentList = owner == TileOwner.Player1 ? feverAttackTilesP1 : feverAttackTilesP2;
        previousTile.ClearAttackTile();
        currentList.Remove(previousTile);

        // 후보 타일 중 현재 리스트에 없는 타일만 대상
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
        // 피버타임이면 일반 공격 타일 설정 안 함
        if (isFeverTimeStarted) return;

        Tile[] allTiles = FindObjectsOfType<Tile>();

        // 기존 타일 클리어
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

        // 후보 타일 선정
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

        // 무작위 타일 지정
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
        // 일정 시간 후 자동으로 공격 타일 재설정
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

            // 피버타임이 아닐 경우에만 다음 공격 타일 지정
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

            // 방어 실패 시 공격자에게 점수
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

            // 승패 판정
            if (player1Score > player2Score)
                resultText.text = "Player 1 승리";
            else if (player2Score > player1Score)
                resultText.text = "Player 2 승리";
            else
                resultText.text = "비김";
        }

        Time.timeScale = 0f; // 게임 멈춤
    }
}