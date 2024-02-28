using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventsPopupManager : MonoBehaviour
{
    [SerializeField] private Transform scoreTextParent;
    [SerializeField] private Transform coinTextParent;
    PopupTextManager popupTextManager;
    private void Start()
    {
        popupTextManager = new PopupTextManager(coinTextParent);
        popupTextManager.item = GlobalGameItemsData.Item.AddText;
        GameEvents.Instance.OnPropertyChange += OnEvent;
    }
    private void OnEvent(string itemName, int changeAmount)
    {
        string textString = string.Empty;
        Transform whichParent = null;
        if(changeAmount > 0)
        {
            textString = "+" + changeAmount.ToString();
        }
        else
        {
            textString = "-" + Mathf.Abs(changeAmount).ToString();
        }
        switch (itemName)
        {
            case "Coin":
                whichParent = coinTextParent;
                break;
            case "Score":
                whichParent = scoreTextParent;
                break;
        }
        ShowText(textString, whichParent);
    }
    public void ShowText(string text, Transform parent)
    {
        popupTextManager.SpawnText(text, parent);
    }

}
