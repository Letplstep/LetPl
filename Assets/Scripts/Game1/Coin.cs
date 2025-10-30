using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public TeamColor coinTeam;
    public AudioClip collectSound; // 코인 먹는 소리
    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    void OnTriggerEnter(Collider other)
    {
        XRGameManager.Instance.ExpandBase(coinTeam);

        if (collectSound != null)
            audioSource.PlayOneShot(collectSound);

        Destroy(gameObject, 0.2f); // 사운드 재생 후 약간 지연 삭제
        Debug.Log($"{coinTeam} 코인 먹힘 → {coinTeam} 기지 확장!");
    }
}