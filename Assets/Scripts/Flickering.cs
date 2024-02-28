using AtmosphericHeightFog;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Tools;
using UnityEngine;

public class Flickering : MonoBehaviour
{
    [Header("Flicker")]
    [MinMaxSlider(0f, 1f)] public Vector2 randomAmplitudeRange;
    [MinMaxSlider(0f, 50f)] public Vector2Int seqmentsAmount;
    [SerializeField] private float duration;
    [SerializeField] private string targetProperty;
    [Header("Color Change")]
    [SerializeField] private Color colorStart;
    [SerializeField] private Color colorEnd;
    [SerializeField] private float colorChangeDuration;
    private float _value;
    [SerializeField] Material mat;
    [SerializeField] HeightFogGlobal fog;

    private float value
    {
        get
        {
            return _value;
        }
        set
        {
            _value = value;
            fog.fogIntensity = value;
            // mat.SetFloat(targetProperty, _value);
        }
    }
    public void StartFlickeringAndChangeColor()
    {
        var randomSegment = (int)UnityEngine.Random.Range(seqmentsAmount.x, seqmentsAmount.y);
        var randomNumber = new float[randomSegment];
        Sequence sequence = DOTween.Sequence();
        float eachSegmentDuration = duration / randomSegment;
        sequence.Append(DOTween.To(() => fog.fogColorEnd, x => fog.fogColorEnd = x, Color.black, .2f));
        sequence.Append(DOTween.To(() => fog.fogColorStart, x => fog.fogColorStart = x, Color.black, .2f));
        sequence.AppendInterval(2);
        for (int i = 0; i < randomNumber.Length; i++)
        {
            randomNumber[i] = UnityEngine.Random.Range(randomAmplitudeRange.x, randomAmplitudeRange.y);
            sequence.Append(DOTween.To(() => value, x => value = x, randomNumber[i], eachSegmentDuration));
        }
        sequence.Play();

        Timer.Register(2, () =>
        {
            DOTween.To(() => fog.fogColorEnd, x => fog.fogColorEnd = x, colorEnd, colorChangeDuration);
            DOTween.To(() => fog.fogColorStart, x => fog.fogColorStart = x, colorStart, colorChangeDuration);
        });




    }
}
