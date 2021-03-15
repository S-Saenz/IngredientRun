﻿#if OPENGL
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
Texture2D CasterTexture;
float2 TextureDimensions;

sampler2D CasterTextureSampler = sampler_state
{
	Texture = <CasterTexture>;
};

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

// returns corresponding pixel from sampler based on pixel world location
float4 GetPixel(const sampler2D sampl, float2 pos)
{
	return tex2Dgrad(sampl, float2(pos.x / TextureDimensions.x, pos.y / TextureDimensions.y), 1, 1);
}

// determine if fragment is blocked from light
bool IsBlocked(int2 lightPos, int2 fragPos)
{
	// Supercover lines: https://www.redblobgames.com/grids/line-drawing.html
	int2 delta = int2(fragPos.x - lightPos.x, fragPos.y - lightPos.y);
	int2 n = int2(abs(delta.x), abs(delta.y));
	int2 sign = int2(delta.x > 0 ? 1 : -1, delta.y > 0 ? 1 : -1);
	int2 curr = int2(lightPos.x, lightPos.y);
	
	for (int2 i = int2(0, 0); i.x < n.x || i.y < n.y;)
	{
		// test current position for obstruction
		if (GetPixel(CasterTextureSampler, curr).a > 0)
		{
			return true;
		}

		// take next step toward fragPos
		int decision = (1 + 2 * i.x) * n.y - (1 + 2 * i.y) * n.x;
		if (decision == 0) // diagonal
		{
			curr.x += sign.x;
			curr.y += sign.y;
			++i.x;
			++i.y;
		}
		else if (decision < 0) // horizontal
		{
			curr.x += sign.x;
			++i.x;
		}
		else // vertical
		{
			curr.y += sign.y;
			++i.y;
		}
	}
	// test last position for obstruction
	if (GetPixel(CasterTextureSampler, curr).a > 0)
	{
		return true;
	}
	return false;

	// Bresenham's algorithm line variables
	// int2 delta = int2(abs(fragPos.x - lightPos.x), abs(fragPos.y - lightPos.y));
	// int2 sign = int2(lightPos.x < fragPos.x ? 1 : -1, lightPos.y < fragPos.y ? 1 : -1);
	// int2 err = int2(2 * delta.x - delta.y, 2 * delta.y - delta.x);
	// 
	// step along path until fragment reached or obstruction hit
	// for(int2 currPos = lightPos; currPos.x != fragPos.x && currPos.y != fragPos.y;)
	// {
	// 	// test current position for obstruction
	// 	if (GetPixel(CasterTextureSampler, currPos).a > 0)
	// 	{
	// 		return true;
	// 	}
	// 	
	// 	if (err.y > 0)
	// 	{
	// 		currPos.y += sign.y;
	// 		err.y -= 2 * delta.x;
	// 	}
	// 
	// 	if (err.x > 0)
	// 	{
	// 		currPos.x += sign.x;
	// 		err.x -= 2 * delta.y;
	// 	}
	// 
	// 	err += int2(2 * delta.x, 2 * delta.y);
	// }
	// 
	// if (GetPixel(CasterTextureSampler, fragPos).a > 0)
	// {
	// 	return true;
	// }
	// return false;
}

// determines the amount of light from one area light that reaches fragment
float4 CalculateAreaLight(int light, float2 fragPos)
{
	float dist = distance(AreaLightPosition[light], fragPos);
	if (dist < AreaLightDistance[light])
	{
		if (!IsBlocked(AreaLightPosition[light], fragPos))
		{
			// TODO: falloff stuff
			float distValue = (AreaLightDistance[light] - dist) / AreaLightDistance[light]; // linear 0-1
			return distValue *.9;
		}
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
			if (!IsBlocked(DirectionalLightPosition[light], fragPos))
			{
				// TODO: falloff stuff
				float distValue = ((DirectionalLightDistance[light] - dist) / DirectionalLightDistance[light]) *3; // linear 0-1
				float angleValue = (DirectionalLightSpread[light] - abs(angle) * 3) / DirectionalLightSpread[light] *0.4 + 0.2; // linear 0-1
				return (distValue * angleValue);
			}
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
	color.a -= light;
	return color;
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};