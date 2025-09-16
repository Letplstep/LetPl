using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TreeEditor.TreeEditorHelper;

// 발판 종류
public enum FlagType
{
    BlueUp,    // 청기 올려
    BlueDown,  // 청기 내려
    WhiteUp,   // 백기 올려
    WhiteDown  // 백기 내려
}


public class FlagPad : MonoBehaviour
{
    [Header("발판 타입")]
    public FlagType flagType;  // Inspector에서 선택

    [Header("TouchPoint 2개 = 사람 1명")]
    public int touchpointsPerPerson = 2;
    private int touchPoints = 0;

    /// 현재 '사람 수' (TouchPoint를 touchpointsPerPerson로 나눈 값)
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
            touchPoints++; // 먼저 증가
            ForceNotify();
            //int persons = (touchpointsPerPerson > 0) ? (touchPoints / touchpointsPerPerson) : 0;

            //Debug.Log($"[Pad Exit ] {flagType}  TP:{touchPoints}  Persons:{persons}");

            //FlagGameManager.Instance?.OnPadPeopleChanged(flagType, persons);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("TouchPoint")) return;

        touchPoints = Mathf.Max(0, touchPoints - 1); // 음수 방지
        ForceNotify();
    }

    public void ForceNotify()
    {
        FlagGameManager.Instance?.OnPadPeopleChanged(flagType, Persons);
    }
} // end class
