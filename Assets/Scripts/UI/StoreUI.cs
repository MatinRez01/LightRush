using BestHTTP.SecureProtocol.Org.BouncyCastle.Asn1.X500;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Tools;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class StoreUI : MonoBehaviour
{
    [SerializeField] private GameObject lockImage;
    [SerializeField] private GameObject UnlockItemsPanel;
    [SerializeField] private Image unlockItemImage;
    [MinMaxSlider(0f, 10f)] public Vector2Int seqmentsAmount;
    [SerializeField] private float unlockSequenceDuration;
    DG.Tweening.Sequence sequence;
    [SerializeField] private Color hoverColor;
    [SerializeField] private Color unhoverColor;
    public void LockScreen()
    {
        lockImage.SetActive(true);
    }
    public void UnlockAnItem(List<StoreButton> items, StoreButton unlockItem, Action callback)
    {
        int randomSegment = (int)UnityEngine.Random.Range(seqmentsAmount.x, seqmentsAmount.y);
        sequence = DOTween.Sequence();
        float eachSegmentDuration = unlockSequenceDuration / randomSegment;
        for (int i = 0; i < randomSegment; i++)
        {
            if (i == randomSegment - 1)
            {
                sequence.Append(unlockItem.lightImage.DOFade(1, eachSegmentDuration / 2));
            }
            else
            {
                int rnd = UnityEngine.Random.Range(0, items.Count);
                sequence.Append(items[rnd].lightImage.DOFade(1, eachSegmentDuration / 2));
                sequence.Append(items[rnd].lightImage.DOFade(0, eachSegmentDuration / 2));
            }

        }
        sequence.Play();
        sequence.onComplete += () => 
        {
            OnAnimationComplete(unlockItem.itemImage);
            unlockItem.Select();
            callback.Invoke();
        };
    }
    private void OnAnimationComplete(Sprite sprite)
    {
        UnlockItemsPanel.SetActive(true);
        unlockItemImage.gameObject.SetActive(true);
        unlockItemImage.sprite = sprite;
    }
}
