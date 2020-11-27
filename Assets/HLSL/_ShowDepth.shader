Shader "Keenan/ShowDepth"
{
    Properties
    {
        _Color   ("Color",Color) = (0,0,0,0)
        Factor ("Factor", Float) = 100
    }

    SubShader
    {
        
        Tags
        {
            "RenderType" = "Opaque"
        }      
        
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

            float _Factor;
            float4 _Color;
            half4 _ModColor;

            v2f vert (AppData v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.depth = -UnityObjectToViewPos(v.vertex).z * lerp(0,1,_Factor);
                return o;
            }

            
            fixed4 frag(v2f i) : SV_TARGET
            {
                float invert = 1-i.depth;
                return fixed4(invert,invert,invert,1)  * _Color * _ModColor;
            }
            ENDHLSL
        }  
    }
    SubShader
    {
        
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType" = "Transparent"
        }      
        
        ZWrite off
        Blend SrcAlpha One 
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

            half4 _Color;
            sampler2D _MainTex;

            v2f vert (AppData v)
            {
                v2f o;
                o.uv = v.uv;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            
            fixed4 frag(v2f i) : SV_TARGET
            {
                fixed4 c = tex2D (_MainTex, i.uv) * _Color;
                return  c;
            }
            ENDHLSL
        }  
    }
}
