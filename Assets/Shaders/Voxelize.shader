Shader "Unlit/Voxelize"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _VoxelsAmount("VoxelsAmount", Range(0, 200)) = 100
		_Voxelization("Voxelization", Range(0, 1)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _VoxelsAmount;
			float _Voxelization;

            float4 Voxelize(float amount, float voxelsCount, float4 vertexPosition)
			{
			float4 modifiedPos;
			modifiedPos = round(vertexPosition * voxelsCount) / voxelsCount;
			return lerp(vertexPosition, modifiedPos, amount);
			}
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(Voxelize(_Voxelization, _VoxelsAmount, v.vertex.rgba));
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
