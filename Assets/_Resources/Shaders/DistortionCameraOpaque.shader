Shader "ZackURP/Effect/DistortionCameraOpaque"
{
    Properties
    {
        _NoiseTex ("Noise Texture", 2D) = "black" {}
        _DistortionParams ("Distortion Parameters", vector) = (0, 0, 0, 0)
    }
    SubShader
    {
        HLSLINCLUDE
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Assets/Scripts/URP/ShaderLibrary/Math.hlsl"
        
            CBUFFER_START(UnityPerMaterial)
                float4 _NoiseTex_ST;
                // 扰动参数 (xy:xy方向速度, z:扰动强度)
                float4 _DistortionParams;
            CBUFFER_END
            
            // _NoiseTex
            TEXTURE2D(_NoiseTex);
            SAMPLER(sampler_NoiseTex);
            // _CameraOpaqueTexture
            TEXTURE2D_X(_CameraOpaqueTexture);
            SAMPLER(sampler_CameraOpaqueTexture);
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            struct Varyings
            {
                half4 positionCS : SV_POSITION;
                half2 uv : TEXCOORD0;
                float4 grabPos : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            // vertex Shader
            Varyings Vertex(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                // 获取抓屏屏幕坐标
                output.grabPos = ComputeGrabScreenPos(output.positionCS);
                // 扰动贴图uv
                output.uv = TRANSFORM_TEX(input.texcoord, _NoiseTex);
                output.uv += _Time.y * _DistortionParams.xy;
                
                return output;
            }
    
            // fragment Shader
            half4 Fragment(Varyings input) : SV_Target
            {
                float distortionStrength = _DistortionParams.z;
                // 扰动偏移
                float2 offset = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, input.uv);
                offset = (offset * 2 - 1) * distortionStrength;
                
                float2 uv = input.grabPos.xy / input.grabPos.w + offset;
                half4 color = SAMPLE_TEXTURE2D_X(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, uv);
                return color;
            }        
        ENDHLSL
        
        Pass
        {
            Tags { "RenderPipeline" = "UniversalPipeline" "LightMode" = "AfterCopyColor" "RenderType" = "Opaque" }
            HLSLPROGRAM
                #pragma vertex Vertex
                #pragma fragment Fragment
            ENDHLSL
        }
        
    }
}
