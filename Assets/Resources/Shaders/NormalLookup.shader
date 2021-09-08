Shader "Custom/NormalLookup" {

	Properties {
		_ColorMap ("Sprite Texture", 2D) = "white" {}
	}

    SubShader {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

			struct v2f {
				float4 pos : SV_POSITION;
				float3 normal : TEXCOORD0;
			};


			v2f vert (appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.normal = UnityObjectToWorldNormal(v.normal);
				return o;
			}

            sampler2D _ColorMap;
        
            fixed4 frag (v2f i) : SV_Target {
				half4 color;
				color = tex2D(_ColorMap, (i.normal.xy + 1) / 2);
				return color;
            }
            ENDCG
        }
    }
}
