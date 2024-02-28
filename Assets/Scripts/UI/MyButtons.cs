using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventTrigger))]
public class MyButtons : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool pressed;
    [SerializeField] private UnityEvent onPressedDown;
    [SerializeField] private UnityEvent onPressedUp;

    public void OnPointerDown(PointerEventData eventData)
    {
        pressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pressed = false;
        onPressedUp.Invoke();
    }

    private void Update()
    {
        if (pressed)
        {
            onPressedDown.Invoke();
        }
    }

    public bool Pressed => pressed;
}
