// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Sprites/NonWhiteWithDisplacement" {
Properties {
	_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
    _Amount ("DisplacementAmount", Range (0, 1)) = 1
    [Header(Waves)]
		_Speed ("Speed", Float) = 64
		_Amp ("Amplitude", Float) = 2
		_Width ("Width", Float) = 10
		_Vertical ("Vertical", Range (0, 10)) = 0

    [Header(Movement)]
		_XSpeed ("X Speed", Float) = 1
		_YSpeed ("Y Speed", Float) = 1
		_YDisplacment ("Y Displacement", Float) = 1
	[PerRendererData] _MainTex ("Particle Texture", 2D) = "white" {}
}

Category {
	Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}
	Name "MainPass"
	Blend SrcAlpha OneMinusSrcAlpha
	AlphaTest Greater .01
	ColorMask RGB
	Cull Off Lighting Off ZWrite Off Fog { Color (0,0,0,0) }
	BindChannels {
		Bind "Color", color
		Bind "Vertex", vertex
		Bind "TexCoord", texcoord
	}
	
	// ---- Fragment program cards
	SubShader {
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma fragmentoption ARB_fog_exp2

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _TintColor;
            float _Amount;
			
			struct appdata_t {
				float4 vertex : POSITION;
				float4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : POSITION;
				float4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};
			
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

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
				return o;
			}

            fixed4 SineDisplace (float2 uv)
			{
				float2 final = uv;
				//uv offset
				final.y = (uv.y + (_Time.w * _YSpeed) * _MainTex_TexelSize.y) % _MainTex_TexelSize.z;
				final.y += floor(_YDisplacment * sin(floor(uv.y / _MainTex_TexelSize.y) / 1 + (_Time * 10))) * _MainTex_TexelSize.y;
		
				//sinewave displacement
				final.y += floor(_Amount *_Amp * _Vertical * sin(floor(uv.x / _MainTex_TexelSize.x) / _Width + (_Time * _Speed))) * _MainTex_TexelSize.y;
				final.x += floor(3 * sin(floor(uv.y / _MainTex_TexelSize.y) / 1 + (_Time * 80))) * _MainTex_TexelSize.x;
				fixed4 color = tex2D(_MainTex, final);
				return color;
			}

			half4 frag (v2f i) : COLOR
			{
				fixed4 c = tex2D(_MainTex, SineDisplace(i.texcoord)).rgba;
				if (any(c.rgb != half3(1,1,1))) {
					c.rgba *= _TintColor.rgba;
                }
                return c;
			}
			ENDCG 
		}
	} 	
	
	// ---- Dual texture cards
	SubShader {
		Pass {
			SetTexture [_MainTex] {
				constantColor [_TintColor]
				combine constant * primary
			}
			SetTexture [_MainTex] {
				combine texture * previous DOUBLE
			}
		}
	}
	
	// ---- Single texture cards (does not do color tint)
	SubShader {
		Pass {
			SetTexture [_MainTex] {
				combine texture * primary
			}
		}
	}
}
}