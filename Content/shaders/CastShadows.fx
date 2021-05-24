#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// Constant limits
#define MAX_AREA_LIGHTS 30
#define MAX_DIRECTIONAL_LIGHTS 10
#define DIRECT_SPREAD 1.2 // 0-2, with 0 being an ellipse and 2 being a triangle

// Performance variable
bool Occlusion;

// Area light information
float2  AreaLightPosition[MAX_AREA_LIGHTS];
float   AreaLightDistance[MAX_AREA_LIGHTS];
float   AreaLightFalloff[MAX_AREA_LIGHTS];
float   AreaLightColor[MAX_AREA_LIGHTS];
int     NumAreaLights;

// Directional light information
float2   DirectionalLightPosition[MAX_DIRECTIONAL_LIGHTS];
float    DirectionalLightDistance[MAX_DIRECTIONAL_LIGHTS];
float    DirectionalLightFalloff[MAX_DIRECTIONAL_LIGHTS];
float2   DirectionalLightDirection[MAX_DIRECTIONAL_LIGHTS];
float    DirectionalLightSpread[MAX_DIRECTIONAL_LIGHTS];
float    DirectionalLightColor[MAX_DIRECTIONAL_LIGHTS];
int      NumDirectionalLights;

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
float IsBlocked(int2 lightPos, int2 fragPos)
{
	// Supercover lines: https://www.redblobgames.com/grids/line-drawing.html
	int2 delta = int2(fragPos.x - lightPos.x, fragPos.y - lightPos.y);
	int2 n = int2(abs(delta.x), abs(delta.y));
	int2 sign = int2(delta.x > 0 ? 1 : -1, delta.y > 0 ? 1 : -1);
	int2 curr = int2(lightPos.x, lightPos.y);

	int2 entryPoint = int2(lightPos.x, lightPos.y);
	bool lastAlpha = 0;
	bool isInside = false;

	float blockAmount = 0;

	const float wallBlock = 0.15;
	
	int2 i = int2(0, 0);
	[loop] for (; i.x < n.x || i.y < n.y;)
	{
		float4 pixelData = GetPixel(CasterTextureSampler, curr);
		
		// test if on rising edge (entering obstruction)
		if (pixelData.a > 0 && !isInside)
		{
			entryPoint = int2(curr.x, curr.y);
			isInside = true;
		}
		else if ( isInside && lastAlpha > 0) // test if on falling edge (exiting obstruction)
		{
			blockAmount += distance(entryPoint, curr) * wallBlock;
			isInside = false;
		}

		if (blockAmount > 1)
			return 1;

		// take next step toward fragPos
		int decision = (1 + 2 * i.x) * n.y - (1 + 2 * i.y) * n.x;
		if (decision < 0) // horizontal
		{
			curr.x += sign.x;
			i.x += 1;
		}
		else // vertical
		{
			curr.y += sign.y;
			i.y += 1;
		}

		// save alpha data
		lastAlpha = pixelData.a;
	}
	// continue loop if end not actually reached yet
	[loop] for (; i.x < n.x || i.y < n.y;)
	{
		float4 pixelData = GetPixel(CasterTextureSampler, curr);

		// test if on rising edge (entering obstruction)
		if (pixelData.a > 0 && !isInside)
		{
			entryPoint = int2(curr.x, curr.y);
			isInside = true;
		}
		else if (isInside && lastAlpha > 0) // test if on falling edge (exiting obstruction)
		{
			blockAmount += distance(entryPoint, curr) * wallBlock;
			isInside = false;
		}

		if (blockAmount > 1)
			return 1;

		// take next step toward fragPos
		int decision = (1 + 2 * i.x) * n.y - (1 + 2 * i.y) * n.x;
		if (decision < 0) // horizontal
		{
			curr.x += sign.x;
			i.x += 1;
		}
		else // vertical
		{
			curr.y += sign.y;
			i.y += 1;
		}

		// save alpha data
		lastAlpha = pixelData.a;
	}

	float4 pixelData = GetPixel(CasterTextureSampler, curr);
	// test last position for obstruction
	if (isInside) // ending inside
	{
		blockAmount += distance(entryPoint, curr) * wallBlock;
	}

	return blockAmount;
}

// determines the amount of light from one area light that reaches fragment
float4 CalculateAreaLight(int light, float2 fragPos)
{
	float dist = distance(AreaLightPosition[light], fragPos);
	if (dist < AreaLightDistance[light])
	{
		float blockVal = 0;
		if (Occlusion == true)
		{
			blockVal = IsBlocked(AreaLightPosition[light], fragPos);
		}

		if (true)
		{
			// TODO: falloff stuff
			float distValue = pow((AreaLightDistance[light] - dist) / AreaLightDistance[light], AreaLightFalloff[light]); // exponential curve (0-3)
			return distValue * (1 - blockVal);
		}
	}
	return 0;
}

// determines the amount of light from one directional light that reaches fragment
float4 CalculateDirectionalLight(int light, float2 fragPos)
{
	// Egg curves: http://www.mathematische-basteleien.de/eggcurves.htm
	float2 lightPos = DirectionalLightPosition[light];
	float2 lightDir = DirectionalLightDirection[light];
	float lightAngle = atan2(lightDir.y, lightDir.x);

	float a = DirectionalLightDistance[light] / 2; // half length of ellipse
	float b = DirectionalLightSpread[light] / 2; // half width of ellipse
	float c = sqrt(a * a - b * b); // distance from center to foci
	float2x2 rotateTranslation = float2x2(cos(-lightAngle), -sin(-lightAngle), sin(-lightAngle), cos(-lightAngle)); // translation matrix to align world pos with ellipse
	float2 tFragPos = mul(rotateTranslation, fragPos - lightPos) - float2(c,0); // translated frag position, proper relation to ellipse without rotation

	// check if fragment outside of light max area
	if (abs(tFragPos.x) > a)
	{
		return 0;
	}

	// check if outside shape
	float r = pow(tFragPos.x, 2) / (a * a) + pow(tFragPos.y, 2) / (b * b) *
		((1 - DIRECT_SPREAD * (tFragPos.x / DirectionalLightDistance[light])) / (1 + DIRECT_SPREAD * (tFragPos.x / DirectionalLightDistance[light])));
	if (r > 1)
	{
		return 0;
	}

	float blockVal = 0;
	if (Occlusion == true)
	{
		blockVal = IsBlocked(DirectionalLightPosition[light], fragPos); // amount of light blocked between lightPos and fragPos
	}

	float fragAngle = atan2(tFragPos.y, tFragPos.x);
	float2 farPoint = float2(a * cos(fragAngle), b * sin(fragAngle));
	float maxDist = distance(float2(-c, 0), farPoint);

	float fragDist = distance(lightPos, fragPos);
	float fromCenterValue = 1 - r; // exponential curve (0-1)
	float fromSourceValue = tFragPos.x < -c ? ((tFragPos.x + a) / (a - c)) : ((a - tFragPos.x) / (a + c));
	return pow(fromCenterValue * fromSourceValue, DirectionalLightFalloff[light]) * (1 - blockVal);

	// determine angle between vector from light to fragment and light direction
	// float2 fragDir = fragPos - DirectionalLightPosition[light];
	// float2 lightDir = DirectionalLightDirection[light];
	// float dotp = dot(lightDir, fragDir);
	// float det = lightDir.x * fragDir.y - lightDir.y * fragDir.x;
	// float angle = atan2(det, dotp);	
	// 
	// // check inside cone
	// if (abs(angle) < DirectionalLightSpread[light] / 2)
	// {
	// 	// check within distance
	// 	float dist = distance(DirectionalLightPosition[light], fragPos);
	// 	if (dist < DirectionalLightDistance[light])
	// 	{
	// 		float blockVal = IsBlocked(DirectionalLightPosition[light], fragPos);
	// 		if (true)
	// 		{
	// 			// TODO: falloff stuff
	// 			float distValue = pow(((DirectionalLightDistance[light] - dist) / DirectionalLightDistance[light]), FALLOFF_EXP); // exponential curve (0-3)
	// 			float angleValue = pow((DirectionalLightSpread[light] - abs(angle) * 2) / DirectionalLightSpread[light], FALLOFF_EXP); // exponential curve (0-3)
	// 			return (distValue * angleValue) * (1 - blockVal);
	// 		}
	// 	}
	// }
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