// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Keenan/GlowComposite"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "black" {}

        _Color ("Color", Color) = (1,1,1,1)
        _Intensity("Intensity",float) = 0
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
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv0 = v.uv0;
                o.uv1 = v.uv1;
                o.normal = v.normal;
                return o;
            }

            sampler2D _MainTex;
            sampler2D _GlowBlurredTex;
            sampler2D _GlowPrePassTex;

            fixed4 _Color;
            float _Intensity;

            float4 frag(v2f i) : SV_TARGET
            {
                fixed4 col = tex2D(_MainTex,i.uv0);
                fixed4 blurred = tex2D(_GlowBlurredTex,i.uv1);
                fixed4 prepass = tex2D(_GlowPrePassTex,i.uv1);
                fixed4 glow = max(0,blurred-prepass);
                
                return col + glow * _Intensity;
            }

            ENDCG

        }
    }
}
