#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// Custom Effects - https://docs.monogame.net/articles/content/custom_effects.html
// High-level shader language (HLSL) - https://docs.microsoft.com/en-us/windows/win32/direct3dhlsl/dx-graphics-hlsl
// Programming guide for HLSL - https://docs.microsoft.com/en-us/windows/win32/direct3dhlsl/dx-graphics-hlsl-pguide
// Reference for HLSL - https://docs.microsoft.com/en-us/windows/win32/direct3dhlsl/dx-graphics-hlsl-reference
// HLSL Semantics - https://docs.microsoft.com/en-us/windows/win32/direct3dhlsl/dx-graphics-hlsl-semantics

float4x4 World;
float4x4 View;
float4x4 Projection;

float3 DiffuseColor;
float Time = 0;

// DiffuseMap
texture Texture;
sampler2D TextureSampler = sampler_state {
    Texture = (Texture);
    MagFilter = Linear;
    MinFilter = Linear;
	MipFilter = Linear;
    AddressU = clamp;
    AddressV = clamp;
};

// NormalMap
texture normalTexture;
sampler2D normalSampler = sampler_state {
	Texture = (normalTexture);
	AddressU = clamp;
    AddressV = clamp;
	MagFilter = Linear;
    MinFilter = Linear;
	MipFilter = Linear;
};
struct VertexShaderInput{
	//Consigue la posicion del vertice
	float4 Position : POSITION0;

	//Consigue la normal del vertice
	float4 Normal : NORMAL0;

	//Consigue las coordenadas de la textura (u,v)
	float2 TextureCoordinates : TEXCOORD0;
	
};

struct VertexShaderOutput{
	//Devuelve la posicion del vertice rasterizado
	//float4 Position : SV_POSITION;

	float4 Position : SV_POSITION;
	float2 TextureCoordinates : TEXCOORD0;
	float3 WorldNormal : TEXCOORD1;
	float4 WorldPosition : TEXCOORD2;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
    // Clear the output
	VertexShaderOutput output = (VertexShaderOutput)0;

	//float4x4 WorldViewProj = mul(mul(World, View), Projection); 
    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);	
    output.Position = mul(viewPosition, Projection);

	output.TextureCoordinates = input.TextureCoordinates; //+ float2(0.5f, 0.5f);
	//output.TextureCoordinates = input.TextureCoordinates;
	
	return output;
}

float3 getNormalFromMap(float2 textureCoordinates, float3 worldPosition, float3 worldNormal){
	float3 tangentNormal = tex2D(normalSampler, textureCoordinates).xyz * 2.0 - 1.0;

	float3 Q1 = ddx(worldPosition);
	float3 Q2 = ddy(worldPosition);
	float2 st1 = ddx(textureCoordinates);
	float2 st2 = ddy(textureCoordinates);

	worldNormal = normalize(worldNormal.xyz);
	float3 T = normalize(Q1 * st2.y - Q2 * st1.y);
	float3 B = -normalize(cross(worldNormal, T));
	float3x3 TBN = float3x3(T, B, worldNormal);

	return normalize(mul(tangentNormal, TBN));
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    //return float4(DiffuseColor, 1.0);

	//float3 albedo = pow(tex2D(albedoSampler, input.TextureCoordinates).rgb, float3(2.2, 2.2, 2.2));
	//float2 TextureCoordinates = float2(input.TextureCoordinates.x, input.TextureCoordinates.y);
	//float s = 1f;
	//input.TextureCoordinates = (input.TextureCoordinates - scaleCenter) * scale + scaleCenter;
	//Red Green Blue Alpha
	//float3 textureColor = pow(tex2D(TextureSampler, input.TextureCoordinates).rgb, float3(1, 1, 1));
	float4 textureSample = tex2D(TextureSampler, input.TextureCoordinates);
	//textureColor.a = 1;
	return float4(textureSample.rgb , 1.0);

}

technique RenderWalls
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};