using UnityEngine;

public enum PlayerID
{
    Player1,
    Player2
}

[RequireComponent(typeof(Rigidbody2D))]
public class RhythmPlayer : MonoBehaviour
{
    [Header("�÷��̾� ����")]
    public PlayerID playerID;

    [Header("�̵� �ӵ�")]
    public float moveSpeed = 5f;

    [Header("�÷��̾� ����")]
    public Color playerColor = Color.white;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Vector2 movement;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        ApplyColor();
    }

    void Update()
    {
        // Temp : �÷��̾� ID�� ���� Ű �Է� �б� ó��
        switch (playerID)
        {
            case PlayerID.Player1:
                movement.x = (Input.GetKey(KeyCode.A) ? -1 : 0) + (Input.GetKey(KeyCode.D) ? 1 : 0);
                movement.y = (Input.GetKey(KeyCode.S) ? -1 : 0) + (Input.GetKey(KeyCode.W) ? 1 : 0);
                break;
            case PlayerID.Player2:
                movement.x = (Input.GetKey(KeyCode.LeftArrow) ? -1 : 0) + (Input.GetKey(KeyCode.RightArrow) ? 1 : 0);
                movement.y = (Input.GetKey(KeyCode.DownArrow) ? -1 : 0) + (Input.GetKey(KeyCode.UpArrow) ? 1 : 0);
                break;
        }

        // �밢�� �̵� �� �ӵ� ����
        movement = movement.normalized;
    }

    void FixedUpdate()
    {
        if (movement != Vector2.zero)
        {
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }
    }

    void ApplyColor()
    {
        if (sr != null)
            sr.color = playerColor;
    }
}
