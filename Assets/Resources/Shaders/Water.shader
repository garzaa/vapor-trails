Shader "Custom/Water"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		[PerRendererData] _Color ("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
		[PerRendererData] _FlashColor ("Flash Color", Color) = (1,1,1,0)
		
		[Header(XProperties)]
		_XWaveSpeed ("X Wave Speed", Float) = 20
		_XAmp ("X Amplitude", Float) = 0.2
		_XWidth ("X Width", Float) = 1.0

		[Header(YProperties)]
		_YWaveSpeed ("Y Wave Speed", Float) = 20
		_YAmp ("Y Amplitude", Float) = 0.2
		_YWidth ("Y Width", Float) = 1.0

		[Header(Movement)]
		_XSpeed ("X Speed", Float) = 0
		_YSpeed ("Y Speed", Float) = 0
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

        GrabPass {
            Tags { "LightMode" = "Always" }
        }

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
				float2 texcoord : TEXCOORD0;
			};
			
			fixed4 _Color;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;

				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _AlphaTex;
			float _AlphaSplitEnabled;
			float4 _MainTex_TexelSize;
			uniform half _PixelSize;

			uniform float _XWaveSpeed;
			uniform float _XAmp;
			uniform float _XWidth;

			uniform float _YWaveSpeed;
			uniform float _YAmp;
			uniform float _YWidth;

			float _XSpeed;
			float _YSpeed;

            sampler2D _GrabTexture;
            float4 _GrabTexture_TexelSize;

			fixed4 _FlashColor;

			float2 SineDisplace (float2 uv)
			{
				float2 final = uv;

				//uv offset
				final.y = (uv.y + (_Time.w * _YSpeed));
				final.x = (uv.x + (_Time.w * _XSpeed));

				// x waves
				final.y += (_XAmp * sin((uv.x/_XWidth) + (_Time * _XWaveSpeed)));

				// y waves
				final.x += (_YAmp * sin((uv.y/_YWidth) + (_Time * _YWaveSpeed)));
                return final;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
                fixed4 color = tex2D(_MainTex, SineDisplace(IN.texcoord));

                // then do the grabpass displacement
                // fixed4 tint = tex2D(_GrabTexture, SineDisplace(IN.texcoord));

                //color *= tint;
                color.rgb = lerp(color.rgb,_FlashColor.rgb,_FlashColor.a);
				color.rgb *= color.a;

				return color;// * tint;
			}
		ENDCG
		}
	}
}
