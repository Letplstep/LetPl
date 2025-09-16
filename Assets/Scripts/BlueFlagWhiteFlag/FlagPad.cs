using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TreeEditor.TreeEditorHelper;

// ���� ����
public enum FlagType
{
    BlueUp,    // û�� �÷�
    BlueDown,  // û�� ����
    WhiteUp,   // ��� �÷�
    WhiteDown  // ��� ����
}


public class FlagPad : MonoBehaviour
{
    [Header("���� Ÿ��")]
    public FlagType flagType;  // Inspector���� ����

    [Header("TouchPoint 2�� = ��� 1��")]
    public int touchpointsPerPerson = 2;
    private int touchPoints = 0;

    /// ���� '��� ��' (TouchPoint�� touchpointsPerPerson�� ���� ��)
    public int Persons
    {
        get
        {
            if (touchpointsPerPerson <= 0) return 0;
            return touchPoints / touchpointsPerPerson;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TouchPoint"))
        {
            touchPoints++; // ���� ����
            ForceNotify();
            //int persons = (touchpointsPerPerson > 0) ? (touchPoints / touchpointsPerPerson) : 0;

            //Debug.Log($"[Pad Exit ] {flagType}  TP:{touchPoints}  Persons:{persons}");

            //FlagGameManager.Instance?.OnPadPeopleChanged(flagType, persons);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("TouchPoint")) return;

        touchPoints = Mathf.Max(0, touchPoints - 1); // ���� ����
        ForceNotify();
    }

    public void ForceNotify()
    {
        FlagGameManager.Instance?.OnPadPeopleChanged(flagType, Persons);
    }
} // end class
