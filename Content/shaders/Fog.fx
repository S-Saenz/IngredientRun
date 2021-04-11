#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;

float2 TextureDimensions;
float2 Movement; // the vector movement in this frame
float  Density;  // min value for drop to be drawn
float  Falloff;  // height/settling curve of fog

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

// returns corresponding pixel from sampler based on pixel world location
float4 GetPixel(const sampler2D sampl, float2 pos)
{
	return tex2Dgrad(sampl, float2(pos.x / TextureDimensions.x, pos.y / TextureDimensions.y), 1, 1);
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float2 loc = input.TextureCoordinates * TextureDimensions + Movement;
	if (loc.x >= TextureDimensions.x)
	{
		loc.x -= TextureDimensions.x;
	}
	if (loc.y >= TextureDimensions.y)
	{
		loc.y -= TextureDimensions.y;
	}
	float4 pix = GetPixel(SpriteTextureSampler, loc);

	float4 color = input.Color;
	float intensity = pow(loc.y / (TextureDimensions.y - 16), Falloff);
	color.a = (pix.r + Density * intensity) * intensity;

	return color;
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};