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

// Agregado para la iluminacion
float4x4 InverseTransposeWorld;
float4x4 WorldViewProjection;

float3 lightPosition;
float3 lightColor;
float3 eyePosition;

#define LIGHT_COUNT 4

static const float PI = 3.14159265359;
static const float modulatedEpsilon = 0.000041200182749889791011810302734375;
static const float maxEpsilon = 0.000023200045689009130001068115234375;


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
sampler2D normalTextureSampler = sampler_state {
	Texture = (normalTexture);
	AddressU = clamp;
    AddressV = clamp;
	MagFilter = Linear;
    MinFilter = Linear;
	MipFilter = Linear;
};

//Textura para Metallic
//texture metallicTexture;
texture metallicTexture;
sampler2D metallicSampler = sampler_state
{
	Texture = (metallicTexture);
	ADDRESSU = WRAP;
	ADDRESSV = WRAP;
	MINFILTER = LINEAR;
	MAGFILTER = LINEAR;
	MIPFILTER = LINEAR;
};

//Textura para Roughness
texture roughnessTexture;
sampler2D roughnessSampler = sampler_state
{
	Texture = (roughnessTexture);
	ADDRESSU = WRAP;
	ADDRESSV = WRAP;
	MINFILTER = LINEAR;
	MAGFILTER = LINEAR;
	MIPFILTER = LINEAR;
};

//Textura para Ambient Occlusion
texture aoTexture;
sampler2D aoSampler = sampler_state
{
	Texture = (aoTexture);
	ADDRESSU = WRAP;
	ADDRESSV = WRAP;
	MINFILTER = LINEAR;
	MAGFILTER = LINEAR;
	MIPFILTER = LINEAR;
};

texture shadowMap;
sampler2D shadowMapSampler = sampler_state
{
    Texture = <shadowMap>;
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

//input Vertex Shader
struct Light
{
	float3 Position;
	float3 Color;
} ;



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
    float4 LightSpacePosition : TEXCOORD2; //AGREGUE
    float4 WorldSpacePosition : TEXCOORD1; //AGREGUE
	float4 Position : SV_POSITION;
	float2 TextureCoordinates : TEXCOORD0;
	float3 Normal : TEXCOORD3;
	float4 WorldPosition : TEXCOORD4;
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

float4 MainPS(VertexShaderOutput input) : COLOR{
	float4 textureSample = tex2D(TextureSampler, input.TextureCoordinates);
	// Shader de pista con sampling

	float3 lightSpacePosition = input.LightSpacePosition.xyz / input.LightSpacePosition.w;
	float2 shadowMapTextureCoordinates = 0.5 * lightSpacePosition.xy + 0.5;
	shadowMapTextureCoordinates.y = 1.0 - shadowMapTextureCoordinates.y;

	float3 lightDirection = normalize(lightPosition - input.WorldSpacePosition.xyz);
	float inclinationBias = max(modulatedEpsilon * (1.0 - dot(input.Normal, lightDirection)), maxEpsilon);

	float shadowMapDepth = tex2D(shadowMapSampler, shadowMapTextureCoordinates).r + inclinationBias;

	float notInShadow = step(lightSpacePosition.z, shadowMapDepth);

	
	return float4(textureSample.rgb * notInShadow, 1.0);
}

technique Render
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};