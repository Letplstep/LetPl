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
    [Header("������")]
    public GameObject padPrefab;
    [Header("��ƮŸ��")]
    public NoteType noteType = NoteType.Single;
    [Header("��������Ʈ �̹���")]
    public Sprite sprite;             // ���� ��������Ʈ
    [Header("���� �� ���� �� �ִ� �ð�, �Ѿ�� �����.")]
    public float lifetime = 3f;
    [Header("����, �ð�")]
    public float excellentThreshold = 0.2f;
    public float goodThreshold = 0.5f;
    public float badThreshold = 1.0f;
    [Header("��Ʈ ����")]
    public Color color = Color.white; // ���п� ����
}




/// 