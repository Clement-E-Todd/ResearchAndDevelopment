Shader "Custom/My First Lighting Shader" {
    Properties{
        _Tint ("Tint", Color) = (1, 1, 1, 1)
        _MainTex("Albedo", 2D) = "white" {}
        _SpecularTint("Specular", Color) = (0.5, 0.5, 0.5)
        _Smoothness("Smoothness", Range(0,1)) = 0.5
    }

    SubShader{

        Pass{
            // We have to give this pass the "LightMode" tag of "ForwardBase" in order
            // to get access to the main directional light in the scene.
            Tags{
                "LightMode" = "ForwardBase"
            }

            CGPROGRAM

            #pragma vertex MyVertexProgram
            #pragma fragment MyFragmentProgram

            //#include "UnityCG.cginc" // This is already included in the include below
            #include "UnityStandardBRDF.cginc"
            #include "UnityStandardUtils.cginc"

            float4 _Tint;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _SpecularTint;
            float _Smoothness;

            struct Parameters {
                float4 clipPosition : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : TEXCOORD1;
                float3 worldPosition : TEXCOORD2;
            };

            struct VertexData {
                float4 position : POSITION;
                float3 normal : NORMAL; // Input to the vertex program from Unity using the "NORMAL" keyword
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
                // Note: even though the normal was normalized in the vertex program, we may want to normalize
                // it again here. This is because we are not working with the direct output of a single call to
                // the vertex program; we are working with the interpolated output between the three seperate
                // calls of the vertex program for the current triangle. However, while the normals we receive
                // as input here may not be unit length, they are very close and the error is very small. It
                // would be reasonable to omit this recalculation as an optimization depending on how serious
                // the error is in the specific shader.
                parameters.normal = normalize(parameters.normal);

                // Get the direction that the light is coming from. _WorldSpaceLightPos0 is a unity shader
                // variable that gives us the light's position or, if it's a directional light, its direction.
                float3 lightDirection = _WorldSpaceLightPos0.xyz;

                // Get the direction between the camera and the fragment's world position (_WorldSpaceCameraPos
                // is part of the unity shadeer variables)
                float3 viewDirection = normalize(_WorldSpaceCameraPos - parameters.worldPosition);

                // Get the light color with another handy unity variable: _LightColor0
                float3 lightColor = _LightColor0.rgb;

                // Get the specular value of the light using the Blinn-Phong method
                float3 lightViewDirection = normalize(lightDirection + viewDirection); // Halfway between light and view directions
                float3 specular = _SpecularTint.rgb * pow(DotClamped(lightViewDirection, parameters.normal), _Smoothness * 100) * lightColor;

                // Find out how directly the light is hitting this surface. DotClamped is dot product clamped
                // between 0 and 1.
                half lightStrength = DotClamped(lightDirection, parameters.normal);

                // Albedo is how much light is reflected from an object's surface. We can view this as the
                // surface color.
                float3 albedo = tex2D(_MainTex, parameters.uv).rgb * _Tint.rgb;

                // The more specular light gets reflected, the less diffuse light does. The function used here is included
                // along with UnityStandardUtils.cginc. 'oneMinusReflectivity' is an output parameter that indicates how
                // much of the albedo remains.
                float oneMinusReflectivity;
                albedo = EnergyConservationBetweenDiffuseAndSpecular(
                    albedo, _SpecularTint.rgb, oneMinusReflectivity
                );

                // Get the diffuse color
                float3 diffuse = albedo * lightColor * lightStrength;

                // 'saturate' is a handy function for clamping color between 0 and 1. It's technically not needed
                // here since we already know that the color is in our desired range.
                return saturate(float4(diffuse + specular, 1));
            }
            ENDCG
        }
    }
}