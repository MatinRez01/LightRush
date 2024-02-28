using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    public AudioClip noTrailCharge;


    private AudioSource audioSource;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        GameEvents.Instance.OnEvent += OnEvent;
    }

    private void OnEvent(GameEvents.EventData data)
    {
        if(data.eventName == "LightTrailChargeState")
        {
            if(data.value == 0)
            {
                PlayNoChargeTrailSFX();
            }
        }
    }

    public void PlayNoChargeTrailSFX()
    {
        PlaySFX(noTrailCharge);
    }
    private void PlaySFX(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }
}
