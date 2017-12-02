Shader "Experiments/Cutout" {
    Properties{
        _LightColor("Light Color", Color) = (0.75,0.75,0.75,1)
        _DarkColor("Dark Color", Color) = (0.25,0.25,0.25,1)
        _Squeeze("Squeeze Gradient", Range(0,1)) = 0.1
    }

    SubShader{

        Pass{
            Tags{
                "LightMode" = "ForwardBase"
            }

            CGPROGRAM

            #pragma vertex VertexProgram
            #pragma fragment FragmentProgram

            #include "UnityStandardUtils.cginc"

            float4 _LightColor;
            float4 _DarkColor;
            float _Squeeze;

            struct VertexParams {
                float4 objectPosition : POSITION;
            };

            struct FragmentParams {
                float4 clipPosition : SV_POSITION;
                float3 worldPosition : TEXCOORD0;
                float3 meshWorldPosition : TEXCOORD1;
            };

            FragmentParams VertexProgram(VertexParams input) {
                FragmentParams output;

                output.clipPosition = UnityObjectToClipPos(input.objectPosition);
                output.worldPosition = mul(unity_ObjectToWorld, input.objectPosition);
                output.meshWorldPosition = mul(unity_ObjectToWorld, float4(0,0,0,1));

                return output;
            }

            float4 FragmentProgram(FragmentParams input) : SV_TARGET {
                static const float PI = 3.14159265f;
                static const float squeezeFactorMax = 5;

                float cameraDistance = distance(_WorldSpaceCameraPos, input.worldPosition);
                float3 cameraToFragDir = normalize(_WorldSpaceCameraPos - input.worldPosition);
                float3 cameraToMeshDir = normalize(_WorldSpaceCameraPos - input.meshWorldPosition);
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                
                // Calculate how well-lit the mesh is from the camera's perspective (0 to 1)
                float meshBrightness = dot(lightDirection, cameraToMeshDir) / 2 + 0.5;

                // Calculate how wide of a range the gradient should have
                float gradientRange = sin(meshBrightness * PI);

                // Calculate the color range of the gradient based on the mesh brightness
                float4 darkColor = lerp(_DarkColor, _LightColor, clamp(meshBrightness - gradientRange / 2, 0, 1));
                float4 lightColor = lerp(_DarkColor, _LightColor, clamp(meshBrightness + gradientRange / 2, 0, 1));

                // Calculate how well-lit this fragment is from the camera's perspective
                float fragBrightness = dot(-lightDirection, cameraToFragDir) / 2 + 0.5;

                // Squeeze the gradient in towards its midpoint
                float squeezeFactor = 0.5 + (_Squeeze * squeezeFactorMax) * cameraDistance;
                fragBrightness = fragBrightness * (squeezeFactor * 2) - max(0, squeezeFactor - 0.5);

                // Center the gradient on the mesh
                fragBrightness += dot(lightDirection, cameraToMeshDir) * squeezeFactor;

                // Select a color from the gradient range according to the frag brightness
                return lerp(darkColor, lightColor, clamp(fragBrightness, 0, 1));
            }

            ENDCG
        }
    }
}
