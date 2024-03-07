using Coffee.UIExtensions;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playHealthAnimation : MonoBehaviour
{
    public AlphaFlicker alphaFlicker;
    private UIShiny uiShiny;
    [SerializeField] private bool shinyAutoStart;
    [SerializeField] private float punchAmount = 1;
    [SerializeField] private int punchVibrato = 8;
    [SerializeField] private float punchDuration = 0.5f;
    [SerializeField] private float fadeDuration = 0.4f;
    Image image;
    Sequence sequence;
    private void OnEnable()
    {
        image = GetComponent<Image>();
        uiShiny = alphaFlicker.gameObject.GetComponent<UIShiny>();
        bool shinyEnabled = shinyAutoStart;
        uiShiny.enabled = shinyEnabled;
    }
    public void PlayAnimationAndDisable()
    {
        uiShiny.enabled = true;
        sequence = DOTween.Sequence();
        sequence.Append(transform.DOPunchScale(new Vector3(punchAmount, punchAmount, punchAmount), punchDuration, punchVibrato));
        sequence.Join(image.DOFade(0, fadeDuration));

        sequence.onComplete += Disable;
    }
    private void Disable()
    {
        gameObject.SetActive(false);
    }
}
