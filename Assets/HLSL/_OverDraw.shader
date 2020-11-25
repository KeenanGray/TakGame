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
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float depth : depth;
            };

            half4 _OverDrawColor;

            v2f vert (AppData v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            
            fixed4 frag(v2f i) : SV_TARGET
            {
                return  _OverDrawColor;
            }
            ENDHLSL
        }  
    }
}
