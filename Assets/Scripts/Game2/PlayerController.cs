using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // 이동 속도
    public string horizontalAxis = "Horizontal1";
    public string verticalAxis = "Vertical1";
    public TileOwner myOwner = TileOwner.Player1; //플레이어 소속

    // 영역 제한 좌표 설정 (필요에 따라 조정 가능)
    private float minX, maxX, minY, maxY;

    void Start()
    {
        // 이동 가능한 X축 범위를 설정
        if (myOwner == TileOwner.Player1)
        {
            minX = -3f; maxX = -0.8f; // 왼쪽
        }
        else if (myOwner == TileOwner.Player2)
        {
            minX = 3f; maxX = 5.2f; //오른쪽
        }

        // Y축은 공통
        minY = 0f;
        maxY = 5.5f;
    }

    void Update()
    {
        float h = Input.GetAxisRaw(horizontalAxis);
        float v = Input.GetAxisRaw(verticalAxis);

        Vector3 movement = new Vector3(h, v, 0) * moveSpeed * Time.deltaTime;

        //플레이어 이동
        transform.Translate(movement);

        // 이동 범위 제한
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        transform.position = pos;
    }
}