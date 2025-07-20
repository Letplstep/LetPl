using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//SpoutSender�� �� ������Ʈ�� �̱������� ����
public class Spout_Singleton : MonoBehaviour
{
    static Spout_Singleton instance;

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
        DontDestroyOnLoad(gameObject); //������� �ı� x
    }
}
