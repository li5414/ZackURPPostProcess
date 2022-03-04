#ifndef ZACK_URP_BAYER_DITHER_INCLUDE
#define ZACK_URP_BAYER_DITHER_INCLUDE

// Bayer Dither Texture
TEXTURE2D(_BayerDitherTex);
SAMPLER(sampler_BayerDitherTex);

// CBUFFER_START(UnityPerMaterial)
//     float4 _BayerDitherTex_TexelSize;	// x contains 1.0 / width; y contains 1.0 / height; z contains width; w contains height
// CBUFFER_END

void ClipByBayerDither(float alpha, float4 screenPos) 
{
	screenPos = frac(floor(screenPos) * _BayerDitherTex_TexelSize.x);
	clip(alpha - SAMPLE_TEXTURE2D(_BayerDitherTex, sampler_BayerDitherTex, screenPos).r);
}

#endif // ZACK_URP_BAYER_DITHER_INCLUDE