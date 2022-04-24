Shader "ZackURP/Effect/BayerDither"
{
    Properties
    {
        [MainColor] _BaseColor("Color", Color) = (1,1,1,1)
        _DisappearRange ("Disappear Range", float) = 2
        _DisappearDistance ("Disappear Distance", float) = 2
        _BayerDitherTex ("Bayer Dither Texture", 2D) = "black" {}
    }
       
    SubShader
    {
        HLSLINCLUDE
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                
                float4 _BayerDitherTex_TexelSize;	// x contains 1.0 / width; y contains 1.0 / height; z contains width; w contains height
                // 控制范围
                float _DisappearRange;
                // 控制消失距离
                float _DisappearDistance;
            CBUFFER_END
            
            #include "Assets/Scripts/URP/ShaderLibrary/BayerDither.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float disappearAlpha : TEXCOORD0;
                
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            // vertex Shader
            Varyings Vertex(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                
                float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
                output.positionCS = mul(GetWorldToHClipMatrix(), float4(positionWS.xyz, 1.0));
                
                float viewDistance = length(positionWS - GetCameraPositionWS());
				output.disappearAlpha = 1 - saturate((_DisappearDistance - viewDistance) * _DisappearRange);
                
                return output;
            }
    
            // fragment Shader
            half4 Fragment(Varyings input) : SV_Target
            {
                ClipByBayerDither(input.disappearAlpha, input.positionCS);
                //return SAMPLE_TEXTURE2D(_BayerDitherTex, sampler_BayerDitherTex, frac(floor(input.positionCS)/16));
                return _BaseColor;
            }
        ENDHLSL
    
		Tags { "Queue" = "AlphaTest" }
        Pass
        {
            Tags { "RenderPipeline" = "UniversalPipeline" "RenderType" = "Opaque" }
            HLSLPROGRAM
                #pragma vertex Vertex
                #pragma fragment Fragment
            ENDHLSL
        }
    }
    
}
