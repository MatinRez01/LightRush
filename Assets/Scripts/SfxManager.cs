using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SfxManager : MonoBehaviour
{
    
    [SerializeField] private AudioClip gameStartSound;
    [SerializeField] private AudioClip gameOverSound;
    [SerializeField] private AudioMixer mixer;
    

    private AudioSource audioSource;
    

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void SoundMute(bool value)
    {
        Debug.Log("Sound is mute:" + value);
        mixer.SetFloat("Volume", value ? -80.0f : 0.00f);
    }

    public void GameStart()
    {
        audioSource.clip = gameStartSound;
        audioSource.Play();
    }

    public void GameEnd()
    {
        audioSource.clip = gameOverSound;
        audioSource.Play();
    }
}
