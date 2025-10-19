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
    public TestSceneSetup sceneSetup;

    [Header("콜라이더 설정")]
    public float colliderRadius = 0.5f;
    public Vector3 colliderCenter = Vector3.zero;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;

        // Y 위치 고정 + 모든 축 회전 고정 (X, Y, Z)
        rb.constraints = RigidbodyConstraints.FreezePositionY |
                         RigidbodyConstraints.FreezeRotationX |
                         RigidbodyConstraints.FreezeRotationY |
                         RigidbodyConstraints.FreezeRotationZ;

        SphereCollider col = GetComponent<SphereCollider>();
        if (col != null)
        {
            col.radius = colliderRadius;
            col.center = colliderCenter;
        }

        Vector3 moveDir = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
        rb.velocity = moveDir * speed;

        transform.rotation = Quaternion.Euler(90f, 0f, 0f);

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

        float y = planArea.GetY();
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }

    void OnMouseDown()
    {
        RemoveBox(); // 클릭은 항상 가능
    }

    void OnCollisionEnter(Collision collision)
    {
        GameObject otherObj = collision.collider.gameObject;

        // 내부 오브젝트 태그
        string[] internalTags = { "Platform", "Box", "Wall" };

        // 플랫폼끼리 충돌 → 반사 이동
        if (otherObj.CompareTag("Platform"))
        {
            Vector3 normal = (transform.position - otherObj.transform.position).normalized;
            rb.velocity = Vector3.Reflect(rb.velocity, normal).normalized * speed;
            return;
        }

        // 내부 오브젝트와 충돌 시 무시
        if (System.Array.Exists(internalTags, tag => otherObj.CompareTag(tag)))
        {
            return;
        }

        // 외부 오브젝트와 충돌 → 기존 로직 수행
        RemoveBox();
    }

    void OnTriggerEnter(Collider other)
{
    GameObject otherObj = other.gameObject;

    // 내부 오브젝트 태그 (기존과 동일)
    string[] internalTags = { "Platform", "Box", "Wall" };

    // 내부 오브젝트면 무시
    if (System.Array.Exists(internalTags, tag => otherObj.CompareTag(tag)))
        return;

    // 외부 오브젝트 감지 시
    Debug.Log($"[Platform {objectNumber}] 외부 트리거 충돌 감지 → {otherObj.name}");

    RemoveBox();
}

    private void RemoveBox()
    {
        int idx = objectNumber - 1;
        bool correctStep = mainObjects != null && idx >= 0 && idx < mainObjects.Length && mainObjects[idx] != null;

        if (sceneSetup != null)
        {
            sceneSetup.PlaySfx(correctStep);
        }

        if (!correctStep)
        {
            Debug.Log($"[Platform {objectNumber}] 잘못된 발판 → 박 없음, 무시");
            return;
        }

        MainObject target = mainObjects[idx];
        Destroy(target.gameObject);
        mainObjects[idx] = null;

        Debug.Log($"[Platform {objectNumber}] 박 {objectNumber} 제거됨");

        if (sceneSetup != null)
        {
            Vector2 platformPosXZ = new Vector2(transform.position.x, transform.position.z);
            sceneSetup.OnBoxRemoved(platformPosXZ, idx);
        }

        Destroy(this.gameObject);
    }
}
