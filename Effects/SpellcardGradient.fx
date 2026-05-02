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
    
float4 Gradient(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    // Normalization
	float2 uv = (coords * uImageSize0 - uSourceRect.xy) / uSourceRect.zw;
    //float frameX = (coords.y * uImageSize0.y - uSourceRect.y) / uSourceRect.w;
    float4 image = tex2D(uImage0, coords);

	// Position of the middle color
    float mid = 0.5;

    // Color mixing
    float mix1A = uv.x / mid;
    float mix2A = (uv.x - mid) / (1.0 - mid);
    float3 firstMix = lerp(leftColor, middleColor, mix1A);
    float3 secondMix = lerp(middleColor, rightColor, mix2A);
	float3 col = lerp(firstMix, secondMix, step(mid, uv.x));
    
    /*float md = uv.x + (1.0 / 3.0);
    float md2 = uv.x - (1.0 / 3.0);
    float3 firstMix = (leftColor * (1.0 - md)) + (middleColor * md);
    firstMix *= step(uv.x, 0.5);
    float3 secondMix = (rightColor * md2) + (middleColor * (1.0 - md2));
    secondMix *= step(0.5, uv.x);
    float3 col = firstMix + secondMix;*/
    
    /*float mix1A = uv.x / mid;
    float mix2A = (uv.x - mid) / (1.0 - mid);
    float3 firstMix = lerp(leftColor, middleColor, mix1A) * mid;
    firstMix *= step(uv.x, mid);
    float3 secondMix = lerp(middleColor, rightColor, mix2A);
    secondMix *= step(mid, uv.x);
    float3 col = firstMix + secondMix;*/
    
    return float4(col * sampleColor.a, image.a) * image.a * sampleColor.a;
}
    
technique Technique1
{
	pass Gradient
	{
		PixelShader = compile ps_3_0 Gradient();
	}
}