// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Texture Splatting" {

    Properties{
        _MainTex("Splat Map", 2D) = "white" {}
        // Note: Use [NoScaleOffset] to prevent tiling and offset data from being passed in
        [NoScaleOffset] _Texture1("Texture 1", 2D) = "white" {}
        [NoScaleOffset] _Texture2("Texture 2", 2D) = "white" {}
        [NoScaleOffset] _Texture3("Texture 3", 2D) = "white" {}
        [NoScaleOffset] _Texture4("Texture 4", 2D) = "white" {}
    }

    SubShader{
        Pass{
            CGPROGRAM

            #pragma vertex MyVertexProgram
            #pragma fragment MyFragmentProgram

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _Texture1, _Texture2, _Texture3, _Texture4;

            struct VertexData {
                float4 position : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Parameters {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 uvSplat : TEXCOORD1;
            };

            Parameters MyVertexProgram(VertexData vertexData) {
                Parameters parameters;
                parameters.position = UnityObjectToClipPos(vertexData.position);
                parameters.uv = TRANSFORM_TEX(vertexData.uv, _MainTex);
                parameters.uvSplat = vertexData.uv; // Use the un-transformed UV for the splat map
                return parameters;
            }

            float4 MyFragmentProgram(Parameters parameters) : SV_TARGET {
                float4 splat = tex2D(_MainTex, parameters.uvSplat);
                return tex2D(_Texture1, parameters.uv) * splat.r +
                       tex2D(_Texture2, parameters.uv) * splat.g +
                       tex2D(_Texture3, parameters.uv) * splat.b +
                       tex2D(_Texture4, parameters.uv) * (1 - splat.r - splat.g - splat.b);
            }

            ENDCG
        }
    }
}