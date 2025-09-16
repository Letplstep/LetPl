using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FlagPatternType
{
    Single, Total
}


[CreateAssetMenu(menuName = "Flag/FlagPatternType")]
public class FlagPatternData : ScriptableObject
{
    [Header("������")]
    public GameObject padPrefab;
    [Header("��ƮŸ��")]
    public FlagPatternType flagPatterType = FlagPatternType.Single;
    [Header("��������Ʈ �̹���")]
    public Sprite sprite;             // ���� ��������Ʈ

    [System.Serializable]
    public class Requirement
    {
        public FlagType flag;   // � ����
        public int persons = 1; // �� ��
    }

    [Header("�� ������ �䱸�ϴ� ���� ���ǵ�")]
    public Requirement[] requirements;
}
