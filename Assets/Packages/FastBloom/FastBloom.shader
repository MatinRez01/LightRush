Shader "RufatShaderlab/FastBloom"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "" {}
	}

	CGINCLUDE
	#include "UnityCG.cginc"
	#pragma multi_compile_local _ _USE_RGBM
	UNITY_DECLARE_SCREENSPACE_TEXTURE(_MainTex);
	UNITY_DECLARE_SCREENSPACE_TEXTURE(_BloomTex);
	half4 _MainTex_TexelSize;
	half4 _MainTex_ST;
	half4 _BloomTex_ST;
	half _BlurAmount;
	half4 _BloomColor;
	half4 _BloomData;
	half _BloomAmount;

	struct appdata {
		half4 pos : POSITION;
		half2 uv : TEXCOORD0;
		UNITY_VERTEX_INPUT_INSTANCE_ID
	};

	struct v2f {
		half4 pos : SV_POSITION;
		half2 uv : TEXCOORD0;
		UNITY_VERTEX_OUTPUT_STEREO
	};

	struct v2fb {
		half4 pos : SV_POSITION;
		half2 uv : TEXCOORD0;
		half4 uv1 : TEXCOORD1;
		UNITY_VERTEX_OUTPUT_STEREO
	};

	float3 Unpack(float4 c)
	{
#if UNITY_COLORSPACE_GAMMA
		c.rgb *= c.rgb;
#endif
#if _USE_RGBM
		return c.xyz * c.w * 8.0;
#else
		return c.rgb;
#endif
	}

	half4 Pack(half3 c)
	{
#if _USE_RGBM
		c *= 0.125;
		half m = max(max(c.x, c.y), max(c.z, 1e-5));
		m = ceil(m * 255) / 255;
		half4 o = half4(c / m, m);
#else
		half4 o = half4(c, 1.0);
#endif
#if UNITY_COLORSPACE_GAMMA
		return half4(sqrt(o.rgb), o.a);
#else
		return o;
#endif
	}

	v2fb vertBlur(appdata i)
	{
		v2fb o;
		UNITY_SETUP_INSTANCE_ID(i);
		UNITY_INITIALIZE_OUTPUT(v2fb, o);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
		o.pos = UnityObjectToClipPos(i.pos);
		o.uv = UnityStereoScreenSpaceUVAdjust(i.uv, _MainTex_ST);
		half2 offset = _MainTex_TexelSize * (1.0 / _MainTex_ST.xy);
		o.uv1 = half4(UnityStereoScreenSpaceUVAdjust(i.uv - offset, _MainTex_ST), UnityStereoScreenSpaceUVAdjust(i.uv + offset, _MainTex_ST));
		return o;
	}

	v2f vert(appdata i)
	{
		v2f o;
		UNITY_SETUP_INSTANCE_ID(i);
		UNITY_INITIALIZE_OUTPUT(v2f, o);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
		o.pos = UnityObjectToClipPos(i.pos);
		o.uv = UnityStereoScreenSpaceUVAdjust(i.uv, _MainTex_ST);
		return o;
	}

	half4 fragBloom(v2f i) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
		half3 c = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv).xyz;
#if UNITY_COLORSPACE_GAMMA
		c.rgb *= c.rgb;
#endif
		half br = max(c.r, max(c.g, c.b));
		half soft = clamp(br - _BloomData.y, 0.0, _BloomData.z);
		half a = max(soft * soft * _BloomData.w, br - _BloomData.x) / max(br, 1e-4);
		return Pack(c * a);
	}

	half4 fragBlur(v2fb i) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
		half3 c = Unpack(UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv));
		c += Unpack(UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv1.xy));
		c += Unpack(UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv1.xw));
		c += Unpack(UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv1.zy));
		c += Unpack(UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv1.zw));
		return Pack(c * 0.2);
	}

	float4 fragUp(v2f i) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
		return Pack(lerp(Unpack(UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv)), Unpack(UNITY_SAMPLE_SCREENSPACE_TEXTURE(_BloomTex, i.uv)), _BlurAmount));
	}

	half4 frag(v2f i) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
		half4 c = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv);

#if UNITY_COLORSPACE_GAMMA
		c.rgb *= c.rgb;
#endif
		half3 bloom = Unpack(UNITY_SAMPLE_SCREENSPACE_TEXTURE(_BloomTex, i.uv));
		c.rgb += bloom * _BloomColor.rgb;

#if UNITY_COLORSPACE_GAMMA
		return half4(sqrt(c.rgb), c.a);
#else
		return c;
#endif
	}

	ENDCG

	Subshader
	{
		LOD 100
		ZTest Always ZWrite Off Cull Off
		Fog{ Mode off }
		Pass //0
		{
		  CGPROGRAM
		  #pragma vertex vertBlur
		  #pragma fragment fragBloom
		  ENDCG
		}

		Pass //1
		{
		  CGPROGRAM
		  #pragma vertex vertBlur
		  #pragma fragment fragBlur
		  ENDCG
		}

		Pass //2
		{
		  CGPROGRAM
		  #pragma vertex vert
		  #pragma fragment fragUp
		  ENDCG
		}

		Pass //3
		{
		  CGPROGRAM
		  #pragma vertex vert
		  #pragma fragment frag
		  ENDCG
		}
	}
	Fallback off
}