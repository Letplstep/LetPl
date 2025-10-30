using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSpawner : MonoBehaviour
{
    public Transform canvasTransform;   // 캔버스 있는 트랜스폼
    public GameObject tilePrefab;
    public int width = 3;
    public int height = 6;
    public float spacing = 1.5f;
    public float tileScale = 4f;

    void Start()
    {
        if (canvasTransform == null)
        {
            return;
        }

        float boardWidth = width * spacing * tileScale;
        float sideOffset = boardWidth / 2f + 16f; // 양쪽 간격

        // 캔버스 위치 기준 왼쪽/오른쪽으로 오프셋
        Vector3 offset1 = canvasTransform.position + canvasTransform.right * -sideOffset;
        Vector3 offset2 = canvasTransform.position + canvasTransform.right * sideOffset;

        SpawnBoard(offset1, TileOwner.Player1);
        SpawnBoard(offset2, TileOwner.Player2);
    }

    void SpawnBoard(Vector3 center, TileOwner owner)
    {
        float tileScaleLocal = tileScale;
        float spacingX = spacing * tileScaleLocal;
        float spacingZ = spacing * tileScaleLocal * 1.05f;

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 localPos = new Vector3(
                    (x - (width - 1) / 2f) * spacingX,
                    0,
                    (z - (height - 1) / 2f) * spacingZ
                );
                Vector3 worldPos = center + localPos;

                GameObject tile = Instantiate(tilePrefab, worldPos, Quaternion.Euler(90f, 0f, 0f));
                tile.transform.localScale = Vector3.one * tileScaleLocal;
                tile.tag = "Tile";

                Tile tileScript = tile.GetComponent<Tile>();
                if (tileScript != null)
                {
                    tileScript.territoryOwner = owner;
                    tileScript.SetOwner(owner);
                }
            }
        }
    }
}