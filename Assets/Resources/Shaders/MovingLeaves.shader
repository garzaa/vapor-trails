Shader "Custom/MovingLeaves"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [PerRendererData] _FlashColor ("Flash Color", Color) = (1,1,1,0)

        [Header(Background Distortion)]
		_BumpAmt   ("Distortion", Range(0, 128)) = 10
        _BumpMap   ("Normal Map", 2D) = "bump" {}
        _BumpScale ("Normal Map Scale", Vector) = (1,1,1,1)
        _MoveSpeed ("Normal Map Speed", Vector) = (0,0,0,0)
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ PIXELSNAP_ON
			#include "UnityCG.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				float2 texcoord  : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
			};
			
			fixed4 _Color;

            float _BumpAmt;
			sampler2D _BumpMap;
			float4 _BumpScale;
			float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
			float4 _MoveSpeed;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif
                OUT.worldPos = mul(unity_ObjectToWorld, IN.vertex);

				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _AlphaTex;
			float _AlphaSplitEnabled;
			fixed4 _FlashColor;

			fixed4 SampleSpriteTexture (float2 uv)
			{
				fixed4 color = tex2D (_MainTex, uv);

#if UNITY_TEXTURE_ALPHASPLIT_ALLOWED
				if (_AlphaSplitEnabled)
					color.a = tex2D (_AlphaTex, uv).r;
#endif //UNITY_TEXTURE_ALPHASPLIT_ALLOWED

				return color;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
                float g = SampleSpriteTexture(IN.texcoord).g;

                float2 bump_uv = IN.worldPos;
                bump_uv /= _BumpScale.xy;
                bump_uv += _Time.w * _MoveSpeed.xy * g;

                half2 bump = UnpackNormal(tex2D(_BumpMap, bump_uv)).rg;
                float2 offset = bump * _BumpAmt * _MainTex_TexelSize.xy;
                IN.texcoord += offset;
				
                fixed4 c = SampleSpriteTexture (IN.texcoord) * IN.color;


                c.rgb = lerp(c.rgb,_FlashColor.rgb,_FlashColor.a);
				c.rgb *= c.a;
				return c;
			}
		ENDCG
		}
	}
}
