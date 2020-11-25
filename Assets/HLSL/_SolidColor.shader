// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Keenan/SolidColor"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "black" {}
        _Color ("Color", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.normal = v.normal;
                return o;
            }

            sampler2D _MainTex;
            fixed4 _Color;

            float4 frag(v2f i) : SV_TARGET
            {
                float3 normal = normalize(i.normal);
               // float3 color = (normal + 1) * 0.5;
                float4 tex = tex2D(_MainTex,i.uv);
                //return fixed4(color.r,color.g,color.b,1);
                return fixed4(_Color.r,_Color.g,_Color.b,0) * tex;
            }

            ENDCG

        }
    }
}
