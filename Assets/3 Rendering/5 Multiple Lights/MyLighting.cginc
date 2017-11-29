#if !defined(MY_LIGHTING_INCLUDED)
#define MY_LIGHTING_INCLUDED

#include "UnityPBSLighting.cginc"

float4 _Tint;
sampler2D _MainTex;
float4 _MainTex_ST;
float4 _SpecularTint;
float _Metallic;
float _Smoothness;

struct Parameters {
    float4 clipPosition : SV_POSITION;
    float2 uv : TEXCOORD0;
    float3 normal : TEXCOORD1;
    float3 worldPosition : TEXCOORD2;
};

struct VertexData {
    float4 position : POSITION;
    float3 normal : NORMAL;
    float2 uv : TEXCOORD0;
};

Parameters MyVertexProgram(VertexData vertexData) {
    Parameters parameters;
    parameters.clipPosition = UnityObjectToClipPos(vertexData.position);
    parameters.worldPosition = mul(unity_ObjectToWorld, vertexData.position);
    parameters.uv = TRANSFORM_TEX(vertexData.uv, _MainTex);
    parameters.normal = UnityObjectToWorldNormal(vertexData.normal);
    parameters.normal = normalize(parameters.normal);

    return parameters;
}

float4 MyFragmentProgram(Parameters parameters) : SV_TARGET{

    parameters.normal = normalize(parameters.normal);

    float3 viewDirection = normalize(_WorldSpaceCameraPos - parameters.worldPosition);
    
    float3 albedo = tex2D(_MainTex, parameters.uv).rgb * _Tint.rgb;
    
    float3 specularTint; // Output from following metallic calculation
    float oneMinusReflectivity; // Output from following metallic calculation
    albedo = DiffuseAndSpecularFromMetallic(
        albedo, _Metallic, specularTint, oneMinusReflectivity
    );
    
    // Package up the remaining light data using Unity's convenience structs...
    UnityLight light;
    light.color = _LightColor0.rgb;
    light.dir = _WorldSpaceLightPos0.xyz;
    light.ndotl = DotClamped(parameters.normal, light.dir);
    
    UnityIndirect indirectLight;
    indirectLight.diffuse = 0;
    indirectLight.specular = 0;

    return UNITY_BRDF_PBS(
        albedo,
        specularTint,
        oneMinusReflectivity,
        _Smoothness,
        parameters.normal,
        viewDirection,
        light,
        indirectLight
    );
}

#endif // MY_LIGHTING_INCLUDED