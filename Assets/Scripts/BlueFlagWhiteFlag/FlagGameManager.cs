using System.Collections;
using System.Collections.Generic;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using static UnityEditor.ShaderData;

public class FlagGameManager : MonoBehaviour
{
    public static FlagGameManager Instance { get; private set; }

    [Header("UI")]
    public Image patternImage;
    public Slider sliderTimer;
    public TextMeshProUGUI textGameStatus;
 
    [Header("���� ����")]
    public float totalPlayTime = 45f;   // ��ü ���� �ð�
    public bool isGameStarted = false;
    public bool isGameCleared = false;
    private bool isGameRunning = false;

    [Header("�غ� �ð�")]
    public float prepareTime = 3f; // ���� ���� �� ��� �ð�
    public float patternShowTime = 3f;  // ���� ǥ�� �ð�
    public float countdownStepTime = 1f;  // 3,2,1 ���� ǥ�� �ð�

    [Header("���� ������ Ǯ")]
    public List<FlagPatternData> patternList = new List<FlagPatternData>();
    private FlagPatternData currentPattern;
    private bool patternCleared = false;   // ���� ������ ���� ó���Ǿ�����                 
    private int[] currentPersons = new int[4]; // ���Ǻ� ���� �ο� ���� (0=BlueUp, 1=BlueDown, 2=WhiteUp, 3=WhiteDown)    
    private int lastPatternIndex = -1; // ���������� ���� ���� �ε��� (���� = -1)

    [Header("ī��Ʈ�ٿ� ��������Ʈ (�ν����Ϳ� ����)")]
    public Sprite sprite3;
    public Sprite sprite2;
    public Sprite sprite1;

    [Header("���� ����")]
    public int correctCount = 0;

    [Header("�е� ������ �ֱ�")]
    [SerializeField] private List<FlagPad> pads = new List<FlagPad>();

    private float gameEndTime;   // ���� ���� �ð�

    void Awake()
    {
        // �̱��� �ν��Ͻ� ����
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // �ߺ� �ν��Ͻ� ����
        }
    }

    private void SetPatternVisible(bool on)
    {
        if (patternImage)
        {
            patternImage.enabled = on;
        }
    }

    private void ShowStatus(string msg, bool on = true)
    {
        if (!textGameStatus) return;
        textGameStatus.text = msg;
        textGameStatus.enabled = on;
    }

    private void Start()
    {
        // ���� �ÿ� ���� �̹����� ����� ���� �ؽ�Ʈ�� ���̵���
        SetPatternVisible(false);
        if (sliderTimer)
        {
            sliderTimer.minValue = 0f;
            sliderTimer.maxValue = 100f;
            sliderTimer.value = 100f; // �ʱⰪ
        }
        StartCoroutine(StartSequence());
    }

    public void StartGame()
    {
        if (isGameRunning) return;

        isGameRunning = true;
        isGameStarted = true;
        isGameCleared = false;

        gameEndTime = Time.time + totalPlayTime;
        StartCoroutine(GameLoop());
    }

    // --- ���� ������: 3,2,1, Game Start! �� ���� ���� ���� ---
    private IEnumerator StartSequence()
    {
        // ī��Ʈ�ٿ� �ؽ�Ʈ
        ShowStatus("3");
        yield return new WaitForSeconds(1f);

        ShowStatus("2");
        yield return new WaitForSeconds(1f);

        ShowStatus("1");
        yield return new WaitForSeconds(1f);

        ShowStatus("Game Start!");
        yield return new WaitForSeconds(1f); // ��¦ �����ֱ�

        // �ؽ�Ʈ ����� ���� �̹��� ���̱� + ���� ����
        ShowStatus("", false);
        SetPatternVisible(true);
        StartGame(); // ���� ���� ���� ����
    }

    private IEnumerator GameLoop()
    {
        float endTime = Time.time + totalPlayTime;

        while (Time.time < endTime)
        {
            // 1) ���� ���� ���� & �̹��� ǥ��
            //FlagPatternData pattern = patternList[Random.Range(0, patternList.Count)];
            //currentPattern = pattern;

            int pickIndex;
            do
            {
                pickIndex = Random.Range(0, patternList.Count);
            } while (pickIndex == lastPatternIndex && patternList.Count > 1);

            FlagPatternData pattern = patternList[pickIndex];
            lastPatternIndex = pickIndex;
            currentPattern = pattern;


            // �� ���� ����: ���� �ʱ�ȭ
            patternCleared = false;
            for (int i = 0; i < 4; i++) currentPersons[i] = 0;

            // UI ����
            if (patternImage != null)
            {
                patternImage.sprite = pattern.sprite;
                patternImage.enabled = (pattern.sprite != null);
            }

            // �� ���� �����Ҷ����� ���� �е����� �ö� ��� ���� �ݿ�
            for (int i = 0; i < pads.Count; i++)
                pads[i]?.ForceNotify();

            //// 2) 3�ʰ� ���� �̹��� �����ֱ�
            //yield return new WaitForSeconds(patternShowTime);

            // --- ���� 3�� ���� �����̴� 3��0 ---
            float timer = patternShowTime;
            while (timer > 0f)
            {
                if (patternCleared) break; // ���� �� ��� ���� ��������

                timer -= Time.deltaTime;
                if (sliderTimer)
                {
                    float ratio = Mathf.Clamp01(timer / patternShowTime); // 1 �� 0
                    sliderTimer.value = ratio * sliderTimer.maxValue;      // 100 �� 0
                }

                yield return null;
            }

            // --- ī��Ʈ�ٿ� (�����̴��� �׻� 100 ����) ---
            if (sliderTimer) sliderTimer.value = sliderTimer.maxValue;

            // ī��Ʈ �ٿ� �̹��� �����ֱ�
            patternImage.sprite = sprite3;
            yield return new WaitForSeconds(1f);

            patternImage.sprite = sprite2;
            yield return new WaitForSeconds(1f);

            patternImage.sprite = sprite1;
            yield return new WaitForSeconds(1f);


        }

        Debug.Log("[Game] ����");

        EndGame();
    }

    // --- ���� ���� ó��: ���� ����� �ؽ�Ʈ�� "���� ����!" ---
    private void EndGame()
    {
        isGameRunning = false;
        isGameCleared = true;

        SetPatternVisible(false);
        ShowStatus("���� ����!", true);

        Debug.Log("[Game] ����");
    }


    // FlagPad�� ȣ��: �ش� ������ ���� �ο� ����
    public void OnPadPeopleChanged(FlagType type, int persons)
    {
        //if (patternCleared) return;
        //if (currentPattern == null) return;

        Debug.Log($"[PadEvent] {type} ���� {persons}�� | ����: {currentPattern.name}");

        // 1) ���� �ο� ������Ʈ
        currentPersons[(int)type] = Mathf.Max(0, persons);

        // �䱸�� ���ǵ鸸 Ȯ��(���ʿ� ������ �Ű� �� ��)
        foreach (var req in currentPattern.requirements)
        {
            int cur = currentPersons[(int)req.flag];
            Debug.Log($"���� ���� : {req.flag}�� �ο� {cur}");
            if (cur < req.persons) return; // ���� �̴� �� ���
        }

        // �̹� ���� ó���� �����̸� ����
        if (patternCleared) return;


        // ������� ���� ��� ���� ����
        Debug.Log("[Pattern OK] ����!");

        // TODO: ���� ó��(����/����Ʈ/���� ���� ��)
        patternCleared = true;
        correctCount += 1;
        Debug.Log($"[Pattern OK] ����! ���� ����: {correctCount}");

        AudioManager.Instance.PlayHitSFX();

    }
} // end class
