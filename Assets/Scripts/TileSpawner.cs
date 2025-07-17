using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSpawner : MonoBehaviour
{
    public GameObject tilePrefab;
    public int width = 3; // ���� ���� ����
    public int height = 6; // ���� ���� ����
    public float spacing = 1.1f; //���� ����

    void Start()
    {
        // Player1 ���� ���� ���� ����
        Vector3 offset1 = new Vector3(-3f, 0, 0);
        SpawnBoard(offset1, TileOwner.Player1);

        // Player2 ���� ���� ���� ������
        Vector3 offset2 = new Vector3(3f, 0, 0);
        SpawnBoard(offset2, TileOwner.Player2);
    }

    void SpawnBoard(Vector3 offset, TileOwner owner)
    {
        for (int x = 0; x < width; x++) // ����
        {
            for (int y = 0; y < height; y++) // ����
            {
                // ���� ��ġ
                Vector3 pos = new Vector3(x * spacing, y * spacing, 0) + offset;

                // ���� ����
                GameObject tile = Instantiate(tilePrefab, pos, Quaternion.identity);
                tile.tag = "Tile"; // �±�

                // ���� ������ ����
                Tile tileScript = tile.GetComponent<Tile>();
                if (tileScript != null)
                {
                    tileScript.territoryOwner = owner; // ���� ����
                    tileScript.SetOwner(owner); // ���� ���� �÷��̾� ����
                }
            }
        }
    }
}
