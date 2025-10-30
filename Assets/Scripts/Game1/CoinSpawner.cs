//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class CoinSpawner : MonoBehaviour
//{
//    [Header("UI 영역 (spawnArea는 코인 전용 Panel)")]
//    public RectTransform spawnArea;

//    [Header("프리팹 (UI Prefab이어야 함: RectTransform 포함)")]
//    public GameObject greenCoinPrefab;
//    public GameObject pinkCoinPrefab;

//    [Header("설정")]
//    public int coinCountPerTeam = 5;
//    public float minDistance = 1000f;      // 픽셀 단위
//    public float baseSafeDistance = 5000f; // 픽셀 단위

//    [Header("기지 (spawnArea와 동일 Canvas 계층의 RectTransform)")]
//    public RectTransform greenBase;
//    public RectTransform pinkBase;

//    // 내부 추적 리스트: 위치 체크 + 생성된 오브젝트 추적
//    private List<Vector2> spawnedPositions = new List<Vector2>();
//    private List<GameObject> spawnedObjects = new List<GameObject>();

//    // 외부에서 안전하게 호출할 수 있는 코루틴 (UI 레이아웃 준비 후 실행)
//    public IEnumerator RespawnAllCoinsCoroutine()
//    {
//        if (spawnArea == null)
//        {
//            Debug.LogError("[CoinSpawner] spawnArea가 할당되지 않았습니다!");
//            yield break;
//        }

//        // UI 레이아웃 보장
//        Canvas.ForceUpdateCanvases();
//        yield return null;

//        // 기존 코인 제거
//        ClearCoins();
//        spawnedPositions.Clear();

//        // 실제 생성
//        SpawnCoinsInternal(greenCoinPrefab, TeamColor.Green);
//        SpawnCoinsInternal(pinkCoinPrefab, TeamColor.Pink);

//        Debug.Log($"[CoinSpawner] RespawnAllCoins 완료 (생성된 오브젝트 수: {spawnedObjects.Count})");
//    }

//    // 편의 호출 (코루틴 내부 실행)
//    public void RespawnAllCoins()
//    {
//        StartCoroutine(RespawnAllCoinsCoroutine());
//    }

//    // 내부 스폰 구현 (태그나 씬 검색에 의존하지 않고 직접 관리)
//    private void SpawnCoinsInternal(GameObject prefab, TeamColor team)
//    {
//        if (prefab == null)
//        {
//            Debug.LogError($"[CoinSpawner] {team} prefab이 할당되지 않았습니다!");
//            return;
//        }

//        int spawned = 0;
//        int attempts = 0;
//        int maxAttempts = 5000;

//        float halfW = spawnArea.rect.width * 0.5f;
//        float halfH = spawnArea.rect.height * 0.5f;

//        while (spawned < coinCountPerTeam && attempts < maxAttempts)
//        {
//            attempts++;

//            Vector2 randomPos = new Vector2(
//                Random.Range(-halfW, halfW),
//                Random.Range(-halfH, halfH)
//            );

//            if (!IsFarEnough(randomPos)) continue;
//            if (IsInsideBase(randomPos)) continue;

//            // Instantiate 후 부모로 붙이고 anchoredPosition 설정 (UI 전용 방식)
//            GameObject coin = Instantiate(prefab);
//            RectTransform rt = coin.GetComponent<RectTransform>();
//            if (rt == null)
//            {
//                Debug.LogWarning("[CoinSpawner] 프리팹에 RectTransform이 없습니다. (UI Prefab인지 확인)");
//                Destroy(coin);
//                continue;
//            }

//            // 부모로 붙이되 로컬 속성 유지
//            rt.SetParent(spawnArea, false);
//            rt.anchoredPosition = randomPos;
//            rt.localScale = Vector3.one;

//            spawnedPositions.Add(randomPos);
//            spawnedObjects.Add(coin);
//            spawned++;

//            // 디버그
//            // Debug.Log($"[CoinSpawner] {team} 코인 생성 #{spawned} at {randomPos} (시도:{attempts})");
//        }

//        if (spawned < coinCountPerTeam)
//            Debug.LogWarning($"[CoinSpawner] {team} 코인 일부만 생성됨: {spawned}/{coinCountPerTeam} (시도:{attempts})");
//    }

//    // 안전하게 생성한 모든 코인 제거
//    public void ClearCoins()
//    {
//        for (int i = spawnedObjects.Count - 1; i >= 0; i--)
//        {
//            if (spawnedObjects[i] != null)
//                Destroy(spawnedObjects[i]);
//        }
//        spawnedObjects.Clear();
//        spawnedPositions.Clear();

//        Debug.Log("[CoinSpawner] ClearCoins: 코인 전부 제거됨");
//    }

//    private bool IsFarEnough(Vector2 newPos)
//    {
//        foreach (Vector2 p in spawnedPositions)
//            if (Vector2.Distance(p, newPos) < minDistance)
//                return false;
//        return true;
//    }

//    private bool IsInsideBase(Vector2 newPos)
//    {
//        if (greenBase != null)
//        {
//            // newPos(SpawnArea 좌표계) → greenBase 좌표계로 변환
//            Vector2 localPos = greenBase.InverseTransformPoint(spawnArea.TransformPoint(newPos));

//            Rect rect = greenBase.rect;
//            rect.xMin -= baseSafeDistance;
//            rect.yMin -= baseSafeDistance;
//            rect.xMax += baseSafeDistance;
//            rect.yMax += baseSafeDistance;

//            if (rect.Contains(localPos))
//                return true;
//        }

//        if (pinkBase != null)
//        {
//            Vector2 localPos = pinkBase.InverseTransformPoint(spawnArea.TransformPoint(newPos));

//            Rect rect = pinkBase.rect;
//            rect.xMin -= baseSafeDistance;
//            rect.yMin -= baseSafeDistance;
//            rect.xMax += baseSafeDistance;
//            rect.yMax += baseSafeDistance;

//            if (rect.Contains(localPos))
//                return true;
//        }

//        return false;
//    }
//}



//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class CoinSpawner : MonoBehaviour
//{
//    [Header("UI 영역 (spawnArea는 코인 전용 Panel)")]
//    public RectTransform spawnArea;

//    [Header("프리팹 (UI Prefab이어야 함: RectTransform 포함)")]
//    public GameObject greenCoinPrefab;
//    public GameObject pinkCoinPrefab;

//    [Header("설정")]
//    public int coinCountPerTeam = 5;
//    public Vector2 coinSize = new Vector2(100f, 100f); // 코인 Rect 크기
//    public float baseSafeDistance = 200f;              // 진영 Rect 확장 여유

//    [Header("기지 (spawnArea와 동일 Canvas 계층의 RectTransform)")]
//    public RectTransform greenBase;
//    public RectTransform pinkBase;

//    // 내부 추적 리스트
//    private List<GameObject> spawnedObjects = new List<GameObject>();

//    // 외부 호출용 코루틴
//    public IEnumerator RespawnAllCoinsCoroutine()
//    {
//        if (spawnArea == null)
//        {
//            Debug.LogError("[CoinSpawner] spawnArea가 할당되지 않았습니다!");
//            yield break;
//        }

//        Canvas.ForceUpdateCanvases();
//        yield return null;

//        ClearCoins();

//        SpawnCoinsInternal(greenCoinPrefab, TeamColor.Green);
//        SpawnCoinsInternal(pinkCoinPrefab, TeamColor.Pink);

//        Debug.Log($"[CoinSpawner] RespawnAllCoins 완료 (생성된 오브젝트 수: {spawnedObjects.Count})");
//    }

//    public void RespawnAllCoins()
//    {
//        StartCoroutine(RespawnAllCoinsCoroutine());
//    }

//    private void SpawnCoinsInternal(GameObject prefab, TeamColor team)
//    {
//        if (prefab == null)
//        {
//            Debug.LogError($"[CoinSpawner] {team} prefab이 할당되지 않았습니다!");
//            return;
//        }

//        int spawned = 0;
//        int attempts = 0;
//        int maxAttempts = 5000;

//        float halfW = spawnArea.rect.width * 0.5f;
//        float halfH = spawnArea.rect.height * 0.5f;

//        while (spawned < coinCountPerTeam && attempts < maxAttempts)
//        {
//            attempts++;

//            Vector2 randomPos = new Vector2(
//                Random.Range(-halfW + coinSize.x * 0.5f, halfW - coinSize.x * 0.5f),
//                Random.Range(-halfH + coinSize.y * 0.5f, halfH - coinSize.y * 0.5f)
//            );

//            Rect newRect = new Rect(randomPos - coinSize * 0.5f, coinSize);

//            if (OverlapsWithOthers(newRect)) continue;
//            if (OverlapsWithBases(newRect)) continue;

//            GameObject coin = Instantiate(prefab, spawnArea);
//            RectTransform rt = coin.GetComponent<RectTransform>();
//            if (rt == null)
//            {
//                Debug.LogWarning("[CoinSpawner] 프리팹에 RectTransform이 없습니다.");
//                Destroy(coin);
//                continue;
//            }

//            rt.anchoredPosition = randomPos;
//            rt.localScale = Vector3.one;

//            spawnedObjects.Add(coin);
//            spawned++;
//        }

//        if (spawned < coinCountPerTeam)
//            Debug.LogWarning($"[CoinSpawner] {team} 코인 일부만 생성됨: {spawned}/{coinCountPerTeam} (시도:{attempts})");
//    }

//    public void ClearCoins()
//    {
//        for (int i = spawnedObjects.Count - 1; i >= 0; i--)
//        {
//            if (spawnedObjects[i] != null)
//                Destroy(spawnedObjects[i]);
//        }
//        spawnedObjects.Clear();
//        Debug.Log("[CoinSpawner] ClearCoins: 코인 전부 제거됨");
//    }

//    // ===== Rect 충돌 검사 =====

//    private bool OverlapsWithOthers(Rect newRect)
//    {
//        foreach (GameObject obj in spawnedObjects)
//        {
//            if (obj == null) continue;
//            RectTransform rt = obj.GetComponent<RectTransform>();
//            if (rt != null)
//            {
//                Rect otherRect = GetLocalRect(rt);
//                if (newRect.Overlaps(otherRect))
//                    return true;
//            }
//        }
//        return false;
//    }

//    private bool OverlapsWithBases(Rect newRect)
//    {
//        if (greenBase != null)
//        {
//            Rect greenRect = ExpandRect(GetLocalRect(greenBase), baseSafeDistance);
//            if (newRect.Overlaps(greenRect)) return true;
//        }

//        if (pinkBase != null)
//        {
//            Rect pinkRect = ExpandRect(GetLocalRect(pinkBase), baseSafeDistance);
//            if (newRect.Overlaps(pinkRect)) return true;
//        }

//        return false;
//    }

//    private Rect GetLocalRect(RectTransform rt)
//    {
//        Vector2 size = rt.rect.size;
//        // 피벗 0.5,0.5 가정
//        Vector2 pos = rt.anchoredPosition - size * 0.5f;
//        return new Rect(pos, size);
//    }

//    private Rect ExpandRect(Rect rect, float margin)
//    {
//        rect.xMin -= margin;
//        rect.yMin -= margin;
//        rect.xMax += margin;
//        rect.yMax += margin;
//        return rect;
//    }
//}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    [Header("UI 영역 (spawnArea는 코인 전용 Panel)")]
    public RectTransform spawnArea;

    [Header("프리팹 (UI Prefab이어야 함: RectTransform 포함)")]
    public GameObject greenCoinPrefab;
    public GameObject pinkCoinPrefab;

    [Header("설정")]
    public int coinCountPerTeam = 5;
    public Vector2 coinSize = new Vector2(100f, 100f); // 코인 Rect 크기

    [Header("진영 영역 크기 (자동계산됨, 인스펙터에서 조정 가능)")]
    public float baseWidth = 300f;   // 진영 가로 크기
    public float baseHeight = 400f;  // 진영 세로 크기

    private Rect greenBaseArea; // 왼쪽 금지영역
    private Rect pinkBaseArea;  // 오른쪽 금지영역
    private List<GameObject> spawnedObjects = new List<GameObject>();

    public IEnumerator RespawnAllCoinsCoroutine()
    {
        if (spawnArea == null)
        {
            Debug.LogError("[CoinSpawner] spawnArea가 할당되지 않았습니다!");
            yield break;
        }

        Canvas.ForceUpdateCanvases();
        yield return null;

        ClearCoins();

        // 패널 크기 기반으로 금지 영역 자동 계산
        CalculateBaseAreas();

        SpawnCoinsInternal(greenCoinPrefab, TeamColor.Green);
        SpawnCoinsInternal(pinkCoinPrefab, TeamColor.Pink);

        Debug.Log($"[CoinSpawner] RespawnAllCoins 완료 (생성된 오브젝트 수: {spawnedObjects.Count})");
    }

    public void RespawnAllCoins()
    {
        StartCoroutine(RespawnAllCoinsCoroutine());
    }

    private void SpawnCoinsInternal(GameObject prefab, TeamColor team)
    {
        if (prefab == null)
        {
            Debug.LogError($"[CoinSpawner] {team} prefab이 할당되지 않았습니다!");
            return;
        }

        int spawned = 0;
        int attempts = 0;

        while (spawned < coinCountPerTeam && attempts < 5000)
        {
            attempts++;
            Vector2 pos = GetRandomSafePosition();

            if (pos == Vector2.negativeInfinity)
                break; // 안전한 위치 못 찾음

            // 코인 생성
            GameObject coin = Instantiate(prefab, spawnArea);
            RectTransform rt = coin.GetComponent<RectTransform>();
            if (rt == null)
            {
                Debug.LogWarning("[CoinSpawner] 프리팹에 RectTransform이 없습니다.");
                Destroy(coin);
                continue;
            }

            rt.anchoredPosition = pos;
            rt.localScale = Vector3.one;

            // 크기 강제 적용 (coinSize 값 반영)
            rt.sizeDelta = coinSize;

            spawnedObjects.Add(coin);
            spawned++;
        }

        if (spawned < coinCountPerTeam)
            Debug.LogWarning($"[CoinSpawner] {team} 코인 일부만 생성됨: {spawned}/{coinCountPerTeam} (시도:{attempts})");
    }

    public void ClearCoins()
    {
        for (int i = spawnedObjects.Count - 1; i >= 0; i--)
        {
            if (spawnedObjects[i] != null)
                Destroy(spawnedObjects[i]);
        }
        spawnedObjects.Clear();
        Debug.Log("[CoinSpawner] ClearCoins: 코인 전부 제거됨");
    }

    // 좌표 뽑기
    private Vector2 GetRandomSafePosition()
    {
        float halfW = spawnArea.rect.width * 0.5f;
        float halfH = spawnArea.rect.height * 0.5f;

        // 금지 영역 확장: 코인 크기만큼 margin 추가
        Rect expandedGreen = ExpandRect(greenBaseArea, coinSize);
        Rect expandedPink = ExpandRect(pinkBaseArea, coinSize);

        for (int i = 0; i < 100; i++) // 최대 100번 시도
        {
            Vector2 randomPos = new Vector2(
                Random.Range(-halfW + coinSize.x * 0.5f, halfW - coinSize.x * 0.5f),
                Random.Range(-halfH + coinSize.y * 0.5f, halfH - coinSize.y * 0.5f)
            );

            Rect newRect = new Rect(randomPos - coinSize * 0.5f, coinSize);

            if (newRect.Overlaps(expandedGreen)) continue;
            if (newRect.Overlaps(expandedPink)) continue;
            if (OverlapsWithOthers(newRect)) continue;

            return randomPos; // 안전한 좌표 찾음
        }

        Debug.LogWarning("[CoinSpawner] 안전한 위치를 찾지 못했습니다.");
        return Vector2.negativeInfinity;
    }

    // 충돌 검사
    private bool OverlapsWithOthers(Rect newRect)
    {
        foreach (GameObject obj in spawnedObjects)
        {
            if (obj == null) continue;
            RectTransform rt = obj.GetComponent<RectTransform>();
            if (rt != null)
            {
                Rect otherRect = GetLocalRect(rt);
                if (newRect.Overlaps(otherRect))
                    return true;
            }
        }
        return false;
    }

    private Rect GetLocalRect(RectTransform rt)
    {
        Vector2 size = rt.rect.size;
        Vector2 pos = rt.anchoredPosition - size * 0.5f; // 피벗 0.5,0.5 가정
        return new Rect(pos, size);
    }

    private Rect ExpandRect(Rect rect, Vector2 margin)
    {
        rect.xMin -= margin.x;
        rect.yMin -= margin.y;
        rect.xMax += margin.x;
        rect.yMax += margin.y;
        return rect;
    }
    private void OnDrawGizmos()
    {
        if (spawnArea == null) return;

        // 금지 영역들을 그릴 때 local -> world 변환
        DrawRectGizmo(greenBaseArea, Color.green);
        DrawRectGizmo(pinkBaseArea, Color.red);
    }

    private void DrawRectGizmo(Rect rect, Color color)
    {
        // 기존 행렬 저장
        Matrix4x4 oldMatrix = Gizmos.matrix;

        // spawnArea의 회전과 스케일 적용
        Gizmos.matrix = Matrix4x4.TRS(spawnArea.position, spawnArea.rotation, spawnArea.lossyScale);

        Gizmos.color = color;
        Gizmos.DrawWireCube(rect.center, rect.size);

        // 원래 행렬 복구
        Gizmos.matrix = oldMatrix;
    }


    // 자동 금지 영역 계산
    private void CalculateBaseAreas()
    {
        float halfW = spawnArea.rect.width * 0.5f;

        //float offsetX = 100f; // 패널 중앙에서 오른쪽/왼쪽으로 이동
        //greenBaseArea = new Rect(
        //    -offsetX - baseWidth * 0.5f,
        //    -baseHeight * 0.5f,
        //    baseWidth,
        //    baseHeight
        //);

        //pinkBaseArea = new Rect(
        //    offsetX - baseWidth * 0.5f,
        //    -baseHeight * 0.5f,
        //    baseWidth,
        //    baseHeight
        //);
        // 왼쪽 진영
        greenBaseArea = new Rect(
            -halfW,              // 왼쪽 시작
            -baseHeight * 0.5f,  // 세로 중앙 기준
            baseWidth,           // 가로
            baseHeight           // 세로
        );

        // 오른쪽 진영
        pinkBaseArea = new Rect(
            halfW - baseWidth,   // 오른쪽 끝에서 baseWidth만큼 빼서 시작
            -baseHeight * 0.5f,
            baseWidth,
            baseHeight
        );
    }
}