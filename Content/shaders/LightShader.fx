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
sampler SpriteTexture : register(s0);
float2 TextureDimensions;

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

// determines the amount of light from one area light that reaches fragment
float4 CalculateAreaLight(int light, float2 fragPos)
{
	float dist = distance(AreaLightPosition[light], fragPos);
	if (dist < AreaLightDistance[light])
	{
		return (AreaLightDistance[light] - dist) / AreaLightDistance[light]; // linear 0-1
	}
	return 0;
}

// determines the amount of light from one directional light that reaches fragment
float4 CalculateDirectionalLight(int light, float2 fragPos)
{
	// determine angle between vector from light to fragment and light direction
	float2 fragDir = fragPos - DirectionalLightPosition[light];
	float2 lightDir = DirectionalLightDirection[light];
	float dotp = dot(lightDir, fragDir);
	float det = lightDir.x * fragDir.y - lightDir.y * fragDir.x;
	float angle = atan2(det, dotp);	

	// check inside cone
	if (abs(angle) < DirectionalLightSpread[light] / 2)
	{
		// check within distance
		float dist = distance(DirectionalLightPosition[light], fragPos);
		if (dist < DirectionalLightDistance[light])
		{
			return (DirectionalLightDistance[light] - dist) / DirectionalLightDistance[light]; // linear 0-1
		}
	}
	return 0;
}

// fragment shader main
float4 MainPS(VertexShaderOutput input) : COLOR
{
	float texWidth, texHeight;
	float4 color = tex2D(SpriteTexture, input.TextureCoordinates); // sample color from sprite

	float light = 0;
	for (int i = 0; i < NumAreaLights && light < 1; ++i) // add all light inputs, stopping if full light is reached
	{
		light += CalculateAreaLight(i, 
			float2(input.TextureCoordinates.x * TextureDimensions.x, input.TextureCoordinates.y * TextureDimensions.y));
		if (light > 1)
		{
			light = 1;
		}
	}

	for (int i = 0; i < NumDirectionalLights && light < 1; ++i) // add all light inputs, stopping if full light is reached
	{
		light += CalculateDirectionalLight(i,
			float2(input.TextureCoordinates.x * TextureDimensions.x, input.TextureCoordinates.y * TextureDimensions.y));
		if (light > 1)
		{
			light = 1;
		}
	}

	// darken rgb values based on light calculated
	color.a = 1 - light;
	return color;
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};