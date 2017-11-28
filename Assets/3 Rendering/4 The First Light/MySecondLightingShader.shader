Shader "Custom/My Second Lighting Shader" {
    Properties{
        _Tint ("Tint", Color) = (1, 1, 1, 1)
        _MainTex("Albedo", 2D) = "white" {}
        [Gamma] _Metallic("Metallic", Range(0,1)) = 0.5 // Gamma tag tells Unity to use gamma correction
        _Smoothness("Smoothness", Range(0,1)) = 0.5
    }

    SubShader{

        Pass{
            Tags{
                "LightMode" = "ForwardBase"
            }

            CGPROGRAM
            
            #pragma vertex MyVertexProgram
            #pragma fragment MyFragmentProgram

            #include "UnityStandardBRDF.cginc"
            #include "UnityStandardUtils.cginc"

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

            float4 MyFragmentProgram(Parameters parameters) : SV_TARGET {

                parameters.normal = normalize(parameters.normal);

                float3 lightDirection = _WorldSpaceLightPos0.xyz;

                float3 viewDirection = normalize(_WorldSpaceCameraPos - parameters.worldPosition);

                float3 lightColor = _LightColor0.rgb;

                half lightStrength = DotClamped(lightDirection, parameters.normal);

                float3 albedo = tex2D(_MainTex, parameters.uv).rgb * _Tint.rgb;

                float3 specularTint; // Output from following metallic calculation
                float oneMinusReflectivity; // Output from following metallic calculation
                albedo = DiffuseAndSpecularFromMetallic(
                    albedo, _Metallic, specularTint, oneMinusReflectivity
                );

                float3 diffuse = albedo * lightColor * lightStrength;

                float3 lightViewDirection = normalize(lightDirection + viewDirection);
                float3 specular = specularTint * lightColor * pow(
                    DotClamped(lightViewDirection, parameters.normal),
                    _Smoothness * 100
                );

                return float4(diffuse + specular, 1);
            }
            ENDCG
        }
    }
}