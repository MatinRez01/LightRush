
Shader "Game/Hologram" {
	Properties {
		[HDR]_Color("Main Color", Color) = (1, 1, 1, 1)
		_MainTex("Base (RGB) Trans (A)", 2D) = "white" { }
		_RimColor("RimColor", Color) = (1,1,1,1)
        _RimPower("RimPowerBlend", Range(0,20)) = 0
		_RimLightingEnabled("RimLighting", Range(0,1)) = 0
		_EmissionPower("EmissionPower", Range(0,20) ) = 0
		_ColorFill("ColorFill", Color) = (1, 1, 1, 1)
		_Fill("Fill", Range(0,1)) = 0
		_Saturation("Saturation", Range(0,1)) = 1
		_Brightness("Brightness", Range(0,5)) = 0
	}

	SubShader {
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

		// Second Pass: Now render color (RGB) channel
		ColorMask RGB
		Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM
		#pragma surface surf NoLighting alpha noambient noshadow novertexlights nolightmap noforwardadd nometa 

		sampler2D _MainTex;
		
		fixed4 _Color;
		fixed4 _RimColor;
        fixed _RimPower;
		fixed _EmissionPower;
		fixed _RimLightingEnabled;
		fixed _Saturation;
		fixed _Brightness;
		fixed4 _ColorFill;
		fixed _Fill;

		struct Input {
			float4 color:COLOR;
			float2 uv_MainTex;
			float3 viewDir;
			
		};
			
		fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten) 
		{
			return fixed4(s.Albedo, s.Alpha);
		}

		float4 ColorCorrection(float4 col)
		{
		   // c *= half4(_TintColor, 1);
            col = clamp(col + col * _Brightness, 0.0, 1.0);
            half maxVal = max(max(col.r, col.g), col.b);
            col = half4(lerp(col.r, maxVal, 1 - _Saturation), lerp(col.g, maxVal,
			 1 - _Saturation), lerp(col.b, maxVal, 1 - _Saturation), col.a);
			return col;
		}
		

		
		void surf(Input IN, inout SurfaceOutput o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			
		    half rim = 1 - saturate(dot(normalize(IN.viewDir), o.Normal));
			float4 rimPower =  pow(rim, _RimPower) * _RimColor;
			rimPower *= _EmissionPower;
			float4 finalColor = lerp(c, rimPower, _RimLightingEnabled) + (_Fill * _ColorFill);
			o.Albedo = _RimLightingEnabled>0.4f ?  finalColor : ColorCorrection(finalColor);
			
         //   o.Emission = _RimLight.rgb * _EmissionPower;
			o.Alpha =  finalColor.a;
		}
     
	    
		ENDCG
		
	}

	Fallback "Legacy Shaders/Transparent/Diffuse"
}