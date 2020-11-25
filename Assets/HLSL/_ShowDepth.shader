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

            float Factor;
            float4 _Color;

            v2f vert (AppData v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.depth = -UnityObjectToViewPos(v.vertex).z * Factor;
                return o;
            }

            
            fixed4 frag(v2f i) : SV_TARGET
            {
                float invert = 1-i.depth;
                return fixed4(invert,invert,invert,1) * _Color;
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
        Blend SrcAlpha OneMinusSrcAlpha 
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

            half4 _Color;

            v2f vert (AppData v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            
            fixed4 frag(v2f i) : SV_TARGET
            {
                return  _Color;
            }
            ENDHLSL
        }  
    }
}
