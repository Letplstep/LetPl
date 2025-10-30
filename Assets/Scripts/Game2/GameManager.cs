using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // 피버타임 관련
    public GameObject feverEffectPrefab; // 피버 이펙트 프리팹
    public GameObject feverBannerLeft;       // 피버 배너
    public GameObject feverBannerRight;       // 피버 배너
    public GameObject feverPlane;        // 피버타임 번쩍임용 Plane
    private List<GameObject> feverEffects = new List<GameObject>();
    private bool isFeverTimeStarted = false;

    // 오디오 관련
    [Header("Audio Clips")]
    public AudioClip feverStartClip;
    public AudioClip attackTileClip;
    public AudioClip defenseFailClip;
    public AudioClip bgmClip;

    private AudioSource sfxSource;   // 효과음 전용
    private AudioSource bgmSource;   // 배경음악 전용


    [SerializeField] private GameObject player1WinEffect;
    [SerializeField] private GameObject player2WinEffect;
    [SerializeField] private GameObject drawEffect;

    // UI
    public TextMeshProUGUI player1ScoreText;
    public TextMeshProUGUI player2ScoreText;
    public TextMeshProUGUI timerText;
    public Slider timerSlider;

    public GameObject resultPanel;
    public Text resultText;

    // 게임 시간 및 상태
    public float gameTime = 60f;
    private float currentTime;
    private bool isGameOver = false;

    // 공격/방어 타일 유지 시간
    public float attackTileLifetime = 5f;
    public float defenseTileLifetime = 5f;

    // 일반 모드 현재 공격 타일
    private Tile player1AttackTile;
    private Tile player2AttackTile;
    private Coroutine player1AttackCoroutine;
    private Coroutine player2AttackCoroutine;

    // 피버 공격 타일
    public List<Tile> feverAttackTilesP1 = new List<Tile>();
    public List<Tile> feverAttackTilesP2 = new List<Tile>();

    // 점수
    private int player1Score = 0;
    private int player2Score = 0;

    private Material feverMaterial;       // Plane용 Material
    private Color originalFeverColor;     // 원래 색 저장

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        currentTime = gameTime;
        if (timerSlider != null)
        {
            timerSlider.maxValue = gameTime;
            timerSlider.value = gameTime;
        }

        resultPanel.SetActive(false);
        player1Score = 0;
        player2Score = 0;
        UpdateScoreUI();

        SetNewAttackTileForPlayer(TileOwner.Player1);
        SetNewAttackTileForPlayer(TileOwner.Player2);

        // Plane 초기 세팅
        if (feverPlane != null)
        {
            feverPlane.SetActive(false);
            Renderer rend = feverPlane.GetComponent<Renderer>();
            if (rend != null)
            {
                feverMaterial = rend.material;
                originalFeverColor = feverMaterial.color;
            }
        }

        // 오디오 소스 준비
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.loop = false;

        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.clip = bgmClip;
        bgmSource.volume = 0.5f;

        // 게임 시작 시 BGM 재생
        if (bgmClip != null)
            bgmSource.Play();
}

private void Update()
    {
        if (isGameOver) return;

        currentTime -= Time.deltaTime;
        if (currentTime < 0) currentTime = 0;

        UpdateTimerText();
        if (timerSlider != null) timerSlider.value = currentTime;

        // 피버타임 시작 조건
        if (!isFeverTimeStarted && currentTime <= 5f)
        {
            StartFeverTime();
        }

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
        if (player1ScoreText != null)
            player1ScoreText.text = player1Score.ToString("00");
        if (player2ScoreText != null)
            player2ScoreText.text = player2Score.ToString("00");
    }

    private void StartFeverTime()
    {
        // 피버타임 시작 사운드
        if (feverStartClip != null)
            sfxSource.PlayOneShot(feverStartClip);

        StartCoroutine(FeverTimeSequence());
    }

    // 외부에서 호출용 함수 (예: Tile.cs에서 밟았을 때 호출)
    public void PlayAttackTileSound()
    {
        if (attackTileClip != null)
            sfxSource.PlayOneShot(attackTileClip);
    }

    public void PlayDefenseFailSound()
    {
        if (defenseFailClip != null)
            sfxSource.PlayOneShot(defenseFailClip);
    }

    // 피버타임 통합 코루틴
    IEnumerator FeverTimeSequence()
    {
        isFeverTimeStarted = true;

        // 모든 타일 초기화
        Tile[] allTiles = FindObjectsOfType<Tile>();
        foreach (Tile tile in allTiles)
        {
            tile.ClearAttackTile();
            tile.ClearDefenseTile();
            tile.SetOwner(tile.territoryOwner);
            tile.UpdateSprite();
        }

        // 게임 정지
        Time.timeScale = 0f;

        // 배너 활성화 후 Animator Clip 재생
        if (feverBannerLeft != null)
        {
            feverBannerLeft.SetActive(true);
        }
        // 배너 활성화 후 Animator Clip 재생
        if (feverBannerRight != null)
        {
            feverBannerRight.SetActive(true);
        }
        // 3초 대기 (실시간)
        yield return new WaitForSecondsRealtime(3f);

        // 배너 숨기기
        if (feverBannerLeft != null)
            feverBannerLeft.SetActive(false);

        // 배너 숨기기
        if (feverBannerRight != null)
            feverBannerRight.SetActive(false);

        // 게임 재개
        Time.timeScale = 1f;

        // 피버 Plane 번쩍임 시작
        if (feverPlane != null)
        {
            feverPlane.SetActive(true);
            StartCoroutine(FeverPlaneFlash());
        }

        // 피버 공격 타일 지정
        feverAttackTilesP1 = CreateFeverAttackTiles(TileOwner.Player1, 8);
        feverAttackTilesP2 = CreateFeverAttackTiles(TileOwner.Player2, 8);

        // 피버 효과 프리팹 생성
        SpawnFeverEffectsBetweenTiles();
    }

    // Plane 번쩍임 코루틴
    IEnumerator FeverPlaneFlash()
    {
        if (feverMaterial == null) yield break;

        float flashDuration = 5f; // 깜빡임 지속
        float timer = 0f;

        while (timer < flashDuration)
        {
            float intensity = Mathf.PingPong(Time.time * 2f, 1f); // 속도 조절
            feverMaterial.color = originalFeverColor * (0.5f + intensity * 0.5f);

            timer += Time.deltaTime;
            yield return null;
        }

        // 종료 후 Plane 비활성화 및 색 복원
        feverMaterial.color = originalFeverColor;
        feverPlane.SetActive(false);
    }

    private void SpawnFeverEffectsBetweenTiles()
    {
        foreach (var effect in feverEffects)
            if (effect != null) Destroy(effect);
        feverEffects.Clear();

        // 위치 예시
        Vector3[] player1Positions = new Vector3[]
        {
            new Vector3(-77.6f, -40, -64.5f),
            new Vector3(-77.6f, -40, -32),
            new Vector3(-77.6f, -40, 0),
            new Vector3(-77.6f, -40, 32.5f),
            new Vector3(-77.6f, -40, 64.8f),
            new Vector3(-46.8f, -40, -64.5f),
            new Vector3(-46.8f, -40, -32),
            new Vector3(-46.8f, -40, 0),
            new Vector3(-46.8f, -40, 32.5f),
            new Vector3(-46.8f, -40, 64.8f),
        };

        Vector3[] player2Positions = new Vector3[]
        {
            new Vector3(47, -40, -64.5f),
            new Vector3(47, -40, -32),
            new Vector3(47, -40, 0),
            new Vector3(47, -40, 32.5f),
            new Vector3(47, -40, 64.8f),
            new Vector3(77.5f, -40, -64.5f),
            new Vector3(77.5f, -40, -32),
            new Vector3(77.5f, -40, 0),
            new Vector3(77.5f, -40, 32.5f),
            new Vector3(77.5f, -40, 64.8f),
        };

        Quaternion fxRotation = Quaternion.Euler(90f, 0, 0);

        void SpawnEffects(Vector3[] positions)
        {
            foreach (Vector3 pos in positions)
            {
                GameObject fx = Instantiate(feverEffectPrefab, pos, fxRotation);
                feverEffects.Add(fx);
            }
        }

        SpawnEffects(player1Positions);
        SpawnEffects(player2Positions);
    }

    private List<Tile> CreateFeverAttackTiles(TileOwner owner, int count)
    {
        List<Tile> selectedTiles = new List<Tile>();
        Tile[] allTiles = FindObjectsOfType<Tile>();
        List<Tile> candidates = new List<Tile>();

        foreach (Tile tile in allTiles)
        {
            if (tile.owner == owner && tile.territoryOwner == owner &&
                !tile.isAttackTile && !tile.isDefenseTile)
                candidates.Add(tile);
        }

        for (int i = 0; i < count && candidates.Count > 0; i++)
        {
            Tile chosen = candidates[Random.Range(0, candidates.Count)];
            chosen.SetAsAttackTile(owner);
            selectedTiles.Add(chosen);
            candidates.Remove(chosen);
        }

        return selectedTiles;
    }

    public bool IsFeverTime => isFeverTimeStarted;

    public void AddScore(TileOwner owner)
    {
        if (owner == TileOwner.Player1) player1Score++;
        else if (owner == TileOwner.Player2) player2Score++;
        UpdateScoreUI();
    }

    public void RespawnFeverAttackTile(TileOwner owner, Tile previousTile)
    {
        List<Tile> currentList = owner == TileOwner.Player1 ? feverAttackTilesP1 : feverAttackTilesP2;
        previousTile.ClearAttackTile();
        currentList.Remove(previousTile);

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
        if (isFeverTimeStarted) return;

        Tile[] allTiles = FindObjectsOfType<Tile>();

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

        List<Tile> candidates = new List<Tile>();
        foreach (Tile t in allTiles)
        {
            if (!t.isAttackTile && !t.isDefenseTile &&
                t.owner == playerOwner && t.territoryOwner == playerOwner &&
                t != excludeTile)
                candidates.Add(t);
        }

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
        float elapsed = 0f;
        while (elapsed < attackTileLifetime)
        {
            if (!tile.isAttackTile) yield break;
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (tile != null && tile.isAttackTile)
        {
            tile.ClearAttackTile();
            if (!isFeverTimeStarted)
                SetNewAttackTileForPlayer(owner, tile);
        }
    }

    private void EndGame()
    {
        isGameOver = true;

        if (resultPanel != null)
            resultPanel.SetActive(true);

        if (resultText != null)
            resultText.gameObject.SetActive(false);

        // 모든 이펙트 초기화
        if (player1WinEffect != null) player1WinEffect.SetActive(false);
        if (player2WinEffect != null) player2WinEffect.SetActive(false);
        if (drawEffect != null) drawEffect.SetActive(false);

        // 승패/무승부 처리
        GameObject effectToShow = null;
        if (player1Score > player2Score) effectToShow = player1WinEffect;
        else if (player2Score > player1Score) effectToShow = player2WinEffect;
        else effectToShow = drawEffect;

        if (effectToShow != null)
        {
            effectToShow.SetActive(true);

            // CanvasGroup 추가 (Alpha 조절용)
            CanvasGroup cg = effectToShow.GetComponent<CanvasGroup>();
            if (cg == null) cg = effectToShow.AddComponent<CanvasGroup>();
            cg.alpha = 0f;

            // Scale 0으로 초기화
            effectToShow.transform.localScale = Vector3.zero;

            // 코루틴 시작
            StartCoroutine(ScaleAndFade(effectToShow.transform, cg, 0.5f));
        }

        Time.timeScale = 0f;

        // 5초 뒤에 게임씬으로 복귀
        StartCoroutine(ReturnToGameSceneAfterDelay(5f));
    }

    // 5초 후 게임씬으로 자동 복귀
    IEnumerator ReturnToGameSceneAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        Time.timeScale = 1f; // 시간 복원
        SceneManager.LoadScene("Pick"); 
    }
    private IEnumerator ScaleAndFade(Transform target, CanvasGroup cg, float duration)
    {
        float timer = 0f;
        Vector3 startScale = Vector3.zero;
        Vector3 endScale = Vector3.one * 55f; 

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime; // Time.timeScale = 0이므로 unscaled 사용
            float t = timer / duration;
            t = Mathf.Sin(t * Mathf.PI * 0.5f); // EaseOut 느낌
            target.localScale = Vector3.Lerp(startScale, endScale, t);
            cg.alpha = t;
            yield return null;
        }

        target.localScale = endScale;
        cg.alpha = 1f;
    }
}