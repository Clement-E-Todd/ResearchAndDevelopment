Shader "Custom/My Third Lighting Shader" {
    Properties{
        _Tint ("Tint", Color) = (1, 1, 1, 1)
        _MainTex("Albedo", 2D) = "white" {}
        [Gamma] _Metallic("Metallic", Range(0,1)) = 0.5
        _Smoothness("Smoothness", Range(0,1)) = 0.5
    }

    SubShader{

        Pass{
            Tags{
                "LightMode" = "ForwardBase"
            }

            CGPROGRAM

            // Target at least shader level 3 to ensure that Unity selects the best BRFD function
            #pragma target 3.0
            
            #pragma vertex MyVertexProgram
            #pragma fragment MyFragmentProgram

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

            float4 MyFragmentProgram(Parameters parameters) : SV_TARGET {

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

                /* Use Unity's PBS lighting. Arguments:
                 * diffuse (albedo)
                 * specular color
                 * diffuse visibility (one minus the reflectivity)
                 * roughness (one minus smoothness)
                 * surface normal
                 * view direction
                 * Unity direct light struct
                 * Unity indirect light struct
                 */
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
            ENDCG
        }
    }
}