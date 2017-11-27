Shader "Custom/Textured With Detail" {


    Properties{
        _Tint ("Tint", Color) = (1, 1, 1, 1)
        _MainTex("Texture", 2D) = "white" {}
        _DetailTex("Detail Texture", 2D) = "gray" {}
    }

    SubShader{
        Pass{
            CGPROGRAM

            #pragma vertex MyVertexProgram
            #pragma fragment MyFragmentProgram

            #include "UnityCG.cginc"

            float4 _Tint;
            sampler2D _MainTex, _DetailTex;
            float4 _MainTex_ST, _DetailTex_ST;

            struct Parameters {
                float4 worldPosition : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 uvDetail : TEXCOORD1;
            };

            struct VertexData {
                float4 position : POSITION;
                float2 uv : TEXCOORD0;
            };

            Parameters MyVertexProgram(VertexData vertexData) {

                Parameters parameters;
                parameters.worldPosition = UnityObjectToClipPos(vertexData.position);
                parameters.uv = TRANSFORM_TEX(vertexData.uv, _MainTex);
                parameters.uvDetail = TRANSFORM_TEX(vertexData.uv, _DetailTex);
                return parameters;
            }

            float4 MyFragmentProgram(Parameters parameters) : SV_TARGET {

                float4 color = tex2D(_MainTex, parameters.uv) * _Tint;
                color *= tex2D(_DetailTex, parameters.uvDetail) * unity_ColorSpaceDouble;
                
                // Note: "unity_ColorSpaceDouble" is essentially "2", but is a safer value
                // that properly handles Unity's color settings. See section 1.5 of the
                // tutorial for more details here:
                // http://catlikecoding.com/unity/tutorials/rendering/part-3/

                return color;
            }
            ENDCG
        }
    }
}