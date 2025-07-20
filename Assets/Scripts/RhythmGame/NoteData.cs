using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NoteType
{
    Single, Sync
}

[CreateAssetMenu(menuName ="Rhythm/NoteData")]
public class NoteData : ScriptableObject
{
    [Header("프리팹")]
    public GameObject padPrefab;
    [Header("노트타입")]
    public NoteType noteType = NoteType.Single;
    [Header("스프라이트 이미지")]
    public Sprite sprite;             // 발판 스프라이트
    [Header("생성 후 밟을 수 있는 시간, 넘어가면 사라짐.")]
    public float lifetime = 3f;
    [Header("판정, 시간")]
    public float excellentThreshold = 0.2f;
    public float goodThreshold = 0.5f;
    public float badThreshold = 1.0f;
    [Header("노트 색상")]
    public Color color = Color.white; // 구분용 색상
}




/// 