Shader "Unlit/PixelWater"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		[PerRendererData] _Color ("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
		
		[Header(Waves)]
		_Speed ("Speed", Float) = 64
		_Amp ("Amplitude", Float) = 2
		_Width ("Width", Float) = 10
		_Vertical ("Vertical", Range (0, 10)) = 0

		[Header(Movement)]
		_XSpeed ("X Speed", Float) = 1
		_YSpeed ("Y Speed", Float) = 1
		_YDisplacment ("Y Displacement", Float) = 1
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
			uniform half _PixelSize;
			uniform float _Speed;
			uniform float _Amp;
			uniform float _Width;
			uniform float _Vertical;

			float _XSpeed;
			float _YSpeed;
			float4 _MainTex_ST;
			float _YDisplacment;

			fixed4 SineDisplace (float2 uv)
			{
				float2 final = uv;
				//uv offset
				final.y = (uv.y + _Time.w * _YSpeed);
				final.x += sin(uv.y*_Width + _Time.w*_Speed) * _Amp;
				
				fixed4 color = tex2D(_MainTex, final);
				return color;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 c = SineDisplace (IN.texcoord) * IN.color;
				c.rgb *= c.a;
				return c;
			}
		ENDCG
		}
	}
}
