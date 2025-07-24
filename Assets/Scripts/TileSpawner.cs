//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class TileSpawner : MonoBehaviour
//{
//    public Camera targetCamera; // Inspector에서 직접 연결할 카메라
//    public GameObject tilePrefab;
//    public int width = 3;      // 발판 가로 개수 (X축 기준)
//    public int height = 6;     // 발판 세로 개수 (Z축 기준)
//    public float spacing = 1.1f; // 발판 간격
//    public float tileScale = 2f; // 타일 크기 배율



//    void Start()
//    {
//        if (targetCamera == null)
//        {
//            return;
//        }

//        float boardWidth = width * spacing * tileScale;  // 한 보드 가로 길이
//        float distanceFromCamera = 5f;

//        // 카메라 중심 기준 생성 지점
//        Vector3 center = targetCamera.transform.position + targetCamera.transform.forward * distanceFromCamera;

//        // 보드 절반 너비 + 여유 공간 만큼 좌우로 벌리기
//        float sideOffset = boardWidth / 2f+1f;

//        // Player1 보드: 중심에서 왼쪽 (카메라의 오른쪽 기준)
//        Vector3 offset1 = center + targetCamera.transform.right * -sideOffset;
//        SpawnBoard(offset1, TileOwner.Player1);

//        // Player2 보드: 중심에서 오른쪽
//        Vector3 offset2 = center + targetCamera.transform.right * sideOffset;
//        SpawnBoard(offset2, TileOwner.Player2);
//    }

//    void SpawnBoard(Vector3 center, TileOwner owner)
//    {
//        float tileScale = 2.5f;             // 타일 크기
//        float spacingAdjusted = spacing * tileScale;  // 간격도 키움

//        for (int x = 0; x < width; x++)
//        {
//            for (int z = 0; z < height; z++)
//            {
//                // 보드의 로컬 위치 계산
//                Vector3 localPos = new Vector3(
//                (x - (width - 1) / 2f) * spacingAdjusted,    // 전체 보드 크기 홀수여서 가운데 정렬 위함
//                0,
//                (z - (height - 1) / 2f) * spacingAdjusted            
//            );


//                Vector3 worldPos = center + localPos;

//                // 타일 생성 및 회전 (바닥에 눕히기)
//                GameObject tile = Instantiate(tilePrefab, worldPos, Quaternion.Euler(90f, 0f, 0f));
//                tile.transform.localScale = Vector3.one * tileScale; // 크기 조절
//                tile.tag = "Tile";

//                // 소유자 설정
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
        float sideOffset = boardWidth / 2f + 10f; // 양쪽 간격

        // 캔버스 위치 기준 왼쪽/오른쪽으로 오프셋
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