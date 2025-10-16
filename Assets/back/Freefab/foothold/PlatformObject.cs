using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
public class PlatformObject : MonoBehaviour
{
    [Header("기본 설정")]
    public int objectNumber;
    public MainObject[] mainObjects;
    public PlanArea planArea;
    public float speed = 2f;

    [HideInInspector]
    public TestSceneSetup sceneSetup; // 연동용

    [Header("콜라이더 설정 (인스펙터에서 조절 가능)")]
    public float colliderRadius = 0.5f;
    public Vector3 colliderCenter = Vector3.zero;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;

        // Y축 고정, 회전 XZ 자유
        rb.constraints = RigidbodyConstraints.FreezePositionY |
                         RigidbodyConstraints.FreezeRotationX |
                         RigidbodyConstraints.FreezeRotationZ;

        // SphereCollider 크기/위치 설정
        SphereCollider col = GetComponent<SphereCollider>();
        if (col != null)
        {
            col.radius = colliderRadius;
            col.center = colliderCenter;
        }

        // 랜덤 XZ 방향 이동
        Vector3 moveDir = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
        rb.velocity = moveDir * speed;

        // Sprite XY → XZ
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        // 태그 지정
        gameObject.tag = "Platform";
        foreach (var box in mainObjects)
        {
            if (box != null) box.gameObject.tag = "Box";
        }
    }

void FixedUpdate()
{
    if (planArea == null) return;

    Vector3 pos = transform.position;
    Vector3 halfSize = new Vector3(planArea.size.x / 2f, 0f, planArea.size.y / 2f);
    Vector3 center = planArea.transform.position;

    Vector3 vel = rb.velocity;

    if (pos.x <= center.x - halfSize.x && vel.x < 0) vel.x = -vel.x;
    if (pos.x >= center.x + halfSize.x && vel.x > 0) vel.x = -vel.x;
    if (pos.z <= center.z - halfSize.z && vel.z < 0) vel.z = -vel.z;
    if (pos.z >= center.z + halfSize.z && vel.z > 0) vel.z = -vel.z;

    rb.velocity = vel.normalized * speed;

    // ✅ Y축 PlanArea 값으로 고정
    float y = planArea.GetY();
    transform.position = new Vector3(transform.position.x, y, transform.position.z);
}

    void OnMouseDown()
    {
        RemoveBox();
    }

    void OnCollisionEnter(Collision collision)
    {
        GameObject otherObj = collision.collider.gameObject;

        if (otherObj.CompareTag("Platform"))
        {
            Vector3 normal = (transform.position - otherObj.transform.position).normalized;
            rb.velocity = Vector3.Reflect(rb.velocity, normal).normalized * speed;
            return;
        }

        if (otherObj.CompareTag("Box"))
            return;

        RemoveBox();
    }

    private void RemoveBox()
    {
        int idx = objectNumber - 1;

        bool correctStep = mainObjects != null && idx >= 0 && idx < mainObjects.Length && mainObjects[idx] != null;

        // 효과음 재생
        if (sceneSetup != null)
        {
            sceneSetup.PlaySfx(correctStep);
        }

        if (!correctStep)
        {
            Debug.Log($"발판 {objectNumber} 클릭/충돌 → 잘못된 발판, 박 없음, 발판은 유지");
            return;
        }

        // 올바른 발판일 경우 박 제거
        MainObject target = mainObjects[idx];
        Destroy(target.gameObject);
        mainObjects[idx] = null;

        Debug.Log($"발판 {objectNumber} → 박 {objectNumber} 제거");

        if (sceneSetup != null)
        {
            Vector2 platformPosXZ = new Vector2(transform.position.x, transform.position.z);
            sceneSetup.OnBoxRemoved(platformPosXZ, idx);
        }

        // 올바른 발판 밟힌 경우 발판도 제거
        Destroy(this.gameObject);
    }
}
