using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartButton : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TouchPoint"))
        {
            RhythmGameManager.Instance.RestartGame();
        }
    }
}
