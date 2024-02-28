using DG.Tweening;
using TMPro;
using Tools;
using UnityEngine;
using UnityEngine.UI;

public class AlphaFlicker : MonoBehaviour
{
    [SerializeField] private Component component;
    [MinMaxSlider(0f, 1f)] public Vector2 randomAmplitudeRange;
    [MinMaxSlider(0f, 50f)] public Vector2Int seqmentsAmount;
    [SerializeField] private string targetProperty;
    [SerializeField] private float duration;
    [SerializeField] Material mat;
    private float _value;
    private Color _color;
    enum Component
    {
        Text,
        Image
    }
    private float value
    {
        get
        {
            return _value;
        }
        set
        {
            _value = value;
            _color.a = _value;
            if (component == Component.Text)
            {
                text.color = _color;
            }else if(component == Component.Image)
            {
                image.color = _color;
            }
        }
    }
    private TextMeshProUGUI text;
    private Image image;
    private void OnEnable()
    {
        if(component == Component.Text)
        {
            text = GetComponent<TextMeshProUGUI>();
        }else if(component == Component.Image) 
        { 
            image = GetComponent<Image>();
        }
        StartFlickering();
    }
    private void StartFlickering()
    {
        if(component == Component.Text)
        {
            _color = text.color;
        }else if(component == Component.Image)
        {
            _color = image.color;
        }
        var randomSegment = (int)UnityEngine.Random.Range(seqmentsAmount.x, seqmentsAmount.y);
        var randomNumber = new float[randomSegment];
        DG.Tweening.Sequence sequence = DOTween.Sequence();
        sequence.SetLoops(10, LoopType.Yoyo);
        float eachSegmentDuration = duration / randomSegment;
        for (int i = 0; i < randomNumber.Length; i++)
        {
            randomNumber[i] = UnityEngine.Random.Range(randomAmplitudeRange.x, randomAmplitudeRange.y);
            sequence.Append(DOTween.To(() => value, x => value = x, randomNumber[i], eachSegmentDuration));
        }
        sequence.Play();
    }
}
