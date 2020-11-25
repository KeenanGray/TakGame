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
            "Glowable"="True"
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
            float4 _MainTex_TexelSize;
            fixed4 _Color;


            float4 box(sampler2D tex, float2 uv, float4 size)
            {
                float4 c = tex2D(tex, uv + float2(-size.x,size.y)) + tex2D(tex,uv+float2(0,size.y)) +tex2D(tex,uv+float2(size.x,size.y)) +
                tex2D(tex, uv + float2(-size.x,0)) + tex2D(tex,uv+float2(0,0)) +tex2D(tex,uv+float2(size.x,0)) +
                tex2D(tex, uv + float2(-size.x,-size.y)) + tex2D(tex,uv+float2(0,-size.y)) +tex2D(tex,uv+float2(size.x,-size.y));
                
                return c/9;
            }

            float4 frag(v2f i) : SV_TARGET
            {
                float4 col = box(_MainTex,i.uv,_MainTex_TexelSize);
                return col;
            }

            ENDCG

        }
    }
}
