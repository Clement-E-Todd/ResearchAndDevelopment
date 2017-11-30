// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Experiments/Cutout" {
    Properties{
        _BrightColor("Bright Color", Color) = (0.75,0.75,0.75,1)
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

            float4 _BrightColor;
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
                float3 viewDirection = normalize(_WorldSpaceCameraPos - input.worldPosition);
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                return lerp(_DarkColor, _BrightColor, dot(lightDirection, viewDirection) / 2 + 0.5);
            }

            ENDCG
        }
    }
}
