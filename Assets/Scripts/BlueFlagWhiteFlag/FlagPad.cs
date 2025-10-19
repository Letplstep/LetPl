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
    private int lastNotifiedPersons = -1;

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

    private void CheckAndNotify()
    {
        int currentPersons = Persons;

        if (currentPersons == lastNotifiedPersons) return;

        lastNotifiedPersons = currentPersons;

        if (FlagGameManager.Instance != null)
        {
            FlagGameManager.Instance.OnPadPeopleChanged(flagType, currentPersons);
        }

        Debug.Log($"[{flagType}] 사람 수 변경: {currentPersons}명 (TP: {touchPoints})");
    }

    public void ResetPad()
    {
        touchPoints = 0;
        lastNotifiedPersons = -1;
    }

    public void SyncToManager()
    {
        if (FlagGameManager.Instance != null)
        {
            FlagGameManager.Instance.SyncPadState(flagType, Persons);
            Debug.Log($"[{flagType}] Sync: {Persons}명");
        }
    }
}