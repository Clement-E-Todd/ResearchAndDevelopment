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

            #include "UnityCG.cginc" // Remove this when including Unity lighting

            float4 _BrightColor;
            float4 _DarkColor;

            struct VertexParams {
                float4 objectSpacePosition : POSITION;
            };

            struct FragmentParams {
                float4 clipPosition : SV_POSITION;
                float3 screenPosition : TEXCOORD0;
                float3 meshScreenPosition : TEXCOORD1;
            };

            FragmentParams VertexProgram(VertexParams input) {
                FragmentParams output;

                output.clipPosition = UnityObjectToClipPos(input.objectSpacePosition);
                float4 screenPosRaw = ComputeScreenPos(output.clipPosition);
                output.screenPosition = screenPosRaw.xyz / screenPosRaw.w;

                float4 meshClipPosition = UnityObjectToClipPos(float4(0,0,0,1));
                float4 meshScreenPosRaw = ComputeGrabScreenPos(meshClipPosition);
                output.meshScreenPosition = meshScreenPosRaw.xyz / meshScreenPosRaw.w;

                return output;
            }

            float4 FragmentProgram(FragmentParams input) : SV_TARGET {
                float radius = 0.5;
                return lerp(_BrightColor, _DarkColor, distance(input.screenPosition, input.meshScreenPosition) / radius);
            }

            ENDCG
        }
    }
}
