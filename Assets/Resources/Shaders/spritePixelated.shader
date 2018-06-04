
//
// shader starts here, c# companion script below
//
// PS. Make sure to use square texture (sides must be equal)
//
 
Shader "Unlit/spritePixelated"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
       
        [Header(Scaling)]
        _Res ("Resolution", Float) = 1024
        _PixelSize ("Pixel Size", Float) = .0625
       
        [Header(Sprite MetaData)]
        _SpriteUV ("Sprite Rect", Vector) = (1,1,0,0)
        _SpritePivot ("Sprite Pivot", Vector) = (1,1,0,0)
        _UVCenter ("_UVCenter", Vector) = (0,0,0,0)
       
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"
        "DisableBatching"="True"
        }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
 
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
           
            #include "UnityCG.cginc"
 
            uniform half _Res, _PixelSize;
            uniform half4 _SpriteUV, _SpritePivot, _UVCenter;
 
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
 
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
 
            sampler2D _MainTex;
            float4 _MainTex_ST;
       
            float2 quant(float2 q, float2 v){
                return floor(q/v)*v;
            }
       
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
           
            float2 quantToWorld(float2 value, float q){
                float2 wp = mul(unity_ObjectToWorld, float4(value,0,0) );
                wp = quant(wp, q) ;
                return mul(unity_WorldToObject, float4(wp,0,0));
            }
           
            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
               // next line is the pixelation
                uv = quantToWorld(uv-_UVCenter.xy,  1/_Res)+_UVCenter.xy;
               
                fixed4 col = tex2D(_MainTex, uv);
                clip(col.a-.001);
                return col;
            }
            ENDCG
        }
    }
}