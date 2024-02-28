using UnityEngine;
using UnityEngine.UI;

public class AlwaysVisible : MonoBehaviour
{
    private ScrollRect scrollRect;
    private RectTransform elementRect;
    private RectTransform scrollRectRect;
    private RectTransform trackRect;

    bool setupComplete;

    void OnEnable()
    {
        if (setupComplete) return;
        scrollRect = FindObjectOfType<ScrollRect>();
        elementRect = this.GetComponent<RectTransform>();
        scrollRectRect = scrollRect.GetComponent<RectTransform>();
        GameObject g = new GameObject(gameObject.name);
        trackRect = g.AddComponent<RectTransform>();
        trackRect.sizeDelta = elementRect.sizeDelta;
        trackRect.gameObject.AddComponent<CanvasRenderer>();
        int i = elementRect.GetSiblingIndex();
        elementRect.SetParent(scrollRectRect);
        trackRect.SetParent(scrollRect.content);
        trackRect.SetSiblingIndex(i);
        setupComplete = true;
    }
    void Update()
    {
        elementRect.sizeDelta = trackRect.sizeDelta;
        Vector3[] corners = new Vector3[4];
        scrollRectRect.GetWorldCorners(corners);
        Vector3 bottomLeft = corners[0];
        Vector3 topRight = corners[2];
        bottomLeft.y += elementRect.rect.height / 2;
        topRight.y -= elementRect.rect.height / 2;

        if (trackRect.position.y < topRight.y && trackRect.position.y > bottomLeft.y)
        {
            // Element is visible, keep it in the list
            FollowTrackRect();

        }
        else
        {
            // Element is invisible, keep it to the corners of the scrollview
            if (trackRect.position.y >= topRight.y)
            {
                Vector3 pos = trackRect.position;
                pos.y = topRight.y;
                elementRect.position = pos;
            }
            else
            {
                Vector3 pos = trackRect.position;
                pos.y = bottomLeft.y;
                elementRect.position = pos;
            }
        }
    }
    void FollowTrackRect()
    {
        elementRect.position = trackRect.position;
    }
}
