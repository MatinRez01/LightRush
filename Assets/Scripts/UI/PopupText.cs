using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(TextMeshProUGUI))]
public class PopupText : MonoBehaviour
{
    [SerializeField] float animationDuration = 1f;
    [SerializeField] AnimationCurve fadeCurve;
    [SerializeField] float yOffset = 5f;
    [SerializeField]Image backImage;
    TextMeshProUGUI myText;
    Color col;
    Color imageCol;
    Vector3 initialPos;
    private void OnEnable()
    {
        initialPos = transform.localPosition;
        if (myText == null)
        {
            myText = GetComponent<TextMeshProUGUI>();
        }

        col = myText.color;
        col.a = 1;
        myText.color = col;
        if (backImage != null)
        {
            imageCol = backImage.color;
            imageCol.a = 1;
            backImage.color = imageCol;
            backImage.DOFade(0, animationDuration).SetEase(fadeCurve);
        }
        myText.DOFade(0, animationDuration).SetEase(fadeCurve).OnComplete(() =>
        {
            transform.localPosition = initialPos;
            Spawner.Instance.Despawn(gameObject.transform.parent.gameObject);
        });
        if (backImage != null)
        {
            myText.gameObject.transform.parent.DOLocalMoveY(transform.localPosition.y + yOffset, animationDuration);
        }
        else
        {
            myText.gameObject.transform.DOLocalMoveY(transform.localPosition.y + yOffset, animationDuration);

        }
    }
    public void SetText(string text)
    {
        myText.text = text;
    }
}
