// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "FX/Mirror"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
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
				o.col = i.col * _Color;
				o.refl = ComputeScreenPos (o.pos);
				return o;
			}
			
			sampler2D _MainTex;
			sampler2D _ReflectionTex;
			float4 _MainTex_TexelSize;

			fixed4 SineDisplace (sampler2D _reflTex, float2 uv)
			{
				float2 final = uv;
				//		   AMP 						 [--- BUCKET WIDTH ----   ]			SPEED
				final.x += .0008 * (sin(floor(uv.y * _MainTex_TexelSize.z * 50) / 1 + (_Time * 100)));

				fixed4 color = tex2D (_reflTex, final);

				return color;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				//get the pattern provided by the texture on the reflector
				fixed4 tex = tex2D(_MainTex, i.uv) * i.col;
				//get opacity
				tex.rgb *= tex.a;
				//get the reflection surface?
				//sample the 2d texture from the projected coordinates of the reflection
				fixed4 refl = tex2D(_ReflectionTex, UNITY_PROJ_COORD(i.refl));
				fixed4 final = refl * tex;
				return final;
			}
			ENDCG
	    }
	}
}