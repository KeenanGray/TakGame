// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Keenan/OverDraw"
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
            "Queue" = "Transparent"
        }      
        
        ZTest Always
        ZWrite off
        
        //additive blending
        Blend One One

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct AppData
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float depth : depth;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;

            half4 _ModColor;
            half4 _Color;

            v2f vert (AppData v)
            {
                v2f o;
                o.uv = v.uv;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }
            
            fixed4 frag(v2f i) : SV_TARGET
            {
                fixed4 c = tex2D(_MainTex,i.uv);

                return c * fixed4(clamp(_ModColor.r,0,.15),
                clamp(_ModColor.g,0,.15),
                clamp(_ModColor.b,0,.15),
                c.a) * _Color;
            }
            ENDHLSL
        }  
    }
}
