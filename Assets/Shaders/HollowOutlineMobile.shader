Shader "Unlit/HollowOutlineMobile"
{
    Properties
    {
        [HDR]_Color("Main Color", Color) = (1, 1, 1, 1)
        _MainTex ("Texture", 2D) = "white" {}
        _HologramTex ("Holgram Texture", 2D) = "white" {}
        _EmissionColor("Emission Color", Color) = (0,0,0,0)
        _EmissionPower("EmissionPower", Range(0,20) ) = 0
        _RimColor("Rim Color", Color) = (1, 1, 1, 1)
        _RimPower("Rim Power", float) = 1
        _RimScale("Rim Scale", float) = 1
        _OutlineWidth("OutlineWidth", float) = 0.1
        _OutlineColor("OutlineColor", Color) = (1,1,1,1)
        _VertexDisplacement("Vertex Displacement", float) = 0
        _AlphaMultiplier("Alpha Multiplier", float) = 1
        _Panner("Panner", float) = 0
        _UVScale("UV Scale", float) = 1
        _HollowOn("Hollow On", float) = 0
    }
    SubShader
    {
        Pass{
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Cull Back
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        CGPROGRAM

        #pragma vertex VertexProgram
        #pragma fragment FragmentProgram

        half _OutlineWidth;
        half _HollowOn;
        half4 _OutlineColor;

        float4 VertexProgram(float4 position : POSITION, float3 normal : NORMAL) : SV_POSITION {
            position.xyz += normal * _OutlineWidth;
            return UnityObjectToClipPos(position);
        }

        half4 FragmentProgram() : SV_TARGET {
            half4 c = lerp(_OutlineColor, half4(0,0,0,0), _HollowOn);
            return c;
        }

        ENDCG
        }
    }
    FallBack "Diffuse"
}
