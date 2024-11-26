#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// Matrices y parámetros generales
float4x4 WorldViewProjection;
float4x4 InverseTransposeWorld;
float4x4 World;

// Posiciones de luz y cámara
float3 lightPosition;
float3 cameraPosition;
float3 eyePosition;

// Parámetros de iluminación
float3 ambientColor;
float3 diffuseColor;
float3 specularColor;
float shininess;

// Texturas
texture baseTexture;
sampler2D textureSampler = sampler_state
{
    Texture = (baseTexture);
    MagFilter = Linear;
    MinFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};

texture normalMap;
sampler2D normalSampler = sampler_state
{
    Texture = <normalMap>;
    MagFilter = Linear;
    MinFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};

texture environmentMap;
samplerCUBE environmentMapSampler = sampler_state
{
    Texture = (environmentMap);
    MagFilter = Linear;
    MinFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};

// Estructuras para datos entre shaders
struct VertexShaderInput
{
    float4 Position : POSITION0;
    float3 Normal : NORMAL;
    float2 TextureCoordinates : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float2 TextureCoordinates : TEXCOORD0;
    float4 WorldPosition : TEXCOORD1;
    float4 Normal : TEXCOORD2;
};

// Función para calcular normales desde el mapa normal
float3 getNormalFromMap(float2 textureCoordinates, float3 worldPosition, float3 worldNormal)
{
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

// Sombrado principal del vértice
VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput) 0;

    output.Position = mul(input.Position, WorldViewProjection);
    output.WorldPosition = mul(input.Position, World);
    output.Normal = mul(float4(normalize(input.Normal.xyz), 1.0), InverseTransposeWorld);
    output.TextureCoordinates = input.TextureCoordinates;

    return output;
}

// Sombreador de píxeles con Blinn-Phong y environment mapping
float4 MainPS(VertexShaderOutput input) : COLOR
{
    // Normalizar vectores
    float3 normal = getNormalFromMap(input.TextureCoordinates, input.WorldPosition.xyz, input.Normal.xyz);

    // Vector hacia la luz
    float3 lightDirection = normalize(lightPosition - input.WorldPosition.xyz);

    // Blinn-Phong: cálculo de iluminación
    float3 viewDirection = normalize(cameraPosition - input.WorldPosition.xyz);
    float3 halfVector = normalize(lightDirection + viewDirection);

    // Componente difusa
    float diffuseFactor = max(dot(normal, lightDirection), 0.0);

    // Componente especular
    float specularFactor = pow(max(dot(normal, halfVector), 0.0), shininess);

    // Componente ambiental
    float3 ambient = ambientColor;

    // Combinación de las componentes
    float3 lighting = ambient + (diffuseColor * diffuseFactor + specularColor * specularFactor);

    // Color base con textura
    float3 baseColor = tex2D(textureSampler, input.TextureCoordinates).rgb;
    baseColor *= lighting;

    // Environment mapping
    float3 view = normalize(eyePosition - input.WorldPosition.xyz);
    float3 reflection = reflect(view, normal);
    float3 reflectionColor = texCUBE(environmentMapSampler, reflection).rgb;

    float fresnel = saturate((1.0 - dot(normal, view)));

    return float4(lerp(baseColor, reflectionColor, fresnel), 1);
}

// Técnicas
technique EnvironmentMap
{
    pass Pass0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};