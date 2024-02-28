using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UvScroll : MonoBehaviour
{
    [SerializeField] public float scrollSpeed;
    [SerializeField] public float maxScrollSpeed;
    Material mat;
    public Material Material => mat;

    private void Awake()
    {
        mat = GetComponent<MeshRenderer>().material;
    }
}
