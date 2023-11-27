Shader "Custom/MaskShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _MaskTex("Mask Texture", 2D) = "white" {}
        _MaskColor("Mask Color", Color) = (1,1,1,1)
        _MaskReplace("Mask Replace Texture", 2D) = "white" {}
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex:POSITION;
            };

            struct v2f
            {
                float4 clipPos:SV_POSITION;
                float2 screenPos: TEXCOORD0;
            };

            sampler2D _MainTex;
            sampler2D _MaskTex;
            sampler2D _MaskReplace;
            fixed4 _MaskColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.clipPos=UnityObjectToClipPos(v.vertex);

                // this will have origin at bottom left corner and the width and height of screen right?
                o.screenPos = ComputeScreenPos(o.clipPos);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 color;
                fixed4 maskPixel = tex2D(_MaskTex, i.screenPos.xy * _ScreenParams.zw);
                if (maskPixel.r == 1) {
                    color = fixed4(1,0,0,0);
                } else {
                    color = fixed4(0,0,1,0);
                }
                //o.color = fixed4(0,1,0,0);
                
                return color;
            }
            ENDCG
        }
    }
}