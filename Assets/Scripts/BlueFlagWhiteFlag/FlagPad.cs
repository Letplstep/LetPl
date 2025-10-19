using System.Collections;
using UnityEngine;

public enum FlagType
{
    BlueUp, BlueDown, WhiteUp, WhiteDown
}

public class FlagPad : MonoBehaviour
{
    [Header("발판 타입")]
    public FlagType flagType;

    [Header("TouchPoint 2개 = 사람 1명")]
    public int touchpointsPerPerson = 2;

    private int touchPoints = 0;
    private int lastNotifiedPersons = -1; // 마지막으로 통보한 사람 수

    public int Persons => touchpointsPerPerson > 0 ? (touchPoints / touchpointsPerPerson) : 0;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("TouchPoint")) return;

        touchPoints++;
        CheckAndNotify();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("TouchPoint")) return;

        touchPoints = Mathf.Max(0, touchPoints - 1);
        CheckAndNotify();
    }

    /// <summary>
    /// 사람 수가 실제로 변경되었을 때만 통보
    /// </summary>
    private void CheckAndNotify()
    {
        int currentPersons = Persons;

        // 사람 수가 변하지 않았으면 통보 안함
        if (currentPersons == lastNotifiedPersons) return;

        lastNotifiedPersons = currentPersons;
        FlagGameManager.Instance?.OnPadPeopleChanged(flagType, currentPersons);

        Debug.Log($"[{flagType}] 사람 수 변경: {currentPersons}명 (TP: {touchPoints})");
    }

    public void ForceNotify()
    {
        lastNotifiedPersons = -1;  // 리셋
        CheckAndNotify();          // 강제 통보
    }

    /// <summary>
    /// 게임 시작 시 초기화
    /// </summary>
    public void ResetPad()
    {
        touchPoints = 0;
        lastNotifiedPersons = -1;
    }

    public void SyncToManager()
    {
        FlagGameManager.Instance.SyncPadState(flagType, Persons);
    }
}