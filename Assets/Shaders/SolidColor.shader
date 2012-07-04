Shader "SolidColor" 
{
    Properties 
    {
        _Color ("Main Color", Color) = (1,.5,.5,1)
    }
    SubShader 
    {
        Pass 
        {
            Color [_Color]
        }
    }
}