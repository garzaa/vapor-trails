Shader "Unlit/TextureOffset"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)

		[Header(Properties)]
		_XSpeed ("X Speed", Float) = 1
		_YSpeed ("Y Speed", Float) = 1
		_YDisplacment ("Y Displacement", Float) = 1
	}
	SubShader
	{
		Tags { 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex   : POSITION;
				float2 uv 		: TEXCOORD0;
				float4 color    : COLOR;
			};

			struct v2f
			{
				float2 uv 		: TEXCOORD0;
				fixed4 color    : COLOR;
				float4 vertex   : SV_POSITION;
			};

			sampler2D _MainTex;
			fixed4 _Color;

			float _XSpeed;
			float _YSpeed;
			float4 _MainTex_ST;
			float4 _MainTex_TexelSize;
			float _YDisplacment;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.color = v.color * _Color;
				return o;
			}
			
			fixed4 offset(float2 uv) {
				float2 final = uv;
				final.y = (uv.y + floor(_Time.w * _YSpeed) * _MainTex_TexelSize.y) % _MainTex_TexelSize.z;
				final.y += floor(_YDisplacment * sin(floor(uv.y / _MainTex_TexelSize.y) / 1 + (_Time * 10))) * _MainTex_TexelSize.y;
				return tex2D(_MainTex, final);
			}

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = offset(i.uv);
				col.rgb *= col.a;
				col.a *= _Color.a;
				return col;
			}
			ENDCG
		}
	}
}
