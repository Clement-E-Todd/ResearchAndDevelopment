Shader "Experiments/Cutout" {
    Properties{
        _LightColor("Light Color", Color) = (0.75,0.75,0.75,1)
        _DarkColor("Dark Color", Color) = (0.25,0.25,0.25,1)
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

            struct VertexParams {
                float4 objectPosition : POSITION;
            };

            struct FragmentParams {
                float4 clipPosition : SV_POSITION;
                float3 objectPosition : TEXCOORD0;
                float3 worldPosition : TEXCOORD1;
                float3 screenPosition : TEXCOORD2;
                
                float4 meshClipPosition : TEXCOORD3;
                float3 meshWorldPosition : TEXCOORD4;
                float3 meshScreenPosition : TEXCOORD5;
            };

            float3 ObjectSpaceToScreenSpace(float3 objectSpaceVector) {
                float4 clipVector = UnityObjectToClipPos(objectSpaceVector);
                float4 screenPosVector = ComputeScreenPos(clipVector);
                return screenPosVector.xyz / screenPosVector.w;
            }

            FragmentParams VertexProgram(VertexParams input) {
                FragmentParams output;

                output.clipPosition = UnityObjectToClipPos(input.objectPosition);
                output.objectPosition = input.objectPosition;
                output.worldPosition = mul(unity_ObjectToWorld, input.objectPosition);
                float4 screenPosRaw = ComputeScreenPos(output.clipPosition);
                output.screenPosition = screenPosRaw.xyz / screenPosRaw.w;

                output.meshClipPosition = UnityObjectToClipPos(float4(0,0,0,1));
                output.meshWorldPosition = mul(unity_ObjectToWorld, float4(0,0,0,1));
                float4 meshScreenPosRaw = ComputeGrabScreenPos(output.meshClipPosition);
                output.meshScreenPosition = meshScreenPosRaw.xyz / meshScreenPosRaw.w;

                return output;
            }

            float4 FragmentProgram(FragmentParams input) : SV_TARGET {
                static const float PI = 3.14159265f;

                float3 viewFragDirection = normalize(_WorldSpaceCameraPos - input.worldPosition);
                float3 viewMeshDirection = normalize(_WorldSpaceCameraPos - input.meshWorldPosition);
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                
                // Calculate how well-lit the mesh is from the camera's perspective
                float meshBrightness = dot(lightDirection, viewMeshDirection) / 2 + 0.5;

                // Calculate how wide of a range the gradient should have
                float gradientRange = sin(meshBrightness * PI);

                // Calculate the color range of the gradient based on the mesh brightness
                float4 darkColor = lerp(_DarkColor, _LightColor, clamp(meshBrightness - gradientRange / 2, 0, 1));
                float4 lightColor = lerp(_DarkColor, _LightColor, clamp(meshBrightness + gradientRange / 2, 0, 1));

                // Calculate how well-lit this fragment is from the camera's perspective
                float fragBrightness = dot(-lightDirection, viewFragDirection) / 2 + 0.5;

                // TEST: Squeeze the gradient in towards its midpoint
                // TODO: Keep this feature but make it more graceful / dynamic
                fragBrightness = fragBrightness * 2 - 0.5;

                // Select a color from the gradient range according to the frag brightness
                float4 fragColor = lerp(darkColor, lightColor, fragBrightness);

                // TEST: Uncomment this to make the midpoint visibility stronger
                //fragColor.r = fragBrightness > 0.5 ? 0.75 : 0.25;

                return fragColor;
            }

            ENDCG
        }
    }
}
