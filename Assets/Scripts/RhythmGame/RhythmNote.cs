using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum NoteJudgeResult
{
    None,   // ���� �������� ���� => ����Ʈ
    OK,     // ����
    Miss    // �� ���� �Ǵ� �ð� �ʰ�
}

public class RhythmNote : MonoBehaviour
{
    [Header("��Ʈ Ÿ�� ����, Ÿ�Ժ� ����")]
    public NoteType noteType = NoteType.Single;
    public int singleScore = 1;
    public int syncScore = 4;

    [Header("��Ʈ ���� �ð�")]
    public float lifetime = 5f;               // ���� ��ȿ�ð�
    //public float excellentThreshold = 0.2f;   // ���� �̻��
    //public float goodThreshold = 0.5f;        // ���� �̻��
    //public float badThreshold = 1.0f;         // ���� �̻��

    [Header("���� ����")]
    public Sprite sprite;            // ���� �̹���
    public Color color = Color.white; // ���� (�����/�ĺ���)

    [Header("���� ���")]
    public NoteJudgeResult judgeResult = NoteJudgeResult.None;
    private float spawnTime;  // ��Ʈ�� ������ �ð�
    private bool judged = false;// �̹� �����Ǿ����� ����

    [Header("���� ���� ����Ʈ")]
    public GameObject hitEffectPrefab;
    public GameObject judgeEffect;

    [Header("���� ������")]
    private Transform visualBarTransform;
    private Vector3 visualBarInitialScale;
    private Vector3 visualBarTargetScale; // �̰� RhythmNote ��ü ũ�� (0.5, 0.5, 0.5)
    private float elapsedTime = 0f;
    private bool isActive = false;


    void Start()
    {
        // SpriteRenderer�� ������ ����
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = color;
        }

        // ������ VisualBar �ʱ�ȭ
        Transform visualBar = judgeEffect.transform;
        if (visualBar != null)
        {
            visualBarTransform = visualBar;
            visualBarInitialScale = visualBar.localScale;
            visualBarTargetScale = Vector3.one; // transform.localScale;
        }
    }

    private void OnEnable()
    {
        // Ÿ�Ӷ��� ���� �������� üũ
        if (Time.timeSinceLevelLoad < 0.1f) return;

        // ���� �ð� ���
        spawnTime = Time.time;

        // ����� �� Ÿ�̸� �ʱ�ȭ
        elapsedTime = 0f;
        isActive = true;

        // ���� ����
        AudioManager.Instance.PlayCreateSFX();

        // ���� �ð��� ������ �ڵ����� ����
        Invoke(nameof(SelfDestruct), lifetime);

    }

    private int touchPointCount = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TouchPoint"))
        {
            touchPointCount++;
            Debug.Log($"[Enter] TouchPoint Count: {touchPointCount}");

            if (!judged && noteType == NoteType.Sync && touchPointCount >= 2)
            {
                TryJudge();
            }
            else if (!judged && noteType == NoteType.Single)
            {
                TryJudge();
            }
        }

        //if (judged) return;

        //// 250721 TouchPoint
        //if (other.tag == "TouchPoint")
        //{
        //    if (noteType == NoteType.Single)
        //    {
        //        // Single: ���� ���͵� OK
        //        TryJudge();
        //    }
        //    else if (noteType == NoteType.Sync)
        //    {
        //        Debug.Log("��ũ�е� �ݶ��̴� ���� Ȯ��");

        //        // ���� �浹 ���� ��� �ݶ��̴� Ȯ�� (3D)
        //        Collider[] hits = Physics.OverlapBox(
        //            transform.position,
        //            GetComponent<Collider>().bounds.extents,
        //            Quaternion.identity
        //        );

        //        if (hits.Length >= 2) {
        //            Debug.Log("��ũ�е� �ݶ��̴� 2�� Ȯ��");
        //            TryJudge();
        //        }
        //    }
        //}
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("TouchPoint"))
        {
            touchPointCount--;
            Debug.Log($"[Exit] TouchPoint Count: {touchPointCount}");
        }
    }


    // ����� �� ���̱�
    void Update()
    {
        if (visualBarTransform != null && !judged)
        {
            float t = Mathf.Clamp01((Time.time - spawnTime) / lifetime);
            visualBarTransform.localScale = Vector3.Lerp(visualBarInitialScale, visualBarTargetScale, t);
        }
    }

    // �÷��̾ �������� �õ��� �� ȣ��
    public void TryJudge()
    {
        if (judged) return;

        float elapsed = Time.time - spawnTime;
        if (elapsed > lifetime) return;

        // �� ���������� �÷��̾� �����Ǿ����Ƿ� OK
        judgeResult = NoteJudgeResult.OK;
        judged = true;

        Debug.Log($"[Judge OK] {name}");

        // ���� ����. �յ��̸� 4��, ������ 1��
        int scoreToAdd = (noteType == NoteType.Single) ? singleScore : syncScore;
        RhythmGameManager.Instance.AddScore(scoreToAdd);

        // ���� ��� ���� ����Ʈ
        //if (hitEffectPrefab != null)
        //{
        //    GameObject effect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        //    effect.transform.SetParent(transform); // ���� ������Ʈ�� �ڽ����� ����
        //    Destroy(effect, 1f);
        //}
        if (hitEffectPrefab != null)
        {
            // �̹��� ��Ȱ��ȭ?
            gameObject.GetComponent<RawImage>().enabled = false;
            judgeEffect.SetActive(false);

            hitEffectPrefab.SetActive(true);
        }

        // ���� ����
        AudioManager.Instance.PlayHitSFX();

        Destroy(gameObject, 0.5f);
    }

    // ��Ʈ�� �ð� ���� ������ �ʾ��� �� ȣ��
    void SelfDestruct()
    {
        // �̹� �����Ǿ����� �ƹ� �͵� ���� ����
        if (judged) return;

        // �� ���� ������ ����
        judgeResult = NoteJudgeResult.Miss;
        Debug.Log($"[Judge] {name} �� Miss (�ð� �ʰ�)");

        // ������Ʈ ����
        Destroy(gameObject);
    }
} // end class
