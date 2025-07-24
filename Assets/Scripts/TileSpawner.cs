//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class TileSpawner : MonoBehaviour
//{
//    public Camera targetCamera; // Inspector���� ���� ������ ī�޶�
//    public GameObject tilePrefab;
//    public int width = 3;      // ���� ���� ���� (X�� ����)
//    public int height = 6;     // ���� ���� ���� (Z�� ����)
//    public float spacing = 1.1f; // ���� ����
//    public float tileScale = 2f; // Ÿ�� ũ�� ����



//    void Start()
//    {
//        if (targetCamera == null)
//        {
//            return;
//        }

//        float boardWidth = width * spacing * tileScale;  // �� ���� ���� ����
//        float distanceFromCamera = 5f;

//        // ī�޶� �߽� ���� ���� ����
//        Vector3 center = targetCamera.transform.position + targetCamera.transform.forward * distanceFromCamera;

//        // ���� ���� �ʺ� + ���� ���� ��ŭ �¿�� ������
//        float sideOffset = boardWidth / 2f+1f;

//        // Player1 ����: �߽ɿ��� ���� (ī�޶��� ������ ����)
//        Vector3 offset1 = center + targetCamera.transform.right * -sideOffset;
//        SpawnBoard(offset1, TileOwner.Player1);

//        // Player2 ����: �߽ɿ��� ������
//        Vector3 offset2 = center + targetCamera.transform.right * sideOffset;
//        SpawnBoard(offset2, TileOwner.Player2);
//    }

//    void SpawnBoard(Vector3 center, TileOwner owner)
//    {
//        float tileScale = 2.5f;             // Ÿ�� ũ��
//        float spacingAdjusted = spacing * tileScale;  // ���ݵ� Ű��

//        for (int x = 0; x < width; x++)
//        {
//            for (int z = 0; z < height; z++)
//            {
//                // ������ ���� ��ġ ���
//                Vector3 localPos = new Vector3(
//                (x - (width - 1) / 2f) * spacingAdjusted,    // ��ü ���� ũ�� Ȧ������ ��� ���� ����
//                0,
//                (z - (height - 1) / 2f) * spacingAdjusted            
//            );


//                Vector3 worldPos = center + localPos;

//                // Ÿ�� ���� �� ȸ�� (�ٴڿ� ������)
//                GameObject tile = Instantiate(tilePrefab, worldPos, Quaternion.Euler(90f, 0f, 0f));
//                tile.transform.localScale = Vector3.one * tileScale; // ũ�� ����
//                tile.tag = "Tile";

//                // ������ ����
//                Tile tileScript = tile.GetComponent<Tile>();
//                if (tileScript != null)
//                {
//                    tileScript.territoryOwner = owner;
//                    tileScript.SetOwner(owner);
//                }
//            }
//        }
//    }
//}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TileSpawner : MonoBehaviour
{
    public Transform canvasTransform;   // ĵ���� �ִ� Ʈ������
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
        float sideOffset = boardWidth / 2f + 10f; // ���� ����

        // ĵ���� ��ġ ���� ����/���������� ������
        Vector3 offset1 = canvasTransform.position + canvasTransform.right * -sideOffset;
        Vector3 offset2 = canvasTransform.position + canvasTransform.right * sideOffset;

        SpawnBoard(offset1, TileOwner.Player1);
        SpawnBoard(offset2, TileOwner.Player2);
    }

    void SpawnBoard(Vector3 center, TileOwner owner)
    {

        float tileScaleLocal = tileScale;
        float spacingAdjusted = spacing * tileScaleLocal;

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 localPos = new Vector3(
                    (x - (width - 1) / 2f) * spacingAdjusted,
                    0,
                    (z - (height - 1) / 2f) * spacingAdjusted
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