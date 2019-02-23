Shader "Unlit/Accelerator"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		[PerRendererData] _Color ("Tint", Color) = (1,1,1,1)
		
		[Header(Properties)]
		_Speed ("Speed", Float) = 64
		_Amp ("Amplitude", Float) = 2
		_Width ("Width", Float) = 10

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

		//UsePass "Sprites/NonWhiteColorization/MainPass"

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
				float2 texcoord : TEXCOORD0;
			};
			
			fixed4 _Color;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _AlphaTex;
			float _AlphaSplitEnabled;
			float4 _MainTex_TexelSize;
			uniform float _Speed;
			uniform float _Amp;
			uniform float _Width;

			float _XSpeed;
			float _YSpeed;

			fixed4 Displace(float2 uv, float4 inColor)
			{
				float2 final = uv;

				//uv offset
				final.y = (uv.y + (_Time.w * _YSpeed));
				final.x = (uv.x + (_Time.w * _XSpeed));

				final.y += tan(final.x*(1/_Width) + (_Time.w)*_Speed) * (_Amp * sin(_Time.w*_Speed));

				fixed4 color = tex2D (_MainTex, final);
				color.rgb *= inColor.rgb;
				color.rgb *= color.a;
				return color;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 c = Displace(IN.texcoord, IN.color);
				return c;
			}
		ENDCG
		}
	}
}
