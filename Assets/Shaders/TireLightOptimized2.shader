Shader "Unlit/TireLightOptimized2"
{
    Properties
    {
        [HDR]_BaseColor("Color", Color) = (0, 0, 0, 0)
        _MainTex("Texture2D", 2D) = "white" {}
        _AddColor("AddColor", Color) = (0, 0, 0, 0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                #ifdef INSTANCING_ON 
                UNITY_VERTEX_INPUT_INSTANCE_ID
                #endif
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID             
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            half4 _AddColor;
            UNITY_INSTANCING_BUFFER_START(ccoll)	
            UNITY_DEFINE_INSTANCED_PROP(half4, _BaseColor)
            UNITY_INSTANCING_BUFFER_END(coll)	
            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
                float3 worldPos = mul(unity_ObjectToWorld, float4(i.vertex.xyz, 1)).xyz;
                float2 newUV = (i.uv.x, i.uv.y -worldPos.b );
                fixed4 col = tex2D(_MainTex, newUV ) ;
                half4 coloredCol = (col + _AddColor) * UNITY_ACCESS_INSTANCED_PROP(coll, _BaseColor) ;
                return coloredCol;
            }
            ENDCG
        }
    }
}
