using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FlagType
{
    BlueUp, BlueDown, WhiteUp, WhiteDown
}

//public class FlagPad : MonoBehaviour
//{
//    [Header("발판 타입")]
//    public FlagType flagType;

//    private HashSet<Collider> currentTouchPoints = new HashSet<Collider>();
//    private int lastNotifiedPersons = -1;

//    public int Persons => currentTouchPoints.Count;

//    private void OnTriggerEnter(Collider other)
//    {
//        if (!other.CompareTag("TouchPoint")) return;

//        if (currentTouchPoints.Add(other))
//        {
//            CheckAndNotify();
//        }
//    }

//    private void OnTriggerExit(Collider other)
//    {
//        if (!other.CompareTag("TouchPoint")) return;

//        if (currentTouchPoints.Remove(other))
//        {
//            CheckAndNotify();
//        }
//    }

//    private void CheckAndNotify()
//    {
//        int currentPersons = Persons;

//        if (currentPersons == lastNotifiedPersons) return;

//        lastNotifiedPersons = currentPersons;

//        if (FlagGameManager.Instance != null)
//        {
//            FlagGameManager.Instance.OnPadPeopleChanged(flagType, currentPersons);
//        }

//        Debug.Log($"[{flagType}] 사람 수 변경: {currentPersons}명");
//    }

//    public void ResetPad()
//    {
//        CountCurrentTouchPoints();
//        lastNotifiedPersons = -1;
//        Debug.Log($"[{flagType}] 리셋 후: {Persons}명");
//    }

//    private void CountCurrentTouchPoints()
//    {
//        currentTouchPoints.Clear();

//        Collider triggerCollider = GetComponent<Collider>();
//        if (triggerCollider == null) return;

//        Collider[] overlapping = Physics.OverlapBox(
//            triggerCollider.bounds.center,
//            triggerCollider.bounds.extents,
//            triggerCollider.transform.rotation
//        );

//        foreach (var col in overlapping)
//        {
//            if (col.CompareTag("TouchPoint"))
//            {
//                currentTouchPoints.Add(col);
//            }
//        }
//    }

//    public void SyncToManager()
//    {
//        CountCurrentTouchPoints();

//        if (FlagGameManager.Instance != null)
//        {
//            FlagGameManager.Instance.SyncPadState(flagType, Persons);
//            Debug.Log($"[{flagType}] Sync: {Persons}명");
//        }
//    }
//}

public class FlagPad : MonoBehaviour
{
    [Header("발판 타입")]
    public FlagType flagType;

    [Header("TouchPoint 2개 = 사람 1명")]
    public int touchpointsPerPerson = 1;

    private int touchPoints = 0;
    private int lastNotifiedPersons = -1;
    private List<Collider> currentTouchPoints = new List<Collider>();

    public int Persons => touchpointsPerPerson > 0 ? (touchPoints / touchpointsPerPerson) : 0;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("TouchPoint")) return;

        if (!currentTouchPoints.Contains(other))
        {
            currentTouchPoints.Add(other);
            touchPoints++;
            CheckAndNotify();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("TouchPoint")) return;

        if (currentTouchPoints.Contains(other))
        {
            currentTouchPoints.Remove(other);
            touchPoints = Mathf.Max(0, touchPoints - 1);
            CheckAndNotify();
        }
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
        CountCurrentTouchPoints();
        lastNotifiedPersons = -1;
        Debug.Log($"[{flagType}] 리셋 후 TouchPoint: {touchPoints}명");
    }

    private void CountCurrentTouchPoints()
    {
        currentTouchPoints.Clear();
        touchPoints = 0;

        Collider triggerCollider = GetComponent<Collider>();
        if (triggerCollider == null) return;

        Collider[] overlapping = Physics.OverlapBox(
            triggerCollider.bounds.center,
            triggerCollider.bounds.extents,
            triggerCollider.transform.rotation
        );

        foreach (var col in overlapping)
        {
            if (col.CompareTag("TouchPoint"))
            {
                currentTouchPoints.Add(col);
                touchPoints++;
            }
        }
    }

    public void SyncToManager()
    {
        CountCurrentTouchPoints();

        if (FlagGameManager.Instance != null)
        {
            FlagGameManager.Instance.SyncPadState(flagType, Persons);
            Debug.Log($"[{flagType}] Sync: {Persons}명 (TP: {touchPoints})");
        }
    }
}