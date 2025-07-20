using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NoteJudgeResult
{
    None,   // 아직 판정되지 않음 => 디폴트
    OK,     // 밟음
    Miss    // 안 밟음 또는 시간 초과
}

public class RhythmNote : MonoBehaviour
{
    [Header("노트 타입 설정, 타입별 점수")]
    public NoteType noteType = NoteType.Single;
    public int singleScore = 1;
    public int syncScore = 4;

    [Header("노트 판정 시간")]
    public float lifetime = 5f;               // 발판 유효시간
    //public float excellentThreshold = 0.2f;   // 현재 미사용
    //public float goodThreshold = 0.5f;        // 현재 미사용
    //public float badThreshold = 1.0f;         // 현재 미사용

    [Header("외형 설정")]
    public Sprite sprite;            // 발판 이미지
    public Color color = Color.white; // 색상 (디버깅/식별용)

    [Header("판정 결과")]
    public NoteJudgeResult judgeResult = NoteJudgeResult.None;
    private float spawnTime;  // 노트가 생성된 시간
    private bool judged = false;// 이미 판정되었는지 여부

    [Header("판정 성공 이펙트")]
    public GameObject hitEffectPrefab;

    [Header("판정 비쥬얼바")]
    private Transform visualBarTransform;
    private Vector3 visualBarInitialScale;
    private Vector3 visualBarTargetScale; // 이게 RhythmNote 자체 크기 (0.5, 0.5, 0.5)
    private float elapsedTime = 0f;
    private bool isActive = false;


    void Start()
    {
        // SpriteRenderer의 색상을 설정
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = color;
        }

        // 하위의 VisualBar 초기화
        Transform visualBar = transform.GetChild(0);
        if (visualBar != null)
        {
            visualBarTransform = visualBar;
            visualBarInitialScale = visualBar.localScale;
            visualBarTargetScale = Vector3.one; // transform.localScale;
        }
    }

    private void OnEnable()
    {
        // 타임라인 시작 이후인지 체크
        if (Time.timeSinceLevelLoad < 0.1f) return;


        // 생성 시각 기록
        spawnTime = Time.time;

        // 비쥬얼 바 타이머 초기화
        elapsedTime = 0f;
        isActive = true;

        // 생성 사운드
        AudioManager.Instance.PlayCreateSFX();

        // 일정 시간이 지나면 자동으로 제거
        Invoke(nameof(SelfDestruct), lifetime);

    }


    // 트리거 충돌로 범위 안에 들어왔는지 체크
    void OnTriggerEnter2D(Collider2D other)
    {
        if (judged) return;

        if (noteType == NoteType.Single)
        {
            // Single: 누가 들어와도 OK
            TryJudge();
        }
        else if (noteType == NoteType.Sync)
        {
            // Sync: 현재 이 오브젝트와 충돌 중인 모든 콜라이더를 체크
            Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, GetComponent<Collider2D>().bounds.size, 0f);

            bool player1Found = false;
            bool player2Found = false;

            foreach (var hit in hits)
            {
                if (hit.CompareTag("Player1")) player1Found = true;
                if (hit.CompareTag("Player2")) player2Found = true;
            }

            if (player1Found && player2Found)
            {
                TryJudge();
            }
        }
    }


    // 비쥬얼 바 줄이기
    void Update()
    {
        if (visualBarTransform != null && !judged)
        {
            float t = Mathf.Clamp01((Time.time - spawnTime) / lifetime);
            visualBarTransform.localScale = Vector3.Lerp(visualBarInitialScale, visualBarTargetScale, t);
        }
    }

    // 플레이어가 밟으려고 시도할 때 호출
    public void TryJudge()
    {
        if (judged) return;

        float elapsed = Time.time - spawnTime;
        if (elapsed > lifetime) return;

        // 이 시점에서는 플레이어 감지되었으므로 OK
        judgeResult = NoteJudgeResult.OK;
        judged = true;

        Debug.Log($"[Judge OK] {name}");

        // 점수 증가. 합동이면 4점, 개인은 1점
        int scoreToAdd = (noteType == NoteType.Single) ? singleScore : syncScore;
        RhythmGameManager.Instance.AddScore(scoreToAdd);

        // 발판 밟기 성공 이펙트
        if (hitEffectPrefab != null)
        {
            GameObject effect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            Destroy(effect, 0.5f); // 애니메이션 길이만큼
        }


        // 제거 사운드
        AudioManager.Instance.PlayHitSFX();

        Destroy(gameObject);
    }

    // 노트가 시간 내에 밟히지 않았을 때 호출
    void SelfDestruct()
    {
        // 이미 판정되었으면 아무 것도 하지 않음
        if (judged) return;

        // 안 밟은 것으로 간주
        judgeResult = NoteJudgeResult.Miss;
        Debug.Log($"[Judge] {name} → Miss (시간 초과)");

        // 오브젝트 제거
        Destroy(gameObject);
    }
} // end class
