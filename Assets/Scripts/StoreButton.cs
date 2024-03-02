using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreButton : MonoBehaviour
{
    public GameObject lockedImage;
    public GameObject unlockedImage;
    public GameObject GameObject;
    public Image imageObject;
    public Sprite itemImage;
    public bool isTrail;
    public bool unlocked;
     public Image buttonbgImage;
    public Image SelectImage;
     public Image lightImage;
    [SerializeField] public Button button;
    [SerializeField] private Color deselectColor;
    [SerializeField] private int itemIndex;
    public Action<int> selectCallback;
  
    private void Awake()
    {
        buttonbgImage = GetComponent<Image>();
        if (unlocked)
        {
            UnlockItem();
        }
        else
        {
            lockedImage.SetActive(true);
            unlockedImage.SetActive(false);
            TurnOffButton();
        }
        button.onClick.AddListener(OnButtonClick);
    }
    public void UnlockItem()
    {
        lockedImage.SetActive(false);
        unlockedImage.SetActive(true);
        imageObject.sprite = itemImage;
        buttonbgImage.color = Color.white;
        unlocked = true;
    }
    private void OnButtonClick()
    {
         selectCallback.Invoke(itemIndex);
    }
    private void TurnOnButton()
    {
        if(buttonbgImage == null)
        {
            buttonbgImage = GetComponent<Image>();
        }
        buttonbgImage.color = Color.white;
        SelectImage.DOFade(1, .1f);
    }
    private void TurnOffButton()
    {
        buttonbgImage.color = deselectColor;
        SelectImage.DOFade(0, .1f);
    }
    public void Select()
    {
        TurnOnButton();
        lightImage.DOFade(0, 0.1f);
    }
    public void Deselect()
    {
        TurnOffButton();
    }
}