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
static const float modulatedEpsilon = 0.000041200182749889791011810302734375;
static const float maxEpsilon = 0.000023200045689009130001068115234375;
float3 DiffuseColor;
float3 lightPosition;
float Time = 0;

uniform texture Texture;
uniform sampler2D TextureSampler = sampler_state {
    Texture = (Texture);
    MagFilter = Linear;
    MinFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
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

float2 TextureScale = float2(20, 20); 

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float3 Color : COLOR0;

	float3 Normal : NORMAL0;
	float2 TextureCoordinates : TEXCOORD0;
	
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float3 Color : COLOR0; 
    float4 WorldSpacePosition : TEXCOORD1;
	float4 WorldPosition : TEXCOORD2;
	float2 TextureCoordinates : TEXCOORD3;
	float4 LocalPosition : TEXCOORD4;
	float4 LightSpacePosition : TEXCOORD5;
    float4 Normal : TEXCOORD6;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
    // Clear the output
	VertexShaderOutput output = (VertexShaderOutput)0;
    // Model space to World space
    float4 worldPosition = mul(input.Position, World);
    // World space to View space
    float4 viewPosition = mul(worldPosition, View);	
	// View space to Projection space
    output.Position = mul(viewPosition, Projection);

	output.LocalPosition = worldPosition;

	// Pass the texture coordinates to the pixel shader
    output.TextureCoordinates = input.TextureCoordinates * TextureScale;

    return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    // Use the texture coordinates directly
    float2 TextureCoordinates = input.TextureCoordinates;
	float3 lightSpacePosition = input.LightSpacePosition.xyz / input.LightSpacePosition.w;
	float2 shadowMapTextureCoordinates = 0.5 * lightSpacePosition.xy + 0.5;
	shadowMapTextureCoordinates.y = 1.0 - shadowMapTextureCoordinates.y;

	float3 lightDirection = normalize(lightPosition - input.WorldSpacePosition.xyz);
	float inclinationBias = max(modulatedEpsilon * (1.0 - dot(input.Normal.xyz, lightDirection)), maxEpsilon);

	float shadowMapDepth = tex2D(shadowMapSampler, shadowMapTextureCoordinates).r + inclinationBias;

	float notInShadow = step(lightSpacePosition.z, shadowMapDepth);
    // Sample the texture using the texture coordinates
    float4 sample = tex2D(TextureSampler, TextureCoordinates);
    return float4(sample.rgb * notInShadow, 1.0);
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};