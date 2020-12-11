Shader "Custom/TransparentWaterfall" {
    Properties {
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}

        _Color     ("Main Color", Color) = (1,1,1,1)
        _BumpAmt   ("Distortion", Range(0, 128)) = 10
        _BumpMap   ("Normal Map", 2D) = "bump" {}
        _BumpScale ("Normal Map Scale", Vector) = (1,1,1,1)
        _MoveSpeed ("Waterfall Speed", Vector) = (0,0,0,0)
    }

    // persist top-level tags across sub-shaders
    Category {

        Tags {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Opaque"
            "LightMode" = "Always"
        }

        SubShader {
            GrabPass {}

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

            Pass {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma fragmentoption ARB_precision_hint_fastest
                #include "UnityCG.cginc"

                struct appdata_t {
                    float4 vertex : POSITION;
                    float2 texcoord : TEXCOORD0;
                };
             
                struct v2f {
                    float4 vertex : POSITION;
                    float4 uvgrab : TEXCOORD0;
                    float2 uvbump : TEXCOORD1;
                    float2 uvmain : TEXCOORD2;
                };

                float _BumpAmt;
                float4 _BumpScale;
                float4 _BumpMap_ST;
                float4 _MainTex_ST;
                float4 _MoveSpeed;

                v2f vert(appdata_t v) {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uvgrab = ComputeGrabScreenPos(o.vertex);
                    
                    o.uvbump = TRANSFORM_TEX(v.texcoord, _BumpMap) * _BumpScale + (_Time.w * _MoveSpeed.xy);
                    o.uvmain = TRANSFORM_TEX(v.texcoord, _MainTex);

                    return o;
                }

                fixed4 _Color;
                sampler2D _GrabTexture;
                float4 _GrabTexture_TexelSize;
                sampler2D _BumpMap;
                sampler2D _MainTex;

                half4 frag(v2f i) : COLOR {
                    half2 bump = UnpackNormal(tex2D(_BumpMap, i.uvbump)).rg;
                    float2 offset = bump * _BumpAmt * _GrabTexture_TexelSize.xy;
                    i.uvgrab.xy = offset * i.uvgrab.z + i.uvgrab.xy;

                    half4 grabPixel = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(i.uvgrab));
                    half4 texPixel = tex2D(_MainTex, i.uvmain) * _Color;

                    return grabPixel;

                    // return grabPixel * texPixel;
                }

                ENDCG
            }
        }
    }
}
