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


/*AGREGADO PARA ILUMINACIÓN*/
float4x4 InverseTransposeWorld;
float4x4 WorldViewProjection;

float3 ambientColor; // Light's Ambient Color
float3 diffuseColor; // Light's Diffuse Color
float3 specularColor; // Light's Specular Color
float KAmbient; 
float KDiffuse; 
float KSpecular;
float shininess; 
float3 lightPosition;
float3 eyePosition; // Camera position
float2 Tiling;


float Time = 0;
uniform texture Texture;
uniform sampler2D TextureSampler = sampler_state {
    Texture = (Texture);
    MagFilter = Linear;
    MinFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
	MIPFILTER = LINEAR;
};

struct VertexShaderInput
{
	float4 Position : POSITION0;
	//float3 Color : COLOR0;

	float4 Normal : NORMAL;
	float2 TextureCoordinates : TEXCOORD0;
	
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	//float3 Color : COLOR0; 

	float2 TextureCoordinates : TEXCOORD0;
	float4 WorldPosition : TEXCOORD1;
	float4 Normal : TEXCOORD2;

	float4 LocalPosition : TEXCOORD3;// LO HICE ANTES PARA LAS COORDENADAS DE TEXTURA
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

	// Propagamos el color
	//output.Color = input.Color;

	output.TextureCoordinates = input.TextureCoordinates;

    return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    //return float4(DiffuseColor, 1.0);
	float2 TextureCoordinates = float2(atan2(input.LocalPosition.x,input.LocalPosition.z),input.LocalPosition.y);

	float4 sample = tex2D(TextureSampler, input.TextureCoordinates);
	return float4(sample.rgb,1.0);
}


VertexShaderOutput MainVS_Lighting(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;

    output.Position = mul(input.Position, WorldViewProjection);
    output.WorldPosition = mul(input.Position, World);
    output.Normal = mul(float4(normalize(input.Normal.xyz), 1.0), InverseTransposeWorld);
    output.TextureCoordinates = input.TextureCoordinates * Tiling;
	
	return output;
}

float4 MainPS_Lighting(VertexShaderOutput input) : COLOR
{
    // Base vectors
    float3 lightDirection = normalize(lightPosition - input.WorldPosition.xyz);
    float3 viewDirection = normalize(eyePosition - input.WorldPosition.xyz);
    float3 halfVector = normalize(lightDirection + viewDirection);
    float3 normal = normalize(input.Normal.xyz);
    
	// Get the texture texel
    float4 texelColor = tex2D(TextureSampler, input.TextureCoordinates);
    
	// Calculate the diffuse light
    float NdotL = saturate(dot(normal, lightDirection));
    float3 diffuseLight = KDiffuse * diffuseColor * NdotL;

	// Calculate the specular light
    float NdotH = dot(normal, halfVector);
    float3 specularLight = KSpecular * specularColor * pow(saturate(NdotH), shininess);
    
    // Final calculation
    float4 finalColor = float4(saturate(ambientColor * KAmbient + diffuseLight) * texelColor.rgb + specularLight, texelColor.a);

    return finalColor;  
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};

//ESTA TÉCNICA SERA LA USADA PARA LA ILUMINACIÓN
technique LightingTechnique
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS_Lighting();
        PixelShader = compile PS_SHADERMODEL MainPS_Lighting();
    }
};