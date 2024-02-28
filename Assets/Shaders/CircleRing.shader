
Shader "Unlit/CircleRing"
{
   Properties
	{
		_Refraction ("Refraction", Range (0.00, 10.0)) = 1.0
		_Power ("Power", Range (1.00, 10.0)) = 1.0
		_AlphaPower ("Vertex Alpha Power", Range (1.00, 10.0)) = 1.0
		_BumpMap( "Normal Map", 2D ) = "bump" {}
	    _RimIntensity("RimIntensity", Range(0,1)) = 0
        _RimPowerBlend("RimPowerBlend", Range(0,5)) = 1
		_Cull ( "Face Culling", Int ) = 2
	}

	SubShader
	{
		Tags { "Queue" = "Transparent+1" }

		GrabPass
		{
			"_GrabTexture"
		}
		
		Pass
		{
			Cull [_Cull]
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
				#pragma target 3.0
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"
				#include "UnityLightingCommon.cginc"
				#include "UnityStandardUtils.cginc"
				#include "UnityStandardInput.cginc"

				// From Valve's Lab Renderer, Copyright (c) Valve Corporation, All rights reserved. 
				float3 Vec3TsToWs( float3 vVectorTs, float3 vNormalWs, float3 vTangentUWs, float3 vTangentVWs )
				{
					float3 vVectorWs;
					vVectorWs.xyz = vVectorTs.x * vTangentUWs.xyz;
					vVectorWs.xyz += vVectorTs.y * vTangentVWs.xyz;
					vVectorWs.xyz += vVectorTs.z * vNormalWs.xyz;
					return vVectorWs.xyz; // Return without normalizing
				}

				// From Valve's Lab Renderer, Copyright (c) Valve Corporation, All rights reserved. 
				float3 Vec3TsToWsNormalized( float3 vVectorTs, float3 vNormalWs, float3 vTangentUWs, float3 vTangentVWs )
				{
					return normalize( Vec3TsToWs( vVectorTs.xyz, vNormalWs.xyz, vTangentUWs.xyz, vTangentVWs.xyz ) );
				}

				struct VS_INPUT
				{
					float4 vPosition : POSITION;
					float3 vNormal : NORMAL;
					float2 vTexcoord0 : TEXCOORD0;
					float4 vTangentUOs_flTangentVSign : TANGENT;
					float4 vColor : COLOR;
				};

				struct PS_INPUT
				{
					float4 vGrabPos : TEXCOORD0;
					float4 vPos : SV_POSITION;
					float4 vColor : COLOR;
    				float3 viewDirection : TEXCOORD5;
					float2 vTexCoord0 : TEXCOORD1;
					float3 vNormalWs : TEXCOORD2;
					float3 vTangentUWs : TEXCOORD3;
					float3 vTangentVWs : TEXCOORD4;
				};

				PS_INPUT vert(VS_INPUT i)
				{
					PS_INPUT o;
					
					// Clip space position
					o.vPos = UnityObjectToClipPos(i.vPosition);
					
					// Grab position
					o.vGrabPos = ComputeGrabScreenPos(o.vPos);
					
					// World space normal
					o.vNormalWs = UnityObjectToWorldNormal(i.vNormal);

					// Tangent
					o.vTangentUWs.xyz = UnityObjectToWorldDir( i.vTangentUOs_flTangentVSign.xyz ); // World space tangentU
					o.vTangentVWs.xyz = cross( o.vNormalWs.xyz, o.vTangentUWs.xyz ) * i.vTangentUOs_flTangentVSign.w;

					// Texture coordinates
					o.vTexCoord0.xy = i.vTexcoord0.xy;

					// Color
					o.vColor = i.vColor;

				    o.viewDirection = WorldSpaceViewDir(i.vPosition);

					return o;
				}

				sampler2D _GrabTexture;
				float _Refraction;
				float _Power;
				float _AlphaPower;
				fixed _RimIntensity;
        		float _RimPowerBlend;
				fixed4 _RimLightColor;
				float4 rimLight(float4 currentColor, float4 rimColor, float3 normal, float3 viewDirection)
				{
					float NdotV = 1 - dot(normal, viewDirection);
					NdotV = pow(NdotV, _RimPowerBlend);
					NdotV *= _RimIntensity;
					float4 finalColor = lerp(currentColor, rimColor, NdotV);
					return finalColor;
				}
				fixed4 frag(PS_INPUT i) : SV_Target
				{
					// Tangent space normals
					float3 vNormalTs = UnpackScaleNormal( tex2D( _BumpMap, i.vTexCoord0.xy ), 1 );

					// Tangent space -> World space
					float3 vNormalWs = Vec3TsToWsNormalized( vNormalTs.xyz, i.vNormalWs.xyz, i.vTangentUWs.xyz, i.vTangentVWs.xyz );

					// World space -> View space
					float3 vNormalVs = normalize(mul((float3x3)UNITY_MATRIX_V, vNormalWs));

					// Calculate offset
					float2 offset = vNormalVs.xy * _Refraction;
					offset *= pow(length(vNormalVs.xy), _Power);

					// Scale to pixel size
					offset /= float2(_ScreenParams.x, _ScreenParams.y);

					// Scale with screen depth
					offset /=  i.vPos.z;

					// Scale with vertex alpha
					offset *= pow(i.vColor.a, _AlphaPower);
					
					
                     
					// Sample grab texture
					float4 vDistortColor = tex2Dproj(_GrabTexture , i.vGrabPos + float4(offset, 0.0, 0.0));
				
					i.viewDirection = normalize(i.viewDirection);
					vDistortColor = rimLight(vDistortColor, i.vColor, i.vNormalWs, i.viewDirection);
				//	i.worldNormal = normalize(i.worldNormal);
   					
					// Debug normals
					// return float4(vNormalVs * 0.5 + 0.5, 1);
				//	float3 colRGB = vDistortColor.rgb  ;
				   //float4 finalCol = (colRGB,i.vColor.a );
					//return finalCol;
				    return vDistortColor;
				}
			ENDCG
		}
	}
}
