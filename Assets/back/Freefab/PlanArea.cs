using UnityEngine;

[System.Serializable]
public class PlanArea : MonoBehaviour
{
    [Header("ZX 평면 크기 (중앙 기준)")]
    public Vector2 size = new Vector2(5f, 5f);

    [Header("Y축 기준 (PlanArea 오브젝트의 높이로 결정)")]
    public bool useTransformY = true;

    [Header("고정 높이 (useTransformY=false일 때만 적용)")]
    public float fixedY = 0f;

    // 현재 사용할 Y좌표 반환 (생성 시만 사용)
    public float GetY()
    {
        return useTransformY ? transform.position.y : fixedY;
    }

    // ZX 기준 범위 제한 (중심 기준)
    public Vector2 ClampPosition(Vector3 pos)
    {
        Vector2 halfSize = size / 2f;
        float x = Mathf.Clamp(pos.x, transform.position.x - halfSize.x, transform.position.x + halfSize.x);
        float z = Mathf.Clamp(pos.z, transform.position.z - halfSize.y, transform.position.z + halfSize.y);
        return new Vector2(x, z);
    }

    // ZX 평면 표시
    void OnDrawGizmos()
    {
        Vector3 center = transform.position;
        Vector3 cubeSize = new Vector3(size.x, 0.1f, size.y);

        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
        Gizmos.DrawCube(center, cubeSize);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(center, cubeSize);
    }
}
