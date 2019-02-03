// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "FX/Mirror"
{
	Properties
	{
		_MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		[HideInInspector] _ReflectionTex ("", 2D) = "white" {}
	}
	SubShader
	{
		Tags  { 
			"Queue"="Transparent"
			"RenderType"="Transparent" 
		}
		LOD 100
		Blend SrcAlpha OneMinusSrcAlpha
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct appdata_t
			{
				float2 uv : TEXCOORD0;
				float4 refl : TEXCOORD1;
				float4 pos : POSITION;
				float4 col: COLOR;
			};
			struct v2f
			{
				half2 uv : TEXCOORD0;
				float4 refl : TEXCOORD1;
				float4 pos : SV_POSITION;
				fixed4 col: COLOR;
			};
			float4 _MainTex_ST;
			fixed4 _Color;
			v2f vert(appdata_t i)
			{
				v2f o;
				o.pos = UnityObjectToClipPos (i.pos);
				o.uv = TRANSFORM_TEX(i.uv, _MainTex);
				o.refl = ComputeScreenPos (o.pos);
				o.col = i.col * _Color;
				return o;
			}
			
			sampler2D _MainTex;
			sampler2D _ReflectionTex;
			float4 _MainTex_TexelSize;

			fixed4 SineDisplace(sampler2D _reflTex, float2 uv)
			{
				// poor man's Fresnel effect!
				float normY  = -(uv.y - _MainTex_TexelSize);
				// distort more towards the bottom of screen
				uv.x += pow(normY/30, 2) * sin(500*((normY)));
				// also fade to black more towards the bottom
				fixed4 color = tex2D (_reflTex, uv);
				//color.rgb *= (uv.y / _MainTex_TexelSize);
				color.rgb = lerp(color.rgb, _Color, normY / _MainTex_TexelSize);

				return color;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				//get the base reflector texture details
				fixed4 tex = tex2D(_MainTex, i.uv) * i.col;
				tex.rgb *= tex.a;
				//sample the 2d texture from the projected coordinates of the reflection
				fixed4 refl = SineDisplace(_ReflectionTex, UNITY_PROJ_COORD(i.refl));
				fixed4 final = refl * tex;
				return final;
			}
			ENDCG
	    }
	}
}