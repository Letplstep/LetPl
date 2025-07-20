using UnityEngine;

public enum PlayerID
{
    Player1,
    Player2
}

[RequireComponent(typeof(Rigidbody2D))]
public class RhythmPlayer : MonoBehaviour
{
    [Header("플레이어 구분")]
    public PlayerID playerID;

    [Header("이동 속도")]
    public float moveSpeed = 5f;

    [Header("플레이어 색상")]
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
        // Temp : 플레이어 ID에 따라 키 입력 분기 처리
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

        // 대각선 이동 시 속도 보정
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
