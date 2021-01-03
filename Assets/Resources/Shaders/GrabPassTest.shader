// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/GrabPassTest" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _BumpAmt  ("Distortion", Range (0,128)) = 10
        _MainTex ("Tint Color (RGB)", 2D) = "white" {}
        _BumpMap ("Normalmap", 2D) = "bump" {}
        _Size ("Size", Range(0, 20)) = 1
    }
 
    Category {
 
        // We must be transparent, so other objects are drawn before this one.
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Opaque" }
 
 
        SubShader {
     
            // Horizontal blur
            GrabPass {                    
                Tags { "LightMode" = "Always" }
            }
            Pass {
                Tags { "LightMode" = "Always" }
             
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma fragmentoption ARB_precision_hint_fastest
                #include "UnityCG.cginc"
             
                struct appdata_t {
                    float4 vertex : POSITION;
                    float2 texcoord: TEXCOORD0;
                };
             
                struct v2f {
                    float4 vertex : POSITION;
                    float4 uvgrab : TEXCOORD0;
                };
             
                v2f vert (appdata_t v) {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    #if UNITY_UV_STARTS_AT_TOP
                    float scale = -1.0;
                    #else
                    float scale = 1.0;
                    #endif
                    o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
                    o.uvgrab.zw = o.vertex.zw;
                    return o;
                }
             
                sampler2D _GrabTexture;
                float4 _GrabTexture_TexelSize;
                float4 _MainTex_TexelSize;
                float _Size;
             
                half4 frag( v2f i ) : COLOR {
                    // uvgrab is the screen position
                    // the 0.4 value has to change
                    // dependent on the material position relative to the camera
                    // can this be done at all?
                    i.uvgrab.y -= 0.4;

                    // https://docs.unity3d.com/Manual/SL-UnityShaderVariables.html
                    // https://github.com/gamedevserj/Shader-Graph-Experiments
                    // Offset.Y = CameraPosition.Y * (-1/cameraOrthographicSize) + ObjectPosition.Y * (1/cameraOrthographicSize)


                    // flip the texture
                    i.uvgrab.y = 1 - i.uvgrab.y;

                    //but the pivot is still the entire screen
                    // we want to pivot off the top of the texture
                    //i.uvgrab.y += i.uvgrab.y*_GrabTexture_TexelSize.y;

                    half4 color = tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(i.uvgrab));
                    return color;
                }
                ENDCG
            }
         
            // Distortion
            GrabPass {                        
                Tags { "LightMode" = "Always" }
            }
            Pass {
                Tags { "LightMode" = "Always" }
             
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma fragmentoption ARB_precision_hint_fastest
                #include "UnityCG.cginc"
             
                struct appdata_t {
                    float4 vertex : POSITION;
                    float2 texcoord: TEXCOORD0;
                };
             
                struct v2f {
                    float4 vertex : POSITION;
                    float4 uvgrab : TEXCOORD0;
                    float2 uvbump : TEXCOORD1;
                    float2 uvmain : TEXCOORD2;
                };
             
                float _BumpAmt;
                float4 _BumpMap_ST;
                float4 _MainTex_ST;
             
                v2f vert (appdata_t v) {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    #if UNITY_UV_STARTS_AT_TOP
                    float scale = -1.0;
                    #else
                    float scale = 1.0;
                    #endif
                    o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
                    o.uvgrab.zw = o.vertex.zw;
                    o.uvbump = TRANSFORM_TEX( v.texcoord, _BumpMap );
                    o.uvmain = TRANSFORM_TEX( v.texcoord, _MainTex );
                    return o;
                }
             
                fixed4 _Color;
                sampler2D _GrabTexture;
                float4 _GrabTexture_TexelSize;
                sampler2D _BumpMap;
                sampler2D _MainTex;
             
                half4 frag( v2f i ) : COLOR {
                    // calculate perturbed coordinates
                    half2 bump = UnpackNormal(tex2D( _BumpMap, i.uvbump )).rg; // we could optimize this by just reading the x  y without reconstructing the Z
                    float2 offset = bump * _BumpAmt * _GrabTexture_TexelSize.xy;
                    i.uvgrab.xy = offset * i.uvgrab.z + i.uvgrab.xy;
                                     
                    half4 color = tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(i.uvgrab));
                    half4 tint = tex2D( _MainTex, i.uvmain ) * _Color;
                 
                    return color * tint;
                }
                ENDCG
            }
        }
    }
}
