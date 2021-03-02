#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// Constant limits
#define MAX_AREA_LIGHTS 10
#define MAX_DIRECTIONAL_LIGHTS 10

// Area light information
float2  AreaLightPosition[MAX_AREA_LIGHTS];
float   AreaLightDistance[MAX_AREA_LIGHTS];
float   AreaLightColor[MAX_AREA_LIGHTS];
int     NumAreaLights;

// Directional light information
float2  DirectionalLightPosition[MAX_DIRECTIONAL_LIGHTS];
float   DirectionalLightDistance[MAX_DIRECTIONAL_LIGHTS];
float2  DirectionalLightDirection[MAX_DIRECTIONAL_LIGHTS];
float   DirectionalLightSpread[MAX_DIRECTIONAL_LIGHTS];
float   DirectionalLightColor[MAX_DIRECTIONAL_LIGHTS];
int     NumDirectionalLights;

// Sprite texture parameters (texture that effect is called on in draw, passed in automatically)
Texture2D SpriteTexture;
sampler s0;
float2 TextureDimensions; //  must be set manually in code, can't find sprite texture size in hlsl

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

// using default vertex shader, these are the parameters it passes back
struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

// determines the amount of light from one light that reaches fragment
float4 CalculateLight(float2 lightPos, float lightDist, float2 fragPos)
{
	float dist = distance(lightPos, fragPos);
	if (dist < lightDist)
	{
		return (lightDist - dist) / lightDist; // linear 0-1
	}
	return 0;
}

// fragment shader main
float4 MainPS(VertexShaderOutput input) : COLOR
{
	float4 color = tex2D(s0, input.TextureCoordinates); // sample color from sprite

	float light = 0;
	for (int i = 0; i < NumAreaLights && light < 1; ++i) // add all light inputs, stopping if full light is reached
	{
		light += CalculateLight(AreaLightPosition[i], AreaLightDistance[i], 
			float2(input.TextureCoordinates.x * TextureDimensions.x, input.TextureCoordinates.y * TextureDimensions.y));
		if (light > 1)
		{
			light = 1;
		}
	}

	// darken rgb values based on light calculated
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