using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightTrailPlayerButton : MonoBehaviour
{
    [SerializeField] private LightTrailGeneration playerLT;
    [SerializeField] private Image iconImage;
    [SerializeField] private Image buttonImage;
    [SerializeField] private Color offColor;

    private Color onColor = Color.white;
    private void Start()
    {
        GameEvents.Instance.OnEvent += OnEvent;
    }

    private void OnEvent(GameEvents.EventData data)
    {
        if(data.eventName == "LightTrailChargeState")
        {
            if(data.value == 0)
            {
                TurnOffButton();
            }
            else
            {
                TurnOnButton();
            }
        }
    }
    private void Update()
    {
        if(playerLT != null)
        {
            buttonImage.fillAmount = playerLT.TrailCharge;
        }
    }
    private void DoColorAllImages(Image image, Color color)
    {
            image.DOColor(color, .5f);
    }
    private void TurnOffButton()
    {
        DoColorAllImages(iconImage, offColor);
    }
    private void TurnOnButton()
    {
        DoColorAllImages(iconImage, onColor);
    }
}
