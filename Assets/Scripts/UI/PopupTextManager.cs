using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupTextManager : MonoBehaviour
{
    [SerializeField] public GlobalGameItemsData.Item item;

    Transform parent;
    public PopupTextManager(Transform parent)
    {
        this.parent = parent;
    }
    public void SpawnText(string text)
    {
        GameObject obj = Spawner.Instance.SpawnUI(item.ToString(), parent);
        PopupText popupText = obj.transform.GetChild(0).GetComponent<PopupText>();
        popupText.SetText(text);
        obj.SetActive(true);
    }
    public void SpawnText(string text, Transform parent)
    {
        GameObject obj = Spawner.Instance.SpawnUI(item.ToString(), parent);
        PopupText popupText = obj.GetComponent<PopupText>();
        popupText.SetText(text);
        obj.SetActive(true);
    }
}
