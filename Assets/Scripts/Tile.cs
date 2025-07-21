using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum TileOwner { None, Player1, Player2 }

[RequireComponent(typeof(BoxCollider2D), typeof(SpriteRenderer))]
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

    private void Awake()
    {
        rend = GetComponent<SpriteRenderer>();
        UpdateColor(); // �ʱ� ���� ����
    }

    // �����ڸ� �����ϰ� ���� ������Ʈ
    public void SetOwner(TileOwner newOwner)
    {
        owner = newOwner;
        UpdateColor();
    }

    // �� Ÿ���� ���� Ÿ�Ϸ� �����ϰ� �ڵ� ���� Ÿ�̸� ����
    public void SetAsAttackTile(TileOwner attacker)
    {
        isAttackTile = true;
        UpdateColor();
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
        UpdateColor();
    }

    // ��� Ÿ�Ϸ� ����
    public void SetAsDefenseTile(TileOwner defender)
    {
        isDefenseTile = true;
        UpdateColor();
    }

    // ��� Ÿ�� ���� �� UI ����
    public void ClearDefenseTile()
    {
        isDefenseTile = false;
        HideDefenseTimerUI();
        UpdateColor();
    }

    // ���� �ð� �� ���� Ÿ�� �ڵ� ����
    private IEnumerator AttackTimeout(TileOwner attacker)
    {
        // �ǹ�Ÿ���̸� �ڷ�ƾ ���� �ߴ�
        if (GameManager.Instance.IsFeverTime)
            yield break;

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

    // Ÿ�� ���� ����
    public void UpdateColor()
    {
        if (isAttackTile) rend.color = Color.yellow;          // ���� Ÿ��
        else if (isDefenseTile) rend.color = Color.gray;      // ��� Ÿ��
        else
        {
            // �Ϲ� Ÿ�� ����
            switch (owner)
            {
                case TileOwner.Player1: rend.color = Color.blue; break;
                case TileOwner.Player2: rend.color = Color.red; break;
                default: rend.color = Color.white; break;      // ������
            }
        }
    }

    // �÷��̾ Ÿ�Ͽ� ����� �� ó��
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;

        // ��� Ÿ��, �÷��̾ Ÿ�� �����ڿ� ���� ��� ��� ����
        if (isDefenseTile && player.myOwner == owner)
        {
            ClearDefenseTile();
            SetOwner(owner); // ��� ���� �� ���� ����
            return;
        }

        // Ÿ���� �ٸ� ������, ��� Ÿ���� �ƴ� ��� ���� ����
        if (owner != player.myOwner && !isDefenseTile)
        {
            // ���� ���� �Ҽ��̸� ���� ����
            if (owner == territoryOwner)
                SetOwner(player.myOwner);
        }

        // ���� Ÿ�Ͽ� �� ��� ó��
        if (isAttackTile)
        {
            bool isFever = GameManager.Instance.IsFeverTime;

            ClearAttackTile();

            if (isFever)
            {
                // �ǹ�Ÿ�� �� ���� Ÿ�� ������ ���� + ���ο� �ǹ� ���� Ÿ�� ����
                GameManager.Instance.AddScore(player.myOwner);
                GameManager.Instance.RespawnFeverAttackTile(player.myOwner, this);
            }
            else
            {
                // �Ϲ� ��� - �� ������ ���� Ÿ���� ��� Ÿ�Ϸ� ����� Ÿ�̸� ����
                TileOwner enemy = player.myOwner == TileOwner.Player1 ? TileOwner.Player2 : TileOwner.Player1;
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
                    GameManager.Instance.StartDefenseTimer(targetTile, player.myOwner);
                }
            }

            // �Ϲ� ��忡���� ���ο� ���� Ÿ�� ����
            GameManager.Instance.SetNewAttackTileForPlayer(player.myOwner, this);
        }
    }
}