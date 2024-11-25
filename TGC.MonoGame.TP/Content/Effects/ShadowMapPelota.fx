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
float4x4 LightViewProjection;

// Posiciones de luz y cámara
float3 lightPosition;
float3 cameraPosition;

// Parámetros de iluminación
float3 ambientColor;
float3 diffuseColor;
float3 specularColor;
float shininess;

// Tamaño del mapa de sombras
float2 shadowMapSize;

// Constantes para la corrección del sesgo
static const float modulatedEpsilon = 0.000041200182749889791011810302734375;
static const float maxEpsilon = 0.000023200045689009130001068115234375;

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

// Declaración de la textura y el sampler para el mapa normal
texture normalMap;
sampler2D normalSampler = sampler_state
{
    Texture = <normalMap>;
    MagFilter = Linear;
    MinFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};

// Declaración de la textura y el sampler para el environment map (mapa de entorno)
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
struct DepthPassVertexShaderInput
{
    float4 Position : POSITION0;
};

struct DepthPassVertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 ScreenSpacePosition : TEXCOORD1;
};

struct ShadowedVertexShaderInput
{
    float4 Position : POSITION0;
    float3 Normal : NORMAL;
    float2 TextureCoordinates : TEXCOORD0;
};

struct ShadowedVertexShaderOutput
{
    float4 Position : SV_POSITION;
    float2 TextureCoordinates : TEXCOORD0;
    float4 WorldSpacePosition : TEXCOORD1;
    float4 LightSpacePosition : TEXCOORD2;
    float4 Normal : TEXCOORD3;
};

// Sombrado de profundidad
DepthPassVertexShaderOutput DepthVS(DepthPassVertexShaderInput input)
{
    DepthPassVertexShaderOutput output;
    output.Position = mul(input.Position, WorldViewProjection);
    output.ScreenSpacePosition = output.Position;
    return output;
}

float4 DepthPS(DepthPassVertexShaderOutput input) : COLOR
{
    float depth = input.ScreenSpacePosition.z / input.ScreenSpacePosition.w;
    return float4(depth, depth, depth, 1.0);
}

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
ShadowedVertexShaderOutput MainVS(ShadowedVertexShaderInput input)
{
    ShadowedVertexShaderOutput output;
    output.Position = mul(input.Position, WorldViewProjection);
    output.TextureCoordinates = input.TextureCoordinates;
    output.WorldSpacePosition = mul(input.Position, World);
    output.LightSpacePosition = mul(output.WorldSpacePosition, LightViewProjection);
    output.Normal = mul(float4(input.Normal, 0.0), InverseTransposeWorld); // Normal en espacio del mundo
    return output;
}

// Sombreador de píxeles con Blinn-Phong, PCF y Environment Mapping (reflección)
float4 ShadowedPCFPS(ShadowedVertexShaderOutput input) : COLOR
{
    // Transformación al espacio de luz y coordenadas del mapa de sombras
    float3 lightSpacePosition = input.LightSpacePosition.xyz / input.LightSpacePosition.w;
    float2 shadowMapTextureCoordinates = 0.5 * lightSpacePosition.xy + 0.5;
    shadowMapTextureCoordinates.y = 1.0 - shadowMapTextureCoordinates.y;

    // Vector hacia la luz
    float3 lightDirection = normalize(lightPosition - input.WorldSpacePosition.xyz);

    // Calcular normales ajustadas por el mapa normal
    float3 normal = getNormalFromMap(input.TextureCoordinates, input.WorldSpacePosition.xyz, input.Normal.xyz);

    // Sesgo para sombras
    float inclinationBias = max(modulatedEpsilon * (1.0 - dot(normal, lightDirection)), maxEpsilon);

    // PCF (Percentage Closer Filtering)
    float notInShadow = 0.0;
    float2 texelSize = 1.0 / shadowMapSize;
    for (int x = -1; x <= 1; x++)
    {
        for (int y = -1; y <= 1; y++)
        {
            float pcfDepth = tex2D(shadowMapSampler, shadowMapTextureCoordinates + float2(x, y) * texelSize).r + inclinationBias;
            notInShadow += step(lightSpacePosition.z, pcfDepth) / 9.0;
        }
    }

    // Blinn-Phong: cálculo de iluminación
    float3 viewDirection = normalize(cameraPosition - input.WorldSpacePosition.xyz);
    float3 halfVector = normalize(lightDirection + viewDirection);

    // Componente difusa
    float diffuseFactor = max(dot(normal, lightDirection), 0.0);

    // Componente especular
    float specularFactor = pow(max(dot(normal, halfVector), 0.0), shininess);

    // Componente ambiental
    float3 ambient = ambientColor;

    // Combinación de las componentes de iluminación
    float3 lighting = ambient + 
                      notInShadow * (diffuseColor * diffuseFactor + specularColor * specularFactor);

    // Color base con textura
    float4 baseColor = tex2D(textureSampler, input.TextureCoordinates);
    baseColor.rgb *= lighting;

    // Calcular la reflexión
    float3 viewDir = normalize(cameraPosition - input.WorldSpacePosition.xyz);
    float3 reflectionDir = reflect(viewDir, normal);

    // Obtener el color del mapa del entorno (reflexión)
    float3 reflectionColor = texCUBE(environmentMapSampler, reflectionDir).rgb;

    // Usar Fresnel para suavizar la transición entre base color y reflejo
    float fresnel = saturate(1.0 - dot(normal, viewDir)); // Usamos el dot product para el ángulo de incidencia

    // Mezcla entre color base y reflexión utilizando el Fresnel
    baseColor.rgb = lerp(baseColor.rgb, reflectionColor, fresnel);

    return baseColor;
}

// Técnicas
technique DepthPass
{
    pass Pass0
    {
        VertexShader = compile VS_SHADERMODEL DepthVS();
        PixelShader = compile PS_SHADERMODEL DepthPS();
    }
};

technique DrawShadowedPCF
{
    pass Pass0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL ShadowedPCFPS();
    }
};
