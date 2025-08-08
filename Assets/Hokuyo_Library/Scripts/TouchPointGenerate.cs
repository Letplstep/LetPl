using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TouchPointGenerate : MonoBehaviour
{
    public SensorEnum sensorEnum;
    private List<GameObject> TouchPoints = new List<GameObject>();

    [SerializeField] private SensorManager sensorManager;
    [SerializeField] private GameObject TouchPoint;//������ ������

    [SerializeField] private Transform TouchPointBasket;//������ ������ Ʈ������

    // Update is called once per frame
    void Update()
    {
        if (SensorActiveState.instance.SensorState[((int)sensorEnum)])//ȣ��� �޴����� ����Ǿ�����
        {
            if (TouchPoint != null)
            {
                if (sensorManager.getSensorVector().Count > TouchPoints.Count)//������Ʈ Ǯ��
                {
                    for (int i = TouchPoints.Count; i < sensorManager.getSensorVector().Count; i++)
                    {
                        TouchPoints.Add(Instantiate(TouchPoint, TouchPointBasket));
                    }
                }
                else if (sensorManager.getSensorVector().Count < TouchPoints.Count)//������Ʈ Ǯ��
                {
                    for (int i = sensorManager.getSensorVector().Count; i < TouchPoints.Count; i++)
                    {
                        TouchPoints[i].SetActive(false);
                    }
                }

                for (int i = 0; i < sensorManager.getSensorVector().Count; i++)//������Ʈ Ǯ��, ���� ��ġ�� �̵�
                {
                    TouchPoints[i].SetActive(true);
                    TouchPoints[i].transform.localPosition = sensorManager.getSensorVector()[i];
                }
            }
        }
        else//ȣ��� �޴����� ������ �ȵǾ������� ��� ������Ʈ False
        {
            for (int i = 0; i < TouchPoints.Count; i++)
            {
                TouchPoints[i].SetActive(false);
            }
        }
    }
}
