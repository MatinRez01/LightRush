Shader "SupGames/PlanarReflection/UnlitTransparent"
{
	Properties{
		[HDR]_Color("Color", Color) = (1,1,1,1)
		_MainTex("Main Texture", 2D) = "white" {}
		_MaskTex("Mask Texture", 2D) = "white" {}
		_BlurAmount("Blur Amount", Range(0,7)) = 1
		_Alpha("Alpha", Range(0,1)) = 1
		_center("Center", Vector) = (0,0,0,0)
		_Radius("Radius", float) = 1
		_lightFalloff("falloff", float) = 0.05
		_DimColor("DimColor", Color) = (0.5,0.5,0.5,1)
		[HDR]_lightColor("LightColor", Color) = (1,1,1,1)
		_lightIntensity("Light Intensity", float) = 1
	}
	SubShader{
		Tags {"RenderType" = "Transparent" "Queue"="Transparent"}
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile_instancing
			#pragma multi_compile_fog

			#include "UnityCG.cginc"

			UNITY_DECLARE_SCREENSPACE_TEXTURE(_ReflectionTex);
			UNITY_DECLARE_SCREENSPACE_TEXTURE(_MainTex);
			UNITY_DECLARE_SCREENSPACE_TEXTURE(_MaskTex);
			fixed4 _LightColor0;
			fixed _RefAlpha;
			float4 _MainTex_ST;
			fixed4 _Color;
			fixed4 _lightColor;
			fixed4 _DimColor;
			float _Radius;
			float2 _center;
			fixed _BlurAmount;
			fixed4 _ReflectionTex_TexelSize;
			fixed _Alpha;
			float _lightFalloff;
			fixed _lightIntensity;
			struct appdata 
			{
				fixed4 vertex : POSITION;
				fixed2 uv : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				fixed4 pos : SV_POSITION;
				fixed4 uv : TEXCOORD0;
				fixed2 fogCoord : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			v2f vert(appdata v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_OUTPUT(v2f, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				o.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);
				o.pos = UnityObjectToClipPos(v.vertex);
				fixed4 scrPos = ComputeNonStereoScreenPos(o.pos);
				o.uv.zw = scrPos.xy;
				o.fogCoord.y = scrPos.w;
				UNITY_TRANSFER_FOG(o, o.pos);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
                float dist = length(_MainTex_ST.zw - (i.uv - _center));
				float t = smoothstep(_Radius, _Radius - _lightFalloff, dist);
                float4 l = lerp(_DimColor, _lightColor, t) * _lightIntensity;
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				fixed4 color = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv.xy);
				fixed4 mask = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MaskTex, i.uv.xy);
				i.uv.zw /= i.fogCoord.y;
				fixed4 reflection = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_ReflectionTex, i.uv.zw);
				UNITY_APPLY_FOG(i.fogCoord.x, color);
				fixed4 col = (lerp(color, reflection, _RefAlpha * mask.r) + lerp(reflection, color, 1 - _RefAlpha * mask.r)) * _Color * 0.5h;
				col.a = _Alpha;
				return col * l;
			}
			ENDCG
		}
	}
}