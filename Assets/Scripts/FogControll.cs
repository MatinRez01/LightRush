using AtmosphericHeightFog;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.StickyNote;

public class FogControll : MonoBehaviour
{
    [SerializeField] float targetValue;
    HeightFogGlobal fog;
    [SerializeField] ParticleSystem fogParticle;
    private void OnEnable()
    {
        fog = GetComponent<HeightFogGlobal>();
        GameEvents.Instance.OnEvent += OnGameStart;
    }

    private void OnGameStart(GameEvents.EventData data)
    {
        if(data.eventName == "GameStart")
        {
            DoTweening();
        }
    }
    private void DoTweening()
    {
        DOTween.To(() => fog.fogIntensity, x => fog.fogIntensity = x, targetValue, .5f);
        DOTween.To(() => fogParticle.emissionRate, x => fogParticle.emissionRate = x, 0, 3f);
    }
}
