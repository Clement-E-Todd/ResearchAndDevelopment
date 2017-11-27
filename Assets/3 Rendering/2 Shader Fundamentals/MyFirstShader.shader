// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/My First Shader" {

    // Custom data to be passed in is defined in the "Propertes" block. Properties can have any
    // name, but the convention is to start with an underscore followed by Pascal casing.
    // Example: _NameInShader ("Name In Unity Inspector", Type) = [default value]
    Properties{
        _Tint ("Tint", Color) = (1, 1, 1, 1)
        _MainTex("Texture", 2D) = "white" {} // TIP: Using the name _MainTex allows us to access
                                             // this property in Unity scripts via Material.mainTexture
                                             // NOTE: Why are the curly braces there? There used to be
                                             // texture settings for old fixed-function shaders, but they
                                             // are no longer used. These settings were put inside
                                             // those brackets.
    }

    // Multiple sub-shaders can be defined to provide variants on the shader. For example, you may
    // have one intended for Desktop devices and another for mobile.
    SubShader{
        // Each "pass" is a seperate render of the mesh. Multiple are used in deferred rendering,
        // but we only need one for now.
        Pass{
            CGPROGRAM

            // We have to tell the compiler which programs to use, via pragma directives.
            #pragma vertex MyVertexProgram
            #pragma fragment MyFragmentProgram

            // Include unity's handy package of basic shader code that hooks into the Unity engine.
            #include "UnityCG.cginc"

            // To access properties in the shader code, we declare variables whose names exactly
            // match the property names.
            float4 _Tint;
            sampler2D _MainTex;
            float4 _MainTex_ST; // To access the texture's tiling & offset data, declare a float4 with
                                // the same name plus "_ST" (stands for "scale and translation")

            // Declaring structs is a handy way to pass along all of the data from the vertex shader
            // to the fragment shader without bloating their parameter lists.
            struct Parameters {
                float4 worldPosition : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            struct VertexData {
                // When used as input to the vertex shader, the "POSITION" semantic gets the vertex's
                // position in local space.
                float4 position : POSITION;

                // When used as input to the vertex shader, the "TEXCOORD0" semantic gets the vertex's
                // UV coordinates.
                float2 uv : TEXCOORD0;
            };

            // A vertex program affects the mesh's vertices. "SV_POSITION" indicates that this
            // particular program will be affecting the vertices' positions.
            // This program takes the position provided by Unity (via the "POSITION" keyword) as input.
            // Note: The semantics TEXCOORD0, TEXCOORD1, TEXCOORD2 and so on are conventionally used
            // to pass any data outside of the fragment's world position. This data must be declared in
            // the vertex program's input using the "out" keyword. We are using a single struct to pass
            // data here, but see the commented-out declaration of "localPosition" below to see how to
            // declare the data without a struct in the parameter list.
            Parameters MyVertexProgram(
                VertexData vertexData/*,
                out float3 localPosition : TEXCOORD0*/) /*: SV_POSITION*/ {

                // Create an instance of the struct whose data needs to be set and passed along to
                // the fragment shader.
                Parameters parameters;
                
                // The position provided to us is in the object's local space. In order to display
                // properly in world space, we must multiply it with the model-view-projection matrix.
                // See upgrade note at top of file.
                parameters.worldPosition = UnityObjectToClipPos(vertexData.position);

                // When calculating the final UVs to use, tiling is stored in the x and y values of the
                // "_ST" variable while offset is stored in z and w. Unity has a handy macro for setting
                // UV info...
                parameters.uv = TRANSFORM_TEX(vertexData.uv, _MainTex);
                // ...which is the same as doing this:
                // parameters.uv = vertexData.uv * _MainTex_ST.xy + _MainTex_ST.zw;

                // The vertex shader's return value is what the fragment shader receives as input.
                return parameters;
            }

            // A fragment shader defines the output of each pixel onscreen. "SV_TARGET" indicates that
            // the output should be written to the shader's default target (the mesh it's attached to).
            // The fragment program takes the SV_POSITION output of the vertex program as input.
            float4 MyFragmentProgram(
                Parameters parameters/*,
                float3 localPosition : TEXCOORD0*/) : SV_TARGET {

                return tex2D(_MainTex, parameters.uv) * _Tint;
                //return float4(parameters.localPosition + 0.5, 1) * _Tint;
            }
            ENDCG
        }
    }
}