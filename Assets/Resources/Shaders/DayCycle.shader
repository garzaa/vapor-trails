// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/DayCycle"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		[PerRendererData] _Daytime ("Daytime", Range(0.0, 1.0)) = 1
		_ReplaceRamp ("Sprite Texture", 2D) = "white" {}
		_SwapMin ("Min Swap Time", Float) = 0.75
		_SwapMax ("Max Swap Time", FLoat) = 0.25
		_Color ("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
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
			sampler2D _ReplaceRamp;

			float _Daytime;
			float _SwapMax;
			float _SwapMin;
			
			float _AlphaSplitEnabled;

			fixed4 SampleSpriteTexture (float2 uv, fixed4 vertexColor)
			{
				fixed4 preVertex = tex2D (_MainTex, uv);
				fixed4 color = preVertex * vertexColor;

				// if before 6am        or after 6pm
				if (_Daytime < _SwapMin || _Daytime > _SwapMax) {
					fixed4 replaceColor = tex2D(_ReplaceRamp, float2(preVertex.r, 0));
					color = lerp(color, replaceColor, replaceColor.a);
				}

#if UNITY_TEXTURE_ALPHASPLIT_ALLOWED
				if (_AlphaSplitEnabled)
					color.a = tex2D (_AlphaTex, uv).r;
#endif //UNITY_TEXTURE_ALPHASPLIT_ALLOWED

				return color;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 c = SampleSpriteTexture (IN.texcoord, IN.color);
				c.rgb *= c.a;
				return c;
			}
		ENDCG
		}
	}
}
