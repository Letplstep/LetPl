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
    [Header("프리팹")]
    public GameObject padPrefab;
    [Header("노트타입")]
    public FlagPatternType flagPatterType = FlagPatternType.Single;
    [Header("스프라이트 이미지")]
    public Sprite sprite;             // 발판 스프라이트

    [System.Serializable]
    public class Requirement
    {
        public FlagType flag;   // 어떤 발판
        public int persons = 1; // 몇 명
    }

    [Header("이 패턴이 요구하는 발판 조건들")]
    public Requirement[] requirements;
}
