Shader "Unlit/HollowOutline"
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
         Pass {
            Tags { "Queue"="Transparent-1" "RenderType"="Transparent" }
            Cull Back
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM

            #pragma vertex VertexProgram
            #pragma fragment FragmentProgram

            half _OutlineWidth;
            half _HollowOn;

            float4 VertexProgram(float4 position : POSITION, float3 normal : NORMAL) : SV_POSITION {

                position.xyz += normal * _OutlineWidth;

                return UnityObjectToClipPos(position);

            }

            half4 _OutlineColor;

            half4 FragmentProgram() : SV_TARGET {
                half4 c = lerp(_OutlineColor, half4(0,0,0,0), _HollowOn);
                return c;
            }

            ENDCG

        }
        Tags { "Queue"="Transparent" "IgnoreProjector"="False" "RenderType"="Transparent" }

		// First Pass: Only render alpha (A) channel
		Pass {
			ColorMask A
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			fixed4 _Color;

			float4 vert(float4 vertex:POSITION) : SV_POSITION {
				return UnityObjectToClipPos(vertex);
			}

			fixed4 frag() : SV_Target {
				return _Color;
			}

			ENDCG
		}
       // Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        ColorMask RGB
        Blend SrcAlpha OneMinusSrcAlpha
        CGPROGRAM
        #pragma surface surf NoLighting vertex:vert alpha

        struct Input {
        float2 uv_MainTex;
        float3 viewDir;
        float3 worldPos;
        };

        float _Amount;
        fixed _RimPower;
        fixed _RimScale;
        fixed4 _RimColor;
        fixed _HollowOn;
        fixed4 _Color;
        fixed _EmissionPower;
        fixed4 _EmissionColor;
        fixed _VertexDisplacement;
        fixed _UVScale;
        fixed _Panner;
        sampler2D _MainTex;
        sampler2D _HologramTex;
        uniform float4 _HologramTex_ST;
        fixed _AlphaMultiplier;
        fixed Frensnel(float3 worldPos, float3 normal, fixed scale, fixed power)
        {
            float3 viewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
			float3 worldNormal = UnityObjectToWorldNormal( normal );
			float fresnelDot = dot( worldNormal, viewDir );
			float fresnel = ( 0.0 + scale * pow( 1.0 - fresnelDot, power ) );
            return fresnel;
        }
        float3 HologramPan(float3 worldPos, float3 normal)
        {
            float2 Pannerr = (float2(0.0 , _Panner)); 
            float2 worldPosXY = (float2(worldPos.x , worldPos.y));
			float2 panner = ( 1.0 * _Time.y * Pannerr + ( worldPosXY * _UVScale ));
            float3 VertDisplace = ( tex2Dlod( _HologramTex, float4( panner, 0, 0.0)));
            fixed rim = Frensnel(worldPos, normal, _RimScale, _RimPower);
            return VertDisplace * rim;
        }
        void vert (inout appdata_full v) 
        {
        	float3 worldPos = mul( unity_ObjectToWorld, v.vertex );
            float3 VertDisplace = HologramPan(worldPos, v.normal) * normalize(v.normal);
            float3 zero = (0,0,0);
            float3 modifiedVert = lerp(zero, VertDisplace, _HollowOn);

            v.vertex.xyz += (modifiedVert * _VertexDisplacement).rgb ;
        }
        fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten) 
		{
			return fixed4(s.Albedo, s.Alpha);
		}
        void surf (Input IN, inout SurfaceOutput o) {
        	fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
		   half rim = 1 - saturate(dot(normalize(IN.viewDir), o.Normal));
			float4 rimPower =  pow(rim, _RimPower) * _RimColor;
			rimPower *= _EmissionPower;
           
          

          /* fixed rim = Frensnel(IN.worldPos, o.Normal, _RimScale, _RimPower);
            half rimPower = (rim * _EmissionPower);
           */
            float3 worldPos = IN.worldPos;
            float3 VertDisplace = HologramPan(worldPos, o.Normal);
            float finalAlpha = lerp(c.a, VertDisplace.r, _HollowOn);
            o.Emission = lerp(half4(0,0,0,0), _EmissionColor, _HollowOn);
            o.Alpha = finalAlpha + _AlphaMultiplier;
            o.Albedo = lerp(tex2D (_MainTex, IN.uv_MainTex) * _Color, _RimColor, rimPower);
        }
        ENDCG
      
    }
}
