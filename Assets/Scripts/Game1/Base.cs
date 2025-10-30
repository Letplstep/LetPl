using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{
    public TeamColor baseTeam;          // ÀÌ ±âÁöÀÇ »ö±ò
    public AudioClip baseHitSound;      // ±âÁö°¡ ¹âÈú ¶§ ³ª´Â ¼Ò¸®
    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        // ´êÀ¸¸é ¹İ´ëÆÀ Á¡¼ö +1
        TeamColor scorer = (baseTeam == TeamColor.Green) ? TeamColor.Pink : TeamColor.Green;
        XRGameManager.Instance.AddScore(scorer, 1);

        if (baseHitSound != null)
            audioSource.PlayOneShot(baseHitSound);

        Debug.Log($"{baseTeam} ±âÁö°¡ ¹âÈû ¡æ {scorer} Á¡¼ö È¹µæ!");
    }
}