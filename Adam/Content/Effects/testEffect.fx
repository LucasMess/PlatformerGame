// Pixel shader applies a one dimensional gaussian blur filter.
// This is used twice by the bloom postprocess, first to
// blur horizontally, and then again to blur vertically.

sampler TextureSampler : register(s0);

#define SAMPLE_COUNT 15

float2 SampleOffsets[SAMPLE_COUNT];
float SampleWeights[SAMPLE_COUNT];

Texture2D InputTexture; // The distortion map.
Texture2D LastTexture; // The actual rendered scene.

sampler inputTexture = sampler_state
{
	texture = <InputTexture>;
	magFilter = POINT;
	minFilter = POINT;
	mipFilter = POINT;
	addressU = CLAMP;
	addressV = CLAMP;
};

sampler lastTexture = sampler_state
{
	texture = <LastTexture>;
	magFilter = POINT;
	minFilter = POINT;
	mipFilter = POINT;
	addressU = CLAMP;
	addressV = CLAMP;
};

float4 PixelShaderF(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 texCoord : TEXCOORD0) : COLOR0
{

	float4 colorA;

	float4 right;
	float4 up;

	float4 color2;
	float2 coords;
	float mul;

	coords = texCoord;
	colorA = tex2D(inputTexture, coords);
	right = tex2D(inputTexture, coords + float2(1, 0));
	up = tex2D(inputTexture, coords + float2(0, 1));

	// 0.1 seems to work nicely.
	mul = (colorA.b * .1 * colorA.a);

	float diff = mul / 8;
	//diff = (colorA.g * mul) - mul / 2;

	if (up.a < colorA.a) 
	{
		coords.y += diff;
	}
	else if (up.a > colorA.a) {
		coords.y -= diff;
	}
	else {

	}

	if (right.a < colorA.a) {
		coords.x += diff;
	}
	else if (right.a > colorA.a) {
		coords.x -= diff;
	}
	else {

	}
	color2 = tex2D(lastTexture, coords);

	return color2;
}


technique GaussianBlur
{
	pass Pass1
	{
#if SM4
		PixelShader = compile ps_4_0_level_9_1 PixelShaderF();
#elif SM3
		PixelShader = compile ps_3_0 PixelShaderF();
#else
		PixelShader = compile ps_2_0 PixelShaderF();
#endif
	}
}