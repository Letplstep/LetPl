using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum TileOwner { None, Player1, Player2 }

[RequireComponent(typeof(BoxCollider), typeof(SpriteRenderer))]
public class Tile : MonoBehaviour
{
    // 타일 소유 정보
    public TileOwner owner = TileOwner.None;               // 현재 타일 소유한 플레이어
    public TileOwner territoryOwner = TileOwner.None;      // 타일 원래 소속

    // 타일 상태
    public bool isAttackTile = false;                      // 공격 타일 여부
    public bool isDefenseTile = false;                     // 방어 타일 여부

    // 방어 타일 UI
    public GameObject defenseTimerUIPrefab;                // 방어 타일 타이머 UI 프리팹
    private GameObject activeDefenseUI;                    // 표시 중인 방어 UI 인스턴스
    private TextMeshProUGUI defenseTimerText;              // UI 내부 타이머 텍스트

    // 컴포넌트 및 타이머
    private SpriteRenderer rend;                           // 타일 색상 표시용 SpriteRenderer
    private Coroutine attackTimeoutCoroutine;

    public Sprite defaultSprite;
    public Sprite player1Sprite;
    public Sprite player2Sprite;
    public Sprite attackSprite;
    public Sprite defenseSprite;

    private void Awake()
    {
        rend = GetComponent<SpriteRenderer>();
        UpdateSprite(); 
    }

    // 소유자를 변경하고 색상 업데이트
    public void SetOwner(TileOwner newOwner)
    {
        owner = newOwner;
        UpdateSprite();
    }

    // 이 타일을 공격 타일로 설정하고 자동 해제 타이머 시작
    public void SetAsAttackTile(TileOwner attacker)
    {
        isAttackTile = true;
        UpdateSprite();
        attackTimeoutCoroutine = StartCoroutine(AttackTimeout(attacker));
    }

    // 공격 타일 해제 및 타이머 종료
    public void ClearAttackTile()
    {
        isAttackTile = false;
        if (attackTimeoutCoroutine != null)
        {
            StopCoroutine(attackTimeoutCoroutine);
            attackTimeoutCoroutine = null;
        }
        UpdateSprite();
    }

    // 방어 타일로 설정
    public void SetAsDefenseTile(TileOwner defender)
    {
        isDefenseTile = true;
        UpdateSprite();
    }

    // 방어 타일 해제 및 UI 제거
    public void ClearDefenseTile()
    {
        isDefenseTile = false;
        HideDefenseTimerUI();
        UpdateSprite();
    }

    // 일정 시간 후 공격 타일 자동 제거
    private IEnumerator AttackTimeout(TileOwner attacker)
    {
        yield return new WaitForSeconds(5f);
        if (isAttackTile)
        {
            ClearAttackTile();
            if (!GameManager.Instance.IsFeverTime)
            {
                GameManager.Instance.SetNewAttackTileForPlayer(attacker, this);
            }
        }
    }
    // 방어 타일 타이머 UI 표시
    public void ShowDefenseTimerUI()
    {
        Debug.Log($"{gameObject.name} - ShowDefenseTimerUI 호출됨");
        if (defenseTimerUIPrefab != null && activeDefenseUI == null)
        {
            activeDefenseUI = Instantiate(defenseTimerUIPrefab, transform);
            activeDefenseUI.transform.localPosition = new Vector3(2.25f, -0.1f, 0);
            defenseTimerText = activeDefenseUI.GetComponentInChildren<TextMeshProUGUI>();
        }
    }
    // 방어 타일 UI 갱신
    public void UpdateDefenseTimerUI(int seconds)
    {
        if (defenseTimerText != null)
            defenseTimerText.text = seconds.ToString();
    }

    // 방어 타일 UI 제거
    public void HideDefenseTimerUI()
    {
        if (activeDefenseUI != null)
        {
            Destroy(activeDefenseUI);
            activeDefenseUI = null;
            defenseTimerText = null;
        }
    }

    public void UpdateSprite() // 타일 스프라이트 변경
    {
        if (isAttackTile)
        {
            rend.sprite = attackSprite;
        }
        else if (isDefenseTile)
        {
            rend.sprite = defenseSprite;
        }
        else
        {
            switch (owner)
            {
                case TileOwner.Player1:
                    rend.sprite = player1Sprite;
                    break;
                case TileOwner.Player2:
                    rend.sprite = player2Sprite;
                    break;
                default:
                    rend.sprite = defaultSprite;
                    break;
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        // 기존 방식 제거: XR에서는 PlayerController 없음
        // PlayerController player = other.GetComponent<PlayerController>();
        // if (player == null) return;

        // XR 환경에서는 위치로 소유자 추정 (플레이어 위치 기준)
        TileOwner assumedOwner = other.transform.position.x < 0 ? TileOwner.Player1 : TileOwner.Player2;
        TileOwner enemy = assumedOwner == TileOwner.Player1 ? TileOwner.Player2 : TileOwner.Player1;
     

        Debug.Log($"타일 {gameObject.name}에 {other.name} 닿음 (추정 소유자: {assumedOwner})");

        // 방어 타일 처리
        if (isDefenseTile && owner == assumedOwner)
        {
            ClearDefenseTile();
            SetOwner(owner); // 방어 성공 시 색 유지
            return;
        }

        // 점령 처리
        if (owner != assumedOwner && !isDefenseTile)
        {
            if (owner == territoryOwner)
                SetOwner(assumedOwner);
        }

        // 공격 타일 처리
        if (isAttackTile)
        {
            bool isFever = GameManager.Instance.IsFeverTime;

            ClearAttackTile();

            if (isFever)
            {
                GameManager.Instance.AddScore(assumedOwner);
                GameManager.Instance.RespawnFeverAttackTile(assumedOwner, this);
            }
            else
            {
                Tile[] allTiles = FindObjectsOfType<Tile>();
                List<Tile> enemyTiles = new List<Tile>();

                foreach (Tile t in allTiles)
                {
                    if (t.owner == enemy && t.territoryOwner == enemy && !t.isAttackTile && !t.isDefenseTile)
                        enemyTiles.Add(t);
                }

                if (enemyTiles.Count > 0)
                {
                    Tile targetTile = enemyTiles[Random.Range(0, enemyTiles.Count)];
                    targetTile.SetAsDefenseTile(enemy);
                    GameManager.Instance.StartDefenseTimer(targetTile, assumedOwner);
                }
            }
            GameManager.Instance.SetNewAttackTileForPlayer(assumedOwner, this);
        }
    }
}