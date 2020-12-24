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

		[Header(Transparency)]
		_TransparentColor ("Transparent Color", Color) = (1, 1, 1, 1)
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

		Pass {

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UNITYCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : POSITION;
				float4 uvgrab : TEXCOORD0;
			};
			
			v2f vert(appdata_t v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uvgrab = ComputeGrabScreenPos(o.vertex);
				return o;
			}

			sampler2D _GrabTexture;
			float4 _GrabTexture_TexelSize;
			float4 _MainTex_TexelSize;

			half4 frag(v2f i): COLOR {
				half4 color = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(i.uvgrab));
				return color;
			}

			ENDCG
		}

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ PIXELSNAP_ON
			#include "UnityCG.cginc"
			#include "Assets/Resources/Shaders/utils.cginc"
			
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
				float4 uvgrab   : TEXCOORD0;
				float2 uvmain   : TEXCOORD2;
				float3 worldPos : TEXCOORD3;
			};
			
			fixed4 _Color;
			float4 _MainTex_ST;

			v2f vert(appdata_t v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				// comment this or else tilemap offsets get messed up somehow (why??)
				o.uvgrab = ComputeGrabScreenPos(o.vertex);
				
				o.uvmain = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.worldPos = mul (unity_ObjectToWorld, v.vertex);

				o.color = v.color * _Color;

				return o;
			}

			sampler2D _MainTex;

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
			fixed4 _TransparentColor;

			float2 SineDisplace (float2 uv)
			{
				float2 final = uv;
				
				float4 time = _Time;


				//uv offset
				final.y = (uv.y + (time.w * _YSpeed));
				final.x = (uv.x + (time.w * _XSpeed));

				// x waves
				final.y += (_XAmp * sin((uv.x/_XWidth) + (time * _XWaveSpeed)));

				// y waves
				final.x += (_YAmp * sin((uv.y/_YWidth) + (time * _YWaveSpeed)));
                return final;
			}

			float2 BGDisplace(float2 uv, float3 worldPos) {
				float2 final = uv;
				float4 time = _Time;

				// x waves
				final.y += (0.002 * sin((worldPos.x/0.02) + (time * 200)));

				// y waves
				final.x += (0.002 * sin((worldPos.y/0.02) + (time * 200)));
                return final;
			}

			fixed4 frag(v2f i) : SV_Target
			{

				i.uvgrab.xy = BGDisplace(i.uvgrab.xy, i.worldPos);

				half4 grabPixel = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(i.uvgrab)) * _TransparentColor;
				half4 texPixel = tex2D(_MainTex, SineDisplace(i.uvmain));


				fixed4 color = lerp(grabPixel, texPixel, round(texPixel.a));
				
				// discard distorted pixel if it's completely transparent
				if (any(texPixel.a == 0)) {
					color.a = 0;
				}

                //color *= tint;
                color.rgb = lerp(color.rgb,_FlashColor.rgb,_FlashColor.a);
				color.rgb *= color.a;

				return color;
			}
		ENDCG
		}
	}
}
