using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorManager : MonoBehaviour
{
    public SensorEnum sensorEnum;

    private OSCManager m_senserData;
    private SensorDataFormat SensorData;
    List<Vector3> vector3 = new List<Vector3>();

    [SerializeField]
    RectTransform SensorPosi;//������ ���

    void Start()
    {
        m_senserData = GameObject.Find("OSCManager").GetComponent<OSCManager>();
        StartCoroutine(GetSendsorData());
    }

    IEnumerator GetSendsorData()
    {
        while (true)
        {
            yield return new WaitUntil(() => SensorActiveState.instance.SensorState[(int)sensorEnum]); //������ ����ɶ����� ���
            yield return new WaitForFixedUpdate();
            SensorData = m_senserData.SensorData[((int)sensorEnum)];//ȣ��� ���� �ο� ������ �ޱ�
            vector3.Clear();

            for (int i = 0; i < SensorData.Position.Count; i++)//ȣ��� ���� �ο� �����͸� ���������� ����� �� �ְ� ����
            {
                vector3.Add(new Vector3(scale(-SensorData.RectSize.x / 2, SensorData.RectSize.x / 2, SensorPosi.position.x - SensorPosi.rect.width / 2, SensorPosi.position.x + SensorPosi.rect.width / 2, SensorData.Position[i].x),
                                        scale(-SensorData.RectSize.y / 2, SensorData.RectSize.y / 2, SensorPosi.position.y - SensorPosi.rect.height / 2, SensorPosi.position.y + SensorPosi.rect.height / 2, SensorData.Position[i].y),
                                        0));
            }
        }
    }

    //�ܺο��� vector3�� �ޱ�
    public List<Vector3> getSensorVector()
    {
        return vector3;
    }

    //ȣ��� �޴������� ���� ��ġ �����͸� ������ ��ġ�� ����
    private float scale(float OldMin, float OldMax, float NewMin, float NewMax, float OldValue)
    {
        float OldRange = (OldMax - OldMin);
        float NewRange = (NewMax - NewMin);
        float NewValue = (((OldValue - OldMin) * NewRange) / OldRange) + NewMin;

        return (NewValue);
    }
}
