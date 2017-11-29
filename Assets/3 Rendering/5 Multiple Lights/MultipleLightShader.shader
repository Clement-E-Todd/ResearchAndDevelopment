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
                // This tag causes the pass to handle the MAIN light
                "LightMode" = "ForwardBase"
            }

            CGPROGRAM

            // Target at least shader level 3 to ensure that Unity selects the best BRFD function
            #pragma target 3.0
            
            #pragma vertex MyVertexProgram
            #pragma fragment MyFragmentProgram

            #include "MyLighting.cginc"

            ENDCG
        }

        Pass{
            Tags{
                // This tag causes the pass to handle ADDITIONAL lights
                "LightMode" = "ForwardAdd"
            }

            // Change the blend mode to additive so we can mix with the first pass instead of overwriting.
            // The default is "One Zero".
            Blend One One

            // Disable the depth check; the GPU already did a depth check in the first pass, so there's no
            // need to write to the depth buffer again.
            ZWrite off

            CGPROGRAM

            // Target at least shader level 3 to ensure that Unity selects the best BRFD function
            #pragma target 3.0
            
            #pragma vertex MyVertexProgram
            #pragma fragment MyFragmentProgram

            #include "MyLighting.cginc"

            ENDCG
        }
    }
}