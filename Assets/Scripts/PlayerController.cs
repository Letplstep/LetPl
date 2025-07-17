using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // �̵� �ӵ�
    public string horizontalAxis = "Horizontal1";
    public string verticalAxis = "Vertical1";
    public TileOwner myOwner = TileOwner.Player1; //�÷��̾� �Ҽ�

    // ���� ���� ��ǥ ���� (�ʿ信 ���� ���� ����)
    private float minX, maxX, minY, maxY;

    void Start()
    {
        // �̵� ������ X�� ������ ����
        if (myOwner == TileOwner.Player1)
        {
            minX = -3f; maxX = -0.8f; // ����
        }
        else if (myOwner == TileOwner.Player2)
        {
            minX = 3f; maxX = 5.2f; //������
        }

        // Y���� ����
        minY = 0f;
        maxY = 5.5f;
    }

    void Update()
    {
        float h = Input.GetAxisRaw(horizontalAxis);
        float v = Input.GetAxisRaw(verticalAxis);

        Vector3 movement = new Vector3(h, v, 0) * moveSpeed * Time.deltaTime;

        //�÷��̾� �̵�
        transform.Translate(movement);

        // �̵� ���� ����
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        transform.position = pos;
    }
}