using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using Sequence = DG.Tweening.Sequence;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] musics;
    [SerializeField] float pitchingAmount;
    [SerializeField] float duration;
    AudioLowPassFilter lowPassFilter;
    AudioSource audioSource;

    float startingValue;
    Sequence pitchingSequnce;
    public void Setup()
    {

        audioSource = GetComponent<AudioSource>();
        AudioClip currentClip = audioSource.clip;
        int rnd = UnityEngine.Random.Range(0,musics.Length);
        while (musics[rnd] == currentClip)
        {
             rnd = UnityEngine.Random.Range(0, musics.Length);
        }
        audioSource.clip = musics[rnd];
        Debug.Log(rnd);
        audioSource.Play();
        lowPassFilter = GetComponent<AudioLowPassFilter>();
        startingValue = lowPassFilter.cutoffFrequency;
        GameEvents.Instance.OnEvent += OnEvent;
        PlayerCar.OnPlayerDamage += DoPitching;
    }
    private void OnDisable()
    {
        PlayerCar.OnPlayerDamage -= DoPitching;
    }

    private void OnEvent(GameEvents.EventData data)
    {
        if (data.eventName == "GameStart")
        {
            DoTweening(0.5f, 22222);
            Timer.Register(2f, () =>
            {
                lowPassFilter.enabled = false;
            });
        }else if(data.eventName == "GameEnd")
        {
            lowPassFilter.enabled=true;
            DoTweening(0.7f, startingValue);
        }
    }
    private void DoTweening(float volume, float cutoffFrequency)
    {
        DOTween.To(() => lowPassFilter.cutoffFrequency, x => lowPassFilter.cutoffFrequency = x, cutoffFrequency, 1.5f);
        DOTween.To(() => audioSource.volume, x => audioSource.volume = x, volume, .5f);

    }
    private void DoPitching()
    {
        pitchingSequnce = DOTween.Sequence();
        pitchingSequnce.Append(DOTween.To(() => audioSource.pitch, x => audioSource.pitch = x, pitchingAmount, duration));
        pitchingSequnce.AppendInterval(1f);
        pitchingSequnce.Append(DOTween.To(() => audioSource.pitch, x => audioSource.pitch = x, 1, .5f));
        Debug.Log("Pitching");
        pitchingSequnce.Play();
    }
}
