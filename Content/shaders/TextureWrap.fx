#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D DestinationTexture;
Texture2D SourceTexture;

float2 DestinationDimensions;
float2 SourceDimensions;

float  Scale;
float2 Position;

sampler2D DestinationTextureSampler = sampler_state
{
	Texture = <DestinationTexture>;
};
sampler2D SourceTextureSampler = sampler_state
{
	Texture = <SourceTexture>;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float2 pixelLoc = float2(input.TextureCoordinates.x * DestinationDimensions.x + Position.x ,
		                      input.TextureCoordinates.y * DestinationDimensions.y + Position.y);
	if (pixelLoc.x < 0)
		pixelLoc.x += DestinationDimensions.x;
	else if (pixelLoc.x >= DestinationDimensions.x)
		pixelLoc.x -= DestinationDimensions.x;
	if (pixelLoc.y < 0)
		pixelLoc.y += DestinationDimensions.y;
	else if (pixelLoc.y >= DestinationDimensions.y)
		pixelLoc.y -= DestinationDimensions.y;

	float2 texLoc = float2(pixelLoc.x % (SourceDimensions.x * Scale) / (SourceDimensions.x * Scale),
				              pixelLoc.y % (SourceDimensions.y * Scale) / (SourceDimensions.y * Scale));
	return tex2D(SourceTextureSampler, texLoc) * input.Color;
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};