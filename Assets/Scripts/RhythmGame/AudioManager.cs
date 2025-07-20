using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

// ÀÏ´Ü ¾À¸¶´Ù ÆÄ±«µÇ´Â ½Ì±ÛÅæ..
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("Audio Clips")]
    public AudioClip mainBGM;
    public AudioClip createSFX;
    public AudioClip hitSFX;

    [Header("Mixer")]
    public AudioMixer audioMixer;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // DontDestroyOnLoad
    }

    public void PlayCreateSFX()
    {
        sfxSource.PlayOneShot(createSFX);
    }

    public void PlayHitSFX()
    {
        sfxSource.PlayOneShot(hitSFX);
    }

} // end class
