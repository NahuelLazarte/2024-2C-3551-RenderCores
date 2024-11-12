#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float4x4 World;
float4x4 View;
float4x4 Projection;

float3 CameraPosition;


//en este caso se usa una textura cubica
texture SkyBoxTexture;
samplerCUBE SkyBoxSampler = sampler_state // el sampler define como se accede a la textura
{
    texture = <SkyBoxTexture>;
    magfilter = LINEAR;  // cuando la textura se amplia se usa el filtrado lineal para suavizar la imagen
    minfilter = LINEAR; // cuando la textura se reduce se usa el filtrado lineal para suavizar la imagen
    mipfilter = LINEAR; // Cuando se usan mipmaps, se usa filtrado lineal para seleccionar el nivel de detalle adecuado.
    AddressU = Mirror;  //Especifica el modo de dirección en la coordenada U. Hace que la textura se repita en un patron espejo cuando las coordenadas de textura esán fuera del rango [0,1]
    AddressV = Mirror; //Especifica el modo de dirección en la coordenada V. Hace que la textura se repita en un patron espejo cuando las coordenadas de textura esán fuera del rango [0,1]
};

struct VertexShaderInput  //Estructura de entrada
{
    float4 Position : POSITION0;
};

struct VertexShaderOutput  //Estructura de salida
{
    float4 Position : POSITION0;
    float3 TextureCoordinate : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

    float4 VertexPosition = mul(input.Position, World);
    output.TextureCoordinate = VertexPosition.xyz - CameraPosition;
    //Transforma la posición del vértice usando las matrices World, View y Projection.
    //Calcula las coordenadas de textura restando la posición de la cámara.
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    return float4(texCUBE(SkyBoxSampler, normalize(input.TextureCoordinate)).rgb, 1);
    //Usa las coordenadas de textura para muestrear la textura cúbica y devuelve el color.
}

technique Skybox
{
    pass Pass1
    {
        VertexShader = compile VS_SHADERMODEL VertexShaderFunction();
        PixelShader = compile PS_SHADERMODEL PixelShaderFunction();
    }
}
