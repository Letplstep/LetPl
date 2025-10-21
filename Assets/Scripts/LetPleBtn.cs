using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LetPleBtn : MonoBehaviour
{
    public UnityEvent onTriggerEnter;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("[LetpleButton] 트리거 감지!");
        onTriggerEnter?.Invoke();
    }
}
