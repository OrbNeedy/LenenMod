sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float3 uColor;
float3 uSecondaryColor;
float uOpacity;
float uSaturation;
float uRotation;
float uTime;
float4 uSourceRect;
float2 uWorldPosition;
float uDirection;
float3 uLightSource;
float2 uImageSize0;
float2 uImageSize1;
float2 uTargetPosition;
float4 uLegacyArmorSourceRect;
float2 uLegacyArmorSheetSize;

/*
// Normalized pixel coordinates (from 0 to 1)
vec2 uv = fragCoord/iResolution.xy;

// Time varying pixel color
float rTime = iTime * 2.;
float timeX = 0.325 * (rTime - fract(rTime));
float timeY = 0.875 * (rTime - fract(rTime));
vec4 tex = texture(iChannel0, uv);
vec4 tex2 = texture(iChannel1, uv + vec2(timeX, timeY));
float origAvrg = tex.r + tex.g + tex.b;
origAvrg /= 3.0;
float shadow = (smoothstep(0.0, 1.0, origAvrg * 1.4));

// Output to screen
fragColor = tex2 * shadow;*/

float4 Texture(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    // Normalization
    float2 uv = (coords * uImageSize0 - uSourceRect.xy) / uSourceRect.wz;
    float4 source = tex2D(uImage0, coords);
    float4 mask = tex2D(uImage1, uv);
    
    // Offset that moves
    float rTime = uTime * 2.0;
    float sFrac = (rTime - frac(rTime));
    float timeX = 0.325 * sFrac;
    float timeY = 0.875 * sFrac;
    
    float origAvrg = source.r + source.g + source.b;
    
    float shadow = (smoothstep(0.0, 1.0, origAvrg * 1.0));
    float transparency = source.a;
    
    return float4(mask.rgb * sampleColor.rgb * shadow, transparency) * transparency;
}
    
technique Technique1
{
    pass Texture
    {
        PixelShader = compile ps_3_0 Texture();
    }
}