using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;
using Tools;
using Random = System.Random;
using Vector2 = UnityEngine.Vector2;

public class PlayInvincibleAnimation : MonoBehaviour
{
    GameObject _activeChild;
    [SerializeField] private AnimationCurve _animationCurve;
    [MinMaxSlider(0f,1f)] public Vector2 randomAmplitudeRange;
    [MinMaxSlider(0f,20f)] public Vector2Int seqmentsAmount;

    private float _value;
    private static readonly int rimLightingEnabled = Shader.PropertyToID("_HollowOn");


    private float Value
    {
        get
        {
            return _value;
        }
        set
        {
            _value = value;
            foreach (var material in mat)
            {
                if (material.HasFloat(rimLightingEnabled))
                {
                    material.SetFloat(rimLightingEnabled, _value);
                }
            }
            
        }
    }
    private Sequence sequence;
    Material[] mat;
    float[] defaultRimVals;
    public void Setup()
    {
        mat = activeChild.GetComponent<MeshRenderer>().materials;
        defaultRimVals = new float[mat.Length];
        for (int i = 0; i < mat.Length; i++)
        {
            if (mat[i].HasFloat(rimLightingEnabled))
            {

                //defaultRimVals[i] = mat[i].GetFloat(rimLightingEnabled);
                defaultRimVals[i] = 0;
            }
        }
    }



    public void PlayAnimationOfActiveChild(float duration){
        /*var randomSegment = (int)UnityEngine.Random.Range(seqmentsAmount.x, seqmentsAmount.y);
        var randomNumber = new float[randomSegment];
        sequence = DOTween.Sequence();
         
        float eachSegmentDuration = duration/ randomSegment;
        for (int i = 0; i < randomNumber.Length; i++)
        {
            randomNumber[i] = UnityEngine.Random.Range(randomAmplitudeRange.x, randomAmplitudeRange.y);
            sequence.Append(DOTween.To(() => value, x => value = x, randomNumber[i], eachSegmentDuration));
        }
        
        sequence.Play();*/
        StartCoroutine(_Animation(duration));
    }
    private IEnumerator _Animation(float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration; // Normalize time between 0 and 1
            float value = _animationCurve.Evaluate(t); // Evaluate the curve at time t
            // Do something with the evaluated value, for example move an object
            Value = value;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Value = 0;
        // Ensure final value is exactly as evaluated at t=1
       // transform.position = new Vector3(curve.Evaluate(1.0f), 0f, 0f);
    }
    
    GameObject activeChild
    {
        get
        {
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                if (gameObject.transform.GetChild(i).gameObject.activeSelf == true)
                {
                    _activeChild = gameObject.transform.GetChild(i).gameObject;
                    return _activeChild;
                }
            }
            Debug.Log("No Active Child");
            return null;
        }
    }
}
