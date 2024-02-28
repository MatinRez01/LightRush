using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ErrorHandler : MonoBehaviour 
{
    [SerializeField] private Transform textParent;
    [SerializeField] private float coolDown;
    PopupTextManager popupTextManager;
    private bool canShow = true;
    private static ErrorHandler _instance;
    public static ErrorHandler Instance
    {
        get
        {
            return _instance;
        }
    }
    private void Start()
    {
        _instance = FindAnyObjectByType<ErrorHandler>();
        popupTextManager = new PopupTextManager(textParent);
        popupTextManager.item = GlobalGameItemsData.Item.ErrorText;
    }
    public void ShowError(string error)
    {
        if(canShow)
        {
            popupTextManager.SpawnText(error);
            StartCoroutine(StartCooldown());
        }
    }
    private IEnumerator StartCooldown()
    {
        canShow = false;
        yield return new WaitForSeconds(coolDown);
        canShow = true;
    }
}
