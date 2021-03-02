#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

#define MAX_AREA_LIGHTS 10
#define MAX_DIRECTIONAL_LIGHTS 10

float2  AreaLightPosition[MAX_AREA_LIGHTS];
float   AreaLightDistance[MAX_AREA_LIGHTS];
float   AreaLightColor[MAX_AREA_LIGHTS];
int     NumAreaLights;

float2  DirectionalLightPosition[MAX_DIRECTIONAL_LIGHTS];
float   DirectionalLightDistance[MAX_DIRECTIONAL_LIGHTS];
float   DirectionalLightColor[MAX_DIRECTIONAL_LIGHTS];
int     NumDirectionalLights;

float2 lightPos;
float lightDist;

Texture2D SpriteTexture;
float2 TextureDimensions;
sampler s0;
float4x4 viewMatrix;
float4x4 projMatrix;

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

float4 CalculateLight(float2 lightPos, float lightDist, float2 fragPos)
{

	float dist = distance(lightPos, fragPos);
	if (dist < lightDist)
	{
		float val = (lightDist - dist) / lightDist;
		return float4(val, val, val, 1);
	}
	return float4(0, 0, 0, 1);
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float4 color = tex2D(s0, input.TextureCoordinates);
	color *= CalculateLight(lightPos, lightDist, float2(input.TextureCoordinates.x * TextureDimensions.x, input.TextureCoordinates.y * TextureDimensions.y));
	return color;
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};