sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float3 uColor;
float3 uSecondaryColor;

float3 leftColor;
float3 middleColor;
float3 rightColor;

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
    
float4 Solid(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    // Normalization
    float2 uv = (coords * uImageSize0 - uSourceRect.xy) / uSourceRect.zw;
    //float frameX = (coords.y * uImageSize0.y - uSourceRect.y) / uSourceRect.w;
    float4 image = tex2D(uImage0, coords);
    
    return float4(uColor * sampleColor.a, image.a) * image.a * sampleColor.a * uOpacity;
}
    
technique Technique1
{
    pass Solid
    {
        PixelShader = compile ps_3_0 Solid();
    }
}