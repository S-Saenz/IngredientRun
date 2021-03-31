#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

//sampler SpriteTexture : register(s0);
Texture2D ditherMap;
Texture2D SpriteTexture;
float2 TextureDimensions;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

sampler2D ditherMapSampler = sampler_state
{
	Texture = <ditherMap>;
};


struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 getAlpha(float2 samp, int alpha) {
	if (alpha == 0) {
		return 0;
	}
	else if (alpha == 5) {
		return 1;
	}
	float2 pixelLocation = float2(samp.x * (TextureDimensions.x ), samp.y * (TextureDimensions.y ));
	float2 ditherLocation = float2(pixelLocation.x % 32, pixelLocation.y);
	//int2 ditherLocation = int2((35 * TextureDimensions.x) % 32, (35 * TextureDimensions.y) % 32);

	ditherLocation.x += (alpha * 32);
	//ditherLocation.y += (alpha * 32);

	return tex2D(ditherMapSampler, float2(ditherLocation.x / (32 * 6), ditherLocation.y / 32));
	//return (pixelLocation.x / TextureDimensions.x);
	//return (samp.x * TextureDimensions.x % 32) / 32;
	//return ditherLocation.x / 32;
	//return (ditherLocation.x / 32) * TextureDimensions.x;
	//return samp.x;
	//return alpha % 2;
	//alpha *= (1 / 6);
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float4 color = tex2D(SpriteTextureSampler,input.TextureCoordinates) * input.Color;
	//color.a *= floor(color.a *5) /5;
	color.a = getAlpha(input.TextureCoordinates, floor(color.a * 5)).a;
	//color.a = input.TextureCoordinates.x % 4.0;
	//color.rgb -= 0.3 * color.rgb;
	return color;
	//return tex2D(SpriteTextureSampler,input.TextureCoordinates) * input.Color;
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};