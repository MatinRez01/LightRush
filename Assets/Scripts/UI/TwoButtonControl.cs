using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TwoButtonControl : MonoBehaviour
{
    [SerializeField] private Image rightClickImage;
    [SerializeField] private Image leftClickImage;
    [SerializeField] private MyButtons rightButton;
    [SerializeField] private MyButtons leftButton;
    private float value;
    public float Value => value;
    private void Update()
    {
        
       /* if (Input.touchCount == 0)
       {
            SetValue(0);
            return;
       }*/
        /*  Touch touch = Input.GetTouch(0);

          float touchPosition = touch.position.x / Screen.width;
          if (touchPosition >= 0.5)
          {
               if (rightClickImage.gameObject.active)
               {
                   rightClickImage.gameObject.SetActive(false);
               }
            //  SetValue(1);
          }
          else
          {
               if (leftClickImage.gameObject.active)
               {
                   leftClickImage.gameObject.SetActive(false);
               }
                 //  SetValue(-1);
          }*/

        if (rightButton.Pressed)
        {
            if (rightClickImage.gameObject.activeSelf)
            {
                rightClickImage.gameObject.SetActive(false);
            }
              SetValue(1);
        }
        else if(leftButton.Pressed)
        {
            if (leftClickImage.gameObject.activeSelf)
            {
                leftClickImage.gameObject.SetActive(false);
            }
             SetValue(-1);
        }
        else
        {
            SetValue(0);
        }
    }
    public void SetValue(float v)
    {
        value = v;
    }
}
