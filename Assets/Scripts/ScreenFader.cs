using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 1;
    Image image;
    Color zeroAlpha = new Color(0, 0, 0, 0);
    void Start()
    {
        image = GetComponentInChildren<Image>();
        DontDestroyOnLoad(gameObject);
        image.color = zeroAlpha;
    }

    public void FadeToBlack()
    {
        image.DOColor(Color.black, fadeDuration).OnComplete(() =>
        {
            FadeFromBlack();
        });
    }
    public void FadeToBlack(float fadeDuration, Action callback)
    {
        image.DOColor(Color.black, fadeDuration).OnComplete(() =>
        {
            if(callback != null)
            {
                callback.Invoke();
            }
            FadeFromBlack();
            
        });
    }
    public void FadeFromBlack()
    {
        image.DOColor(zeroAlpha, fadeDuration);
    }
}
