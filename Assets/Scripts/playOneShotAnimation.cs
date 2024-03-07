using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Animation))]
public class playOneShotAnimation : MonoBehaviour
{
    Animation animationComponent;
    [SerializeField] AnimationClip[] clips;
    public Action OnAnimationDone;
    private void OnEnable()
    {
        Setup();
    }
    private void Setup()
    {
        animationComponent = GetComponent<Animation>();
        animationComponent.enabled = true;
    }
    public void PlayAnimation()
    {
        try
        {
            animationComponent.Play();
            Timer.Register(clips[0].length, () =>
            {
                OnAnimationDone?.Invoke();
            });
        }
        catch
        {
            Setup();
            PlayAnimation();
        }

    }
    public void PlayAnimation(string animationName)
    {
        animationComponent.enabled = true;
        animationComponent?.Play(animationName);
        Timer.Register(animationComponent.clip.length, () =>
        {
            OnAnimationDone?.Invoke();
        });
    }

    public void PlayAnimationsAndDisable()
    {
        StartCoroutine(_PlayAllAnimations());

    }
    private void PlayAnimationWithoutCallback(string animationName)
    {
        animationComponent.enabled = true;
        animationComponent?.Play(animationName);
    }
    private IEnumerator _PlayAllAnimations()
    {
        for (int i = 0; i < clips.Length; i++)
        {
            PlayAnimationWithoutCallback(clips[i].name);
            yield return new WaitForSeconds(clips[i].length);
        }

        yield return null;

    }
    public void Deactive()
    {
        gameObject.SetActive(false);
    }

}
