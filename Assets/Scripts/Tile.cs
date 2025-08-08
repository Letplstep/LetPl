using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum TileOwner { None, Player1, Player2 }

[RequireComponent(typeof(BoxCollider), typeof(SpriteRenderer))]
public class Tile : MonoBehaviour
{
    // Ÿ�� ���� ����
    public TileOwner owner = TileOwner.None;               // ���� Ÿ�� ������ �÷��̾�
    public TileOwner territoryOwner = TileOwner.None;      // Ÿ�� ���� �Ҽ�

    // Ÿ�� ����
    public bool isAttackTile = false;                      // ���� Ÿ�� ����
    public bool isDefenseTile = false;                     // ��� Ÿ�� ����

    // ��� Ÿ�� UI
    public GameObject defenseTimerUIPrefab;                // ��� Ÿ�� Ÿ�̸� UI ������
    private GameObject activeDefenseUI;                    // ǥ�� ���� ��� UI �ν��Ͻ�
    private TextMeshProUGUI defenseTimerText;              // UI ���� Ÿ�̸� �ؽ�Ʈ

    // ������Ʈ �� Ÿ�̸�
    private SpriteRenderer rend;                           // Ÿ�� ���� ǥ�ÿ� SpriteRenderer
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

    // �����ڸ� �����ϰ� ���� ������Ʈ
    public void SetOwner(TileOwner newOwner)
    {
        owner = newOwner;
        UpdateSprite();
    }

    // �� Ÿ���� ���� Ÿ�Ϸ� �����ϰ� �ڵ� ���� Ÿ�̸� ����
    public void SetAsAttackTile(TileOwner attacker)
    {
        isAttackTile = true;
        UpdateSprite();
        attackTimeoutCoroutine = StartCoroutine(AttackTimeout(attacker));
    }

    // ���� Ÿ�� ���� �� Ÿ�̸� ����
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

    // ��� Ÿ�Ϸ� ����
    public void SetAsDefenseTile(TileOwner defender)
    {
        isDefenseTile = true;
        UpdateSprite();
    }

    // ��� Ÿ�� ���� �� UI ����
    public void ClearDefenseTile()
    {
        isDefenseTile = false;
        HideDefenseTimerUI();
        UpdateSprite();
    }

    // ���� �ð� �� ���� Ÿ�� �ڵ� ����
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
    // ��� Ÿ�� Ÿ�̸� UI ǥ��
    public void ShowDefenseTimerUI()
    {
        Debug.Log($"{gameObject.name} - ShowDefenseTimerUI ȣ���");
        if (defenseTimerUIPrefab != null && activeDefenseUI == null)
        {
            activeDefenseUI = Instantiate(defenseTimerUIPrefab, transform);
            activeDefenseUI.transform.localPosition = new Vector3(2.25f, -0.1f, 0);
            defenseTimerText = activeDefenseUI.GetComponentInChildren<TextMeshProUGUI>();
        }
    }
    // ��� Ÿ�� UI ����
    public void UpdateDefenseTimerUI(int seconds)
    {
        if (defenseTimerText != null)
            defenseTimerText.text = seconds.ToString();
    }

    // ��� Ÿ�� UI ����
    public void HideDefenseTimerUI()
    {
        if (activeDefenseUI != null)
        {
            Destroy(activeDefenseUI);
            activeDefenseUI = null;
            defenseTimerText = null;
        }
    }

    public void UpdateSprite() // Ÿ�� ��������Ʈ ����
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
        // ���� ��� ����: XR������ PlayerController ����
        // PlayerController player = other.GetComponent<PlayerController>();
        // if (player == null) return;

        // XR ȯ�濡���� ��ġ�� ������ ���� (�÷��̾� ��ġ ����)
        TileOwner assumedOwner = other.transform.position.x < 0 ? TileOwner.Player1 : TileOwner.Player2;
        TileOwner enemy = assumedOwner == TileOwner.Player1 ? TileOwner.Player2 : TileOwner.Player1;
     

        Debug.Log($"Ÿ�� {gameObject.name}�� {other.name} ���� (���� ������: {assumedOwner})");

        // ��� Ÿ�� ó��
        if (isDefenseTile && owner == assumedOwner)
        {
            ClearDefenseTile();
            SetOwner(owner); // ��� ���� �� �� ����
            return;
        }

        // ���� ó��
        if (owner != assumedOwner && !isDefenseTile)
        {
            if (owner == territoryOwner)
                SetOwner(assumedOwner);
        }

        // ���� Ÿ�� ó��
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