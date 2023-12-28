// Upgrade NOTE: replaced '_Projector' with 'unity_Projector'

Shader "Custom/UnwrapShader" {
	Properties {
		_ProjectedTexture ("Projected Texture", 2D) = "" { TexGen ObjectLinear }
	}
	SubShader {
		Tags { "Queue" = "Transparent" }
		Pass {
			Cull Off
			AlphaTest Greater 0
			Blend SrcAlpha OneMinusSrcAlpha
			
			CGPROGRAM

			#pragma vertex vert_main
			#pragma fragment frag_main
			#include "UnityCG.cginc"
		
			sampler2D _ProjectedTexture;
			float4x4 unity_Projector;
		
			struct Output_Struct {
	    		float4 position : SV_POSITION;
	    		float4 texcoord : TEXCOORD0;
			};	
			
			Output_Struct vert_main (appdata_tan v) {
	    		Output_Struct OUT;

				OUT.position.xy = v.texcoord.xy * 2.0 - 1.0;
				OUT.position.z = 0;
				OUT.position.w = v.vertex.w;
				
				OUT.texcoord = mul(unity_Projector, v.vertex);
				return OUT;
			}
			
			float4 frag_main (Output_Struct OUT) : COLOR {
				half4 tex = tex2Dproj(_ProjectedTexture, UNITY_PROJ_COORD(OUT.texcoord)) * half4(1,0,0,1);
    			return tex;
			}
	
			ENDCG
    	}
	}
}