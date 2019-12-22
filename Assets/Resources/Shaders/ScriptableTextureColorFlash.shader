// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

 Shader "Sprites/ScriptableTextureColorFlash"
 {
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        [PerRendererData] _Color ("Tint", Color) = (1,1,1,0.5)
        [PerRendererData] _FlashColor ("Flash Color", Color) = (1,1,1,1)
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
        Fog { Mode Off }
        Blend One OneMinusSrcAlpha

        Pass
        {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile DUMMY PIXELSNAP_ON
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
                half2 texcoord  : TEXCOORD0;
            };
            
            fixed4 _Color;
            fixed4 _FlashColor;
            float _FlashAmount;
            float4 _UVCenter = (0,0,0,0);
            float2 _Res = 64;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color;
                return OUT;
            }

            sampler2D _MainTex;

            float2 quant(float2 q, float2 v){
                return floor(q/v)*v;
            }

            float2 quantToWorld(float2 value, float q){
                float2 wp = mul(unity_ObjectToWorld, float4(value,0,0) );
                wp = quant(wp, q) ;
                return mul(unity_WorldToObject, float4(wp,0,0));
            }


            fixed4 frag(v2f IN) : COLOR
            {
                float2 uv = IN.texcoord;
                uv = quantToWorld(uv-_UVCenter.xy,  1/_Res)+_UVCenter.xy;
                fixed4 c = tex2D(_MainTex, uv) * IN.color;
                c.rgb = lerp(c.rgb,_FlashColor.rgb,_FlashColor.a);
                c.rgb *= c.a;
            
                return c;
            }
        ENDCG
        }
    }
}