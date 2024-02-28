using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class OpenURL : MonoBehaviour
{
    [SerializeField] private string url;
    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => Application.OpenURL(url));
    }
}