float2 SineDisplace (float2 uv) {
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