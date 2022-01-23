Shader "Unlit/InFrontShader"
{
    Properties
    {
        _MainColor ("Main Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags {
            "RenderType"="Opaque"
            "Queue"="AlphaTest+1"
        }
        LOD 100
        ZWrite Off
        ZTest Always


        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float4 _MainColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = (0,0);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return _MainColor;
            }
            ENDCG
        }
    }
}
