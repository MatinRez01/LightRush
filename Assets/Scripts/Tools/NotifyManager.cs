using DG.Tweening;
using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
[RequireComponent(typeof(CanvasGroup))]

[DefaultExecutionOrder(-200)]
public class NotifyManager : Singleton<NotifyManager>
{
    [SerializeField] private TMP_Text text;
    CanvasGroup canvasGroup;
    Sequence sequence;
    void OnEnable()
    {
        Setup();
    }
    void Setup()
    {
        sequence = DOTween.Sequence();
        canvasGroup = GetComponent<CanvasGroup>();
    }
    public void ShowTip(string message)
    {
        ShowTipAsync(message); 
    }
    private async void ShowTipAsync(string message)
    {
        text.text = message;
        sequence.Append(canvasGroup.DOFade(1, 1)).AppendInterval(2).Append(canvasGroup.DOFade(0, 1).OnComplete(() =>
        {
            sequence.Kill();
            sequence = null;
        }));
    }
}
