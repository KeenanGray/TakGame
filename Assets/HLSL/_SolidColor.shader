// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Keenan/SolidColor"
{
    Properties
    {
        [PerRendererData]_MainTex ("Texture", 2D) = "black" {}
        [PerRendererData] _Color ("Color", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
            "Queue"="Transparent"
        }
        ZWrite off
        Blend SrcAlpha OneMinusSrcAlpha // use alpha blending
        // Blend one one//use additive blending
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
                float4 tex = tex2D(_MainTex,i.uv);
                return fixed4(_Color.r,_Color.g,_Color.b,_Color.a) * tex;
            }
            ENDCG

        }
    }
}
