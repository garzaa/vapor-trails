Shader "Custom/FBMWater"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        [PerRendererData] _Color ("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [PerRendererData] _FlashColor ("Flash Color", Color) = (1,1,1,0)

        [Header(FBM)]
        _MaskColor ("Mask Color", Color) = (0, 0, 0, 1)
        col1 ("Color 1", Color) = (1, 0, 0, 1)
        col2 ("Color 2", Color) = (0, 1, 0, 1)
        col3 ("Color 3", Color) = (0, 0, 1, 1)
        col4 ("Color 4", Color) = (1, 0, 1, 1)
        
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
                float2 texcoord : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };

            struct ColorResult {
                float2 q;
                float2 r;
                float t;
            };
            
            fixed4 _Color;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color;
                OUT.worldPos = mul (unity_ObjectToWorld, IN.vertex);

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

            fixed4 _MaskColor;
            fixed4 col1;
            fixed4 col2;
            fixed4 col3;
            fixed4 col4;

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

            ColorResult fbmChain(in float2 uv) {

                uv.x += _Time.w * _XSpeed;
                uv.y += _Time.w * _YSpeed;

                float2 q = float2(
                  fbm( uv ),
                  fbm( uv + uv)
                );

                float2 r = float2( 
                  fbm( uv + 4.0*q + 5*sin(_Time/2.0)),
                  fbm( uv + 4.0*q));

                float t = fbm( uv + 4.0*(r-(_Time/2.0)));

                ColorResult result;
                result.q = q;
                result.r = r;
                result.t = t;
                
                return result;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                float2 uv = IN.texcoord;

                fixed4 color = tex2D(_MainTex, SineDisplace(uv));

                if (compareColor(color, _MaskColor, 0.1)) {
                  fixed2 worldPos = IN.worldPos.xy;
                  ColorResult r = fbmChain(worldPos * 3.0);
                  float4 fbmColor = lerp(
                    lerp(col1, col2, length(r.q)),
                    lerp(col3, col4, r.r.x),
                    r.t
                  );
                  color.rgb = fbmColor.rgb;
                }

                color.rgb = lerp(color.rgb,_FlashColor.rgb,_FlashColor.a);
                color.rgb *= color.a;

                return color;
            }
        ENDCG
        }
    }
}
