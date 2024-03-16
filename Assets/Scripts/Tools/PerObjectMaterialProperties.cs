using System;
using UnityEngine;

[DisallowMultipleComponent]
public class PerObjectMaterialProperties : MonoBehaviour
{
    static MaterialPropertyBlock block;
    [SerializeField] private ColorProperty[] properties = null;
    void OnValidate()
    {
        if (block == null)
        {
            block = new MaterialPropertyBlock();
        }
        foreach (var property in properties)
        {
            block.SetColor(property.GetColorPropertyID(), property.GetColorValue());
        }
        GetComponent<Renderer>().SetPropertyBlock(block);
    }
    void Awake()
    {
       OnValidate();
    }
}
[Serializable]
public class ColorProperty
{
    public string colorProperty;
    private int colorPropertyID;
    [ColorUsage(true, true)]
    public Color[] colorValue = null;
    public int GetColorPropertyID()
    {
        colorPropertyID = Shader.PropertyToID(colorProperty);
        return colorPropertyID;
    }
    public Color GetColorValue()
    {
        int r = UnityEngine.Random.Range(0, colorValue.Length);
        return colorValue[r];
    }
}