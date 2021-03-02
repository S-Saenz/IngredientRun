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
float2  DirectionalLightDirection[MAX_DIRECTIONAL_LIGHTS];
float   DirectionalLightSpread[MAX_DIRECTIONAL_LIGHTS];
float   DirectionalLightColor[MAX_DIRECTIONAL_LIGHTS];
int     NumDirectionalLights;

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

float CalculateLight(float2 lightPos, float lightDist, float2 fragPos)
{

	float dist = distance(lightPos, fragPos);
	if (dist < lightDist)
	{
		return (lightDist - dist) / lightDist;
	}
	return 0;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float4 color = tex2D(s0, input.TextureCoordinates);

	float light = 0;
	for (int i = 0; i < NumAreaLights && light < 1; ++i)
	{
		light += CalculateLight(AreaLightPosition[i], AreaLightDistance[i], 
			float2(input.TextureCoordinates.x * TextureDimensions.x, input.TextureCoordinates.y * TextureDimensions.y));
		if (light > 1)
		{
			light = 1;
		}
	}
	color.rgb *= light;
	return color;
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};