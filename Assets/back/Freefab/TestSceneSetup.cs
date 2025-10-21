using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UI;

public class TestSceneSetup : MonoBehaviour
{
    public float finishBoxLifetime = 5f; // 🕒 Finish 박 유지 시간 (초 단위, 인스펙터에서 조정 가능)

    [Header("설정")]
    public PlanArea planArea;

    [Header("박 프리팹 배열 (순서대로 번호 부여)")]
    public GameObject[] mainPrefabs;

    [Header("발판 프리팹 배열 (순서대로 번호 부여)")]
    public GameObject[] platformPrefabs;

    [Header("박 초기 위치 (ZX 평면 좌표)")]
    public Vector2[] fixedMainPositions;

    [Header("라운드 수 (박/발판 갯수 통합)")]
    public int rounds = 5;

    [Header("발판 이동 속도")]
    public float platformSpeed = 2f;

    [Header("박 설정")]
    public float mainSphereRadius = 2.5f;

    [Header("Finish 박 프리팹")]
    public GameObject finishPrefab;

    [Header("Finish 박 위치 설정 (사용하지 않음)")]
    public Vector2 finishBoxPosition = Vector2.zero;
    public float finishBoxY = 0f;

    [Header("UI 표시용 레거시 텍스트")]
    public Text roundsText;

    [Header("효과음")]
    public AudioClip stepCorrectSfx;
    public AudioClip stepWrongSfx;
    public AudioClip finishBoxSfx;

    [Header("효과음 볼륨 (증폭 가능 0~3)")]
    [Range(0f, 3f)] public float stepCorrectVolume = 1f;
    [Range(0f, 3f)] public float stepWrongVolume = 1f;
    [Range(0f, 3f)] public float finishBoxVolume = 1f;

    [Header("튜토리얼 슬라이드쇼 연결")]
    public TutorialSlideshow tutorialSlideshow;

    [HideInInspector] public MainObject[] mains;
    [HideInInspector] public int remainingRounds;

    private AudioSource audioSource;

void Start()
{
    if (tutorialSlideshow != null)
    {
        tutorialSlideshow.OnSlideshowFinished += OnTutorialFinished;
    }
    else
    {
        // 튜토리얼 없으면 바로 세팅 시작
        SetupScene();
    }

    // 초기화 (fixedMainPositions 모두 첫 위치로 초기화)
    if (fixedMainPositions != null && fixedMainPositions.Length > 0)
    {
        Vector2 basePos = fixedMainPositions[0];
        for (int i = 1; i < fixedMainPositions.Length; i++)
        {
            fixedMainPositions[i] = basePos;
        }
    }

    remainingRounds = rounds;
    UpdateRoundsUI();

    audioSource = gameObject.AddComponent<AudioSource>();
}

    private void OnTutorialFinished()
    {
        Debug.Log("튜토리얼 종료, 게임 씬 세팅 시작");
        SetupScene();
    }

    void SetupScene()
    {
        int boxCount = Mathf.Min(rounds, mainPrefabs.Length, fixedMainPositions.Length);
        if (boxCount == 0)
        {
            Debug.LogWarning("박 또는 발판 프리팹/위치가 설정되지 않았습니다.");
            return;
        }

        mains = new MainObject[mainPrefabs.Length];

        // 박 생성
        for (int i = 0; i < boxCount; i++)
        {
            Vector2 pos = fixedMainPositions[i];
            RespawnMain(i, pos);
        }

        // 플랫폼 생성
        for (int i = 0; i < boxCount; i++)
        {
            int prefabIndex = i % platformPrefabs.Length;
            int objectNumber = i + 1;

            Vector2 pos = fixedMainPositions[i];
            Vector3 worldPos = new Vector3(pos.x, planArea.GetY(), pos.y);

            GameObject p = Instantiate(platformPrefabs[prefabIndex], worldPos, Quaternion.identity);
            p.name = $"{platformPrefabs[prefabIndex].name}_Platform_{objectNumber}";

            PlatformObject po = p.GetComponent<PlatformObject>();
            po.mainObjects = mains;
            po.objectNumber = objectNumber;
            po.planArea = planArea;
            po.speed = platformSpeed;
            po.sceneSetup = this;
        }

        // 추가 라운드 처리
        for (int i = boxCount; i < rounds; i++)
        {
            int prefabIndex = i % platformPrefabs.Length;
            int objectNumber = prefabIndex + 1;

            Vector2 randomPos = new Vector2(
                Random.Range(-planArea.size.x / 2f, planArea.size.x / 2f),
                Random.Range(-planArea.size.y / 2f, planArea.size.y / 2f)
            );

            Vector3 worldPos = planArea.transform.position + new Vector3(randomPos.x, 0f, randomPos.y);

            GameObject p = Instantiate(platformPrefabs[prefabIndex], worldPos, Quaternion.identity);
            p.name = $"{platformPrefabs[prefabIndex].name}_Platform_{i + 1}";

            PlatformObject po = p.GetComponent<PlatformObject>();
            po.mainObjects = mains;
            po.objectNumber = objectNumber;
            po.planArea = planArea;
            po.speed = platformSpeed;
            po.sceneSetup = this;
        }
    }

    public void RespawnMain(int index, Vector2 positionXZ)
    {
        if (index < 0 || index >= mainPrefabs.Length) return;

        if (mains[index] != null)
        {
            Destroy(mains[index].gameObject);
            mains[index] = null;
        }

        Vector3 spawnPos = new Vector3(positionXZ.x, planArea.GetY(), positionXZ.y);

        GameObject m = Instantiate(mainPrefabs[index], spawnPos, Quaternion.identity);
        m.name = $"{mainPrefabs[index].name}_Main_{index + 1}";
        m.SetActive(true);

        MainObject mo = m.GetComponent<MainObject>();
        mo.objectNumber = index + 1;
        mains[index] = mo;

        SphereCollider col = m.GetComponent<SphereCollider>();
        if (!col) col = m.AddComponent<SphereCollider>();
        col.radius = mainSphereRadius;
        col.center = Vector3.zero;

        m.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        Debug.Log($"RespawnMain: 박 {index + 1} 생성 (위치: {spawnPos}, Collider 반지름 {mainSphereRadius})");
    }

public void OnBoxRemoved(Vector2 platformPosXZ, int idx)
{
    remainingRounds--;
    UpdateRoundsUI();

    if (remainingRounds <= 0)
    {
        // 성공 처리
        SimpleFillTimer timer = FindObjectOfType<SimpleFillTimer>();
        if (timer != null)
            timer.OnSuccess();

        if (finishPrefab != null)
        {
            // 초기 박 위치를 사용해 Finish 박 생성
            Vector2 finishPosXZ = fixedMainPositions != null && fixedMainPositions.Length > 0
                ? fixedMainPositions[0]
                : Vector2.zero;

            Vector3 finishPos = new Vector3(finishPosXZ.x, planArea.GetY(), finishPosXZ.y);

            GameObject finishBox = Instantiate(finishPrefab, finishPos, Quaternion.identity);
            finishBox.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

            SphereCollider col = finishBox.GetComponent<SphereCollider>();
            if (!col) col = finishBox.AddComponent<SphereCollider>();
            col.radius = mainSphereRadius;
            col.center = Vector3.zero;

            Rigidbody rb = finishBox.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            }

            if (audioSource != null && finishBoxSfx != null)
                audioSource.PlayOneShot(finishBoxSfx, finishBoxVolume);

            Debug.Log($"🎯 Finish 박 생성 위치: {finishPos}");

            // 🕒 N초 후 자동 삭제
            Destroy(finishBox, finishBoxLifetime);
        }
    }
    else
    {
        RespawnMainRandom(platformPosXZ);
    }
}

private void UpdateRoundsUI()
{
    if (roundsText != null)
    {
        int currentRound = rounds - remainingRounds + 1;
        currentRound = Mathf.Clamp(currentRound, 1, rounds);
        roundsText.text = $"{currentRound} / {rounds}";
    }
}


    public void RespawnMainRandom(Vector2 clickedPlatformPosXZ)
    {
        if (mains == null || mains.Length == 0) return;

        PlatformObject[] allPlatforms = FindObjectsOfType<PlatformObject>();
        List<PlatformObject> missingMainsPlatforms = new List<PlatformObject>();

        for (int i = 0; i < mains.Length; i++)
        {
            if (mains[i] == null)
            {
                var platforms = allPlatforms.Where(p => p.objectNumber == i + 1).ToList();
                missingMainsPlatforms.AddRange(platforms);
            }
        }

        if (missingMainsPlatforms.Count == 0) return;

        float maxDist = missingMainsPlatforms.Max(p => Vector2.Distance(
            new Vector2(p.transform.position.x, p.transform.position.z),
            clickedPlatformPosXZ));

        var farthestPlatforms = missingMainsPlatforms
            .Where(p => Mathf.Approximately(Vector2.Distance(
                new Vector2(p.transform.position.x, p.transform.position.z),
                clickedPlatformPosXZ), maxDist))
            .ToList();

        PlatformObject chosenPlatform = farthestPlatforms[Random.Range(0, farthestPlatforms.Count)];
        int chosenIndex = chosenPlatform.objectNumber - 1;

        Vector2 spawnPosXZ = fixedMainPositions != null && fixedMainPositions.Length > 0
            ? fixedMainPositions[0]
            : (Vector2)planArea.transform.position;

        RespawnMain(chosenIndex, spawnPosXZ);
        Debug.Log($"RespawnMainRandom: 박 {chosenIndex + 1} 재생성 (위치 고정: {spawnPosXZ})");
    }

    public void PlaySfx(bool correctStep)
    {
        if (audioSource == null) return;

        if (correctStep && stepCorrectSfx != null)
            audioSource.PlayOneShot(stepCorrectSfx, stepCorrectVolume);
        else if (!correctStep && stepWrongSfx != null)
            audioSource.PlayOneShot(stepWrongSfx, stepWrongVolume);
    }

    void OnValidate()
    {
        if (mains == null) return;

        for (int i = 0; i < mains.Length; i++)
        {
            if (mains[i] == null) continue;
            SphereCollider col = mains[i].GetComponent<SphereCollider>();
            if (col != null)
                col.radius = mainSphereRadius;
        }
    }
}
