float hash(float2 p) {
    return frac(sin(dot(p, fixed2(12.9898, 78.233))) * 43758.5453);
}

fixed2 hash2(float n) {
    return frac(sin(fixed2(n,n+1.0))*fixed2(13.5453123,31.1459123));
}

float valueNoise(float2 p) {
    fixed2 i = floor(p);
    fixed2 f = frac(p);
    
    f = f*f*(3.0 - 2.0*f);
    
    float bottomOfGrid =    lerp( hash( i + fixed2( 0.0, 0.0 ) ), hash( i + fixed2( 1.0, 0.0 ) ), f.x );
    float topOfGrid =       lerp( hash( i + fixed2( 0.0, 1.0 ) ), hash( i + fixed2( 1.0, 1.0 ) ), f.x );

    float t = lerp( bottomOfGrid, topOfGrid, f.y );
    
    return t;
}

float fbm( float2 uv )
{
    float sum = 0.00;
    float amp = 0.7;
    
    for( int i = 0; i < 4; ++i )
    {
        sum += valueNoise( uv ) * amp;
        uv += uv * 1.2;
        amp *= 0.4;
    }
    
    return sum;
}

int compareColor(fixed4 c1, fixed4 c2, float tolerance) {
    half3 delta = abs(c1.rgb - c2.rgb);
    return length(delta) < tolerance ? 1 : 0;
}