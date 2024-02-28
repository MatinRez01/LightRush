Shader "SupGames/PlanarReflection/Bumped Specular"
{
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Main Texture", 2D) = "white" {}
		_BumpTex("Normal Map", 2D) = "bump" {}
		_Glossiness("Glossiness", Range(0.01,100)) = 0.03
		_SpecColor("Specular Color", Color) = (1,1,1,1)
		_Distort("Distort Amount", Range(0.01,10)) = 1
		_MaskTex("Mask Texture", 2D) = "white" {}
		_BlurAmount("Blur Amount", Range(0,7)) = 1
	}
	SubShader{
		Tags {"RenderType" = "Opaque"}
		LOD 150
		Pass {
			Tags { "LightMode" = "ForwardBase" }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma shader_feature_local BLUR
			#pragma shader_feature_local VRon
			#pragma multi_compile_fwdbase novertexlight
			#pragma multi_compile_instancing
			#pragma multi_compile_fog
			#include "UnityCG.cginc" 
			#include "AutoLight.cginc"

			UNITY_DECLARE_SCREENSPACE_TEXTURE(_ReflectionTex);
#ifdef VRon
		UNITY_DECLARE_SCREENSPACE_TEXTURE(_ReflectionTexRight);
#endif
			UNITY_DECLARE_SCREENSPACE_TEXTURE(_MainTex);
			UNITY_DECLARE_SCREENSPACE_TEXTURE(_BumpTex);
			UNITY_DECLARE_SCREENSPACE_TEXTURE(_MaskTex);
			fixed _RefAlpha;
			fixed _Glossiness;
			fixed _Distort;
			fixed _BlurAmount;
			fixed4 _MainTex_ST;
			fixed4 _BumpTex_ST;
			fixed4 _Color;
			fixed4 _SpecColor;
			fixed _ReflectionAlpha;
			fixed4 _ReflectionTex_TexelSize;
			fixed4 _LightColor0;

			
			struct appdata
			{
				fixed4 vertex : POSITION;
				fixed4 uv : TEXCOORD0;
				fixed3 normal : NORMAL;
				fixed4 tangent : TANGENT;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				fixed4 pos : SV_POSITION;
				fixed4 uv : TEXCOORD0;
				fixed2 fogCoord : TEXCOORD1;
				fixed4 normal : TEXCOORD2;
				fixed4 tangent : TEXCOORD3;
				fixed4 bitangent : TEXCOORD4;
#if defined(BLUR)
				fixed4 offset : TEXCOORD5;
#endif
				LIGHTING_COORDS(6, 7)
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
				fixed3 viewDirection = _WorldSpaceCameraPos - mul(unity_ObjectToWorld, v.vertex).xyz;
				o.normal = fixed4(normalize(mul(fixed4(v.normal, 0.0h), unity_WorldToObject).xyz), viewDirection.x);
				o.tangent = fixed4(normalize(mul(unity_ObjectToWorld, fixed4(v.tangent.xyz, 0.0h)).xyz), viewDirection.y);
				o.bitangent = fixed4(cross(o.normal.xyz, o.tangent.xyz) * v.tangent.w * unity_WorldTransformParams.w, viewDirection.z);
				o.pos = UnityObjectToClipPos(v.vertex);
				fixed4 scrPos = ComputeNonStereoScreenPos(o.pos);
				o.uv.zw = scrPos.xy;
				o.fogCoord.y = scrPos.w;
#if defined(BLUR)
				fixed2 offset = _ReflectionTex_TexelSize.xy * _BlurAmount;
				o.offset = fixed4(-offset, offset);
#endif
				TRANSFER_VERTEX_TO_FRAGMENT(o);
				UNITY_TRANSFER_FOG(o, o.pos);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				fixed4 encodedNormal = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_BumpTex, _BumpTex_ST.xy * i.uv + _BumpTex_ST.zw);
				fixed3 localCoords = UnpackNormal(encodedNormal);
				fixed3 viewDirection = fixed3(i.normal.w, i.tangent.w, i.bitangent.w);
				fixed3 normalDirection = normalize(mul(localCoords, fixed3x3(i.tangent.xyz, i.bitangent.xyz, i.normal.xyz)));

				fixed3 diffuseReflection = UNITY_LIGHTMODEL_AMBIENT.rgb + _LightColor0.rgb * dot(normalDirection, _WorldSpaceLightPos0.xyz)* LIGHT_ATTENUATION(i);
				fixed3 specularReflection = _SpecColor.rgb * _LightColor0.rgb * pow(saturate(dot(normalDirection, normalize(_WorldSpaceLightPos0.xyz + viewDirection))), _Glossiness);

				fixed4 color = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv.xy);
				fixed4 mask = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MaskTex, i.uv.xy);
				fixed4 bump = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_BumpTex, i.uv.xy);

				i.uv.z += (bump.x - 0.5h) * _Distort;
				i.uv.w += (0.5h - bump.y) * _Distort;
				i.uv.zw /= i.fogCoord.y;
				fixed4 reflection = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_ReflectionTex, i.uv.zw);
#if defined(BLUR)
				i.offset /= i.fogCoord.y;
				i.offset = fixed4(i.uv.zz + i.offset.xz, i.uv.ww + i.offset.yw);
				reflection += UNITY_SAMPLE_SCREENSPACE_TEXTURE(_ReflectionTex, i.offset.xz);
				reflection += UNITY_SAMPLE_SCREENSPACE_TEXTURE(_ReflectionTex, i.offset.xw);
				reflection += UNITY_SAMPLE_SCREENSPACE_TEXTURE(_ReflectionTex, i.offset.yz);
				reflection += UNITY_SAMPLE_SCREENSPACE_TEXTURE(_ReflectionTex, i.offset.yw);
				reflection *= 0.2h;
#endif
#ifdef VRon
				fixed4 reflectionr = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_ReflectionTexRight, i.uv.zw);
#ifdef BLUR
				reflectionr += UNITY_SAMPLE_SCREENSPACE_TEXTURE(_ReflectionTexRight, i.offset.xz);
				reflectionr += UNITY_SAMPLE_SCREENSPACE_TEXTURE(_ReflectionTexRight, i.offset.xw);
				reflectionr += UNITY_SAMPLE_SCREENSPACE_TEXTURE(_ReflectionTexRight, i.offset.yz);
				reflectionr += UNITY_SAMPLE_SCREENSPACE_TEXTURE(_ReflectionTexRight, i.offset.yw);
				reflectionr *= 0.2h;
#endif
				reflection = lerp(reflection, reflectionr, unity_StereoEyeIndex);
#endif
				color = fixed4(specularReflection * (1.0h - color.a) + diffuseReflection * color.rgb, 1.0h);
				UNITY_APPLY_FOG(i.fogCoord.x, color);
				return (lerp(color, reflection, _RefAlpha * mask.r) + lerp(reflection, color, 1 - _RefAlpha * mask.r)) * _Color * 0.5h;
			}
			ENDCG
		}
	}
}