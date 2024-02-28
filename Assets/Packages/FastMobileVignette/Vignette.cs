using UnityEngine;

[ExecuteInEditMode]
public class Vignette : MonoBehaviour
{
    public Material material;

    [Range(0, 1)]
    public float VignetteValue;
        
    private static int vignetteString = Shader.PropertyToID("_Vignette");

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        material.SetFloat(vignetteString, VignetteValue*2.5f);
        Graphics.Blit(source, destination, material, 0);
    }

}
