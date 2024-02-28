using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrollFx : MonoBehaviour
{
    [SerializeField]
    private Vector2 uvScroll;
    Material mat;
    private void OnEnable()
    {
        mat = GetComponent<MeshRenderer>().material;
    }
    private void Update()
    {
        mat.mainTextureOffset += uvScroll * Time.deltaTime;
    }
}
