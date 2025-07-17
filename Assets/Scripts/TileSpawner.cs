using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSpawner : MonoBehaviour
{
    public GameObject tilePrefab;
    public int width = 3; // 발판 가로 개수
    public int height = 6; // 발판 세로 개수
    public float spacing = 1.1f; //발판 간격

    void Start()
    {
        // Player1 발판 보드 생성 왼쪽
        Vector3 offset1 = new Vector3(-3f, 0, 0);
        SpawnBoard(offset1, TileOwner.Player1);

        // Player2 발판 보드 생성 오른쪽
        Vector3 offset2 = new Vector3(3f, 0, 0);
        SpawnBoard(offset2, TileOwner.Player2);
    }

    void SpawnBoard(Vector3 offset, TileOwner owner)
    {
        for (int x = 0; x < width; x++) // 가로
        {
            for (int y = 0; y < height; y++) // 세로
            {
                // 발판 위치
                Vector3 pos = new Vector3(x * spacing, y * spacing, 0) + offset;

                // 발판 생성
                GameObject tile = Instantiate(tilePrefab, pos, Quaternion.identity);
                tile.tag = "Tile"; // 태그

                // 발판 소유권 설정
                Tile tileScript = tile.GetComponent<Tile>();
                if (tileScript != null)
                {
                    tileScript.territoryOwner = owner; // 원래 진영
                    tileScript.SetOwner(owner); // 현재 소유 플레이어 설정
                }
            }
        }
    }
}
