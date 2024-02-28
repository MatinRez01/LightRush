Shader "SupGames/Mobile/Vignette"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "" {}
	}

	CGINCLUDE

#include "UnityCG.cginc"

	struct appdata
	{
		fixed4 pos : POSITION;
		fixed2 uv : TEXCOORD0;
	};

	struct v2f 
	{
		fixed4 pos : POSITION;
		fixed4 uv : TEXCOORD0;
	};

	sampler2D _MainTex;
	fixed _Vignette;

	v2f vert(appdata i)
	{
		v2f o;
		o.pos = UnityObjectToClipPos(i.pos);
		o.uv.xy = i.uv;
		o.uv.zw = i.uv - 0.5h;
		return o;
	}

	fixed4 frag(v2f i) : COLOR
	{
		return tex2D(_MainTex, i.uv.xy)*(1.0h - dot(i.uv.zw, i.uv.zw) * _Vignette);
	}

	ENDCG

	Subshader
	{
		Pass
		{
		  ZTest Always Cull Off ZWrite Off
		  Fog { Mode off }
		  CGPROGRAM
		  #pragma vertex vert
		  #pragma fragment frag
		  #pragma fragmentoption ARB_precision_hint_fastest
		  ENDCG
		}
	}
	Fallback off
}