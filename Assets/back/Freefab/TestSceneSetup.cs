using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UI;

public class TestSceneSetup : MonoBehaviour
{
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

    [Header("피니쉬 박 프리팹")]
    public GameObject finishPrefab;

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

    [HideInInspector] public MainObject[] mains;
    [HideInInspector] public int remainingRounds;

    private AudioSource audioSource;

    void Start()
    {
        remainingRounds = rounds;
        UpdateRoundsUI();

        audioSource = gameObject.AddComponent<AudioSource>();
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
        for (int i = 0; i < boxCount; i++)
        {
            Vector2 pos = fixedMainPositions.Length > i ? fixedMainPositions[i] : (Vector2)planArea.transform.position;
            RespawnMain(i, pos);
        }

        for (int i = 0; i < rounds; i++)
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
            po.sceneSetup = this; // 연동
        }
    }

public void RespawnMain(int index, Vector2 positionXZ)
{
    if (index < 0 || index >= mainPrefabs.Length) return;

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
            // ✅ 성공 처리 호출
            SimpleFillTimer timer = FindObjectOfType<SimpleFillTimer>();
            if (timer != null)
            {
                timer.OnSuccess();
            }

            if (finishPrefab != null)
            {
                Vector2 finishPos = fixedMainPositions.Length > idx ? fixedMainPositions[idx] : (Vector2)planArea.transform.position;
                Vector3 worldPos = new Vector3(finishPos.x, 0f, finishPos.y);
                GameObject finishBox = Instantiate(finishPrefab, worldPos, Quaternion.identity);
                finishBox.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

                SphereCollider col = finishBox.GetComponent<SphereCollider>();
                if (!col) col = finishBox.AddComponent<SphereCollider>();
                col.radius = mainSphereRadius;
                col.center = Vector3.zero;

                // 마지막 박 효과음 (볼륨 증폭)
                if (audioSource != null && finishBoxSfx != null)
                    audioSource.PlayOneShot(finishBoxSfx, finishBoxVolume);

                Debug.Log("마지막 발판 → Finish 박 생성 (ZX 평면)");
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
            roundsText.text = remainingRounds.ToString();
    }

public void RespawnMainRandom(Vector2 clickedPlatformPosXZ)
{
    if (mains == null || mains.Length == 0) return;

    PlatformObject[] allPlatforms = FindObjectsOfType<PlatformObject>();
    List<PlatformObject> availablePlatforms = new List<PlatformObject>();

    // 사라진 박만 선택
    for (int i = 0; i < mains.Length; i++)
    {
        if (mains[i] == null)
        {
            var platforms = allPlatforms.Where(p => p.objectNumber == i + 1).ToList();
            availablePlatforms.AddRange(platforms);
        }
    }

    if (availablePlatforms.Count == 0) return;

    // 밟힌 발판에서 가장 먼 발판 찾기
    float maxDist = availablePlatforms.Max(p => Vector2.Distance(
        new Vector2(p.transform.position.x, p.transform.position.z),
        clickedPlatformPosXZ));

    var farthestPlatforms = availablePlatforms
        .Where(p => Mathf.Approximately(Vector2.Distance(
            new Vector2(p.transform.position.x, p.transform.position.z),
            clickedPlatformPosXZ), maxDist))
        .ToList();

    PlatformObject chosenPlatform = farthestPlatforms[Random.Range(0, farthestPlatforms.Count)];
    int chosenIndex = chosenPlatform.objectNumber - 1;

    // ZX 위치는 항상 고정 위치 사용
    Vector2 spawnPosXZ = fixedMainPositions.Length > chosenIndex 
        ? fixedMainPositions[chosenIndex]
        : (Vector2)planArea.transform.position;

    RespawnMain(chosenIndex, spawnPosXZ);

    Debug.Log($"RespawnMainRandom: 박 {chosenIndex + 1} 재생성 (항상 ZX 고정 위치, 밟힌 발판에서 가장 멀리)");
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