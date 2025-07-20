using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorActiveState : MonoBehaviour
{
    public static SensorActiveState instance;

    public bool[] SensorState; //������ ������� ���

    //�̱���
    private void Awake()
    {
        if (instance == null) //�����ϰ� ���� ������
        {
            instance = this; //�ٽ� �ֽ�ȭ
            DontDestroyOnLoad(gameObject); //������� �ı� x
        }
        else
        {
            if (instance != this) //�ߺ����� �����ҽÿ��� �ı�! 
                Destroy(this.gameObject);
        }
    }

    //�ʱ�ȭ
    void Start()
    {
        SensorState = new bool[System.Enum.GetValues(typeof(SensorEnum)).Length];
        for (int i = 0; i < SensorState.Length; i++)
            SensorState[i] = false;
    }
}
