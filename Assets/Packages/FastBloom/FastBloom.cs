using UnityEngine;
using UnityEngine.XR;

[ExecuteInEditMode]
public class FastBloom : MonoBehaviour
{
    public bool SetBloomIterations = false;
    [Range(2, 9)]
    public int BloomIterations = 5;
    [Range(0, 1)]
    public float BloomDiffusion = 1f;
    public Color BloomColor = Color.white;
    public float BloomAmount = 1f;
    public float BloomThreshold = 1.0f;
    [Range(0, 1)]
    public float BloomSoftness = 0.0f;

    public Material material = null;

    static readonly int blurAmountString = Shader.PropertyToID("_BlurAmount");
    static readonly int bloomColorString = Shader.PropertyToID("_BloomColor");
    static readonly int blDataString = Shader.PropertyToID("_BloomData");
    static readonly int bloomTexString = Shader.PropertyToID("_BloomTex");
    static readonly string rgbmKeyword = "_USE_RGBM";

    float knee;
    RenderTextureFormat format;
    bool rgbm;
    Vector4 bloomData;
    RenderTextureDescriptor opaqueDesc;
    Vector3 lm = new Vector3(0.2126f, 0.7152f, 0.0722f);

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (BloomDiffusion == 0 && BloomAmount == 0)
        {
            Graphics.Blit(source, destination);
            return;
        }

        var c = BloomColor.linear;
        var l = Vector3.Dot(new Vector3(c.r, c.g, c.b), lm);
        var color  = l > 0f ? c * (1f / l) : Color.white;
        var threshold = Mathf.GammaToLinearSpace(BloomThreshold);
        knee = threshold * BloomSoftness;
        bloomData = new Vector4(threshold, threshold - knee, 2f * knee, 1f / (4f * knee + 0.0001f));
        rgbm = !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.DefaultHDR);
        format = !rgbm ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.ARGB32;

        material.SetFloat(blurAmountString, Mathf.Lerp(0.05f, 0.95f, BloomDiffusion));
        material.SetColor(bloomColorString,  color * BloomAmount);
        material.SetVector(blDataString, bloomData);

        if (rgbm && !material.IsKeywordEnabled(rgbmKeyword))
            material.EnableKeyword(rgbmKeyword);
        else if (!rgbm && material.IsKeywordEnabled(rgbmKeyword))
            material.DisableKeyword(rgbmKeyword);

        opaqueDesc = XRSettings.enabled ? XRSettings.eyeTextureDesc : new RenderTextureDescriptor(Screen.width, Screen.height, format, 0);
        opaqueDesc.autoGenerateMips = false;
        opaqueDesc.useMipMap = false;
        opaqueDesc.msaaSamples = 1;
        var bloomIteration = SetBloomIterations ? BloomIterations : Mathf.Clamp(Mathf.FloorToInt(Mathf.Log(Mathf.Max(opaqueDesc.width >> 1, opaqueDesc.height >> 1), 2f) - 1), 1, 9);

        RenderTexture[] bloomTemp = new RenderTexture[bloomIteration];
        RenderTexture[] bloomTemp1 = new RenderTexture[bloomIteration];

        for (int i = 0; i < bloomIteration; i++)
        {
            opaqueDesc.width = Mathf.Max(1, opaqueDesc.width >> 1);
            opaqueDesc.height = Mathf.Max(1, opaqueDesc.height >> 1);
            bloomTemp[i] = RenderTexture.GetTemporary(opaqueDesc);
            bloomTemp1[i] = RenderTexture.GetTemporary(opaqueDesc);
        }

        Graphics.Blit(source, bloomTemp[0], material, 0);

        for (int i = 0; i < bloomIteration - 1; i++)
            Graphics.Blit(bloomTemp[i], bloomTemp[i + 1], material, 1);

        for (int i = bloomIteration - 2; i >= 0; i--)
        {
            material.SetTexture(bloomTexString, i == bloomIteration - 2 ? bloomTemp[i + 1] : bloomTemp1[i + 1]);
            Graphics.Blit(bloomTemp[i], bloomTemp1[i], material, 2);
        }

        material.SetTexture(bloomTexString, bloomTemp1[0]);
        Graphics.Blit(source, destination, material, 3);

        for (int i = 0; i < bloomIteration; i++)
        {
            RenderTexture.ReleaseTemporary(bloomTemp[i]);
            RenderTexture.ReleaseTemporary(bloomTemp1[i]);
        }
    }
}