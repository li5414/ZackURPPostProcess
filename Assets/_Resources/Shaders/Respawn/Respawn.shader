Shader "ZackURP/Effect/Respawn"
{
    Properties
    {
        _MainTex ("Albedo", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Normal ("Normal", 2D) = "bump" {}

		[HDR]_EdgeColor1 ("Edge Color", Color) = (1,1,1,1)
		[HDR]_EdgeColor2 ("Edge Color", Color) = (1,1,1,1)
		_Noise ("Noise", 2D) = "white" {}
        _Cutoff ("Cutoff", Range(0.01,1)) = 1.0
		_Cutoff2 ("Cutoff2", Range(0.0,2.0)) = 1.0
		_EdgeSizeBot ("下边界宽度", Range(0,1)) = 0.2
		_EdgeSizeTop ("上边界宽度", Range(0,1)) = 0.2
		_Bounds ("渲染器包围盒大小", Vector) =  (1,1,1,1)
    }
    SubShader
    {
        HLSLINCLUDE
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Assets/Scripts/URP/ShaderLibrary/Math.hlsl"
        
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                half4 _Color;
                
                // Respawn
                half4 _EdgeColor1;
                half4 _EdgeColor2;
                half _Cutoff;
                half _Cutoff2;
                half _EdgeSizeBot;
                half _EdgeSizeTop;
                float4 _Bounds;
            CBUFFER_END
            
            // _MainTex
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            // _Noise
            TEXTURE2D(_Noise);
            SAMPLER(sampler_Noise);
            
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
                half3 positionWS : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            // vertex Shader
            Varyings Vertex(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                
                output.positionWS = TransformObjectToWorld(input.positionOS.xyz);
                output.positionCS = TransformWorldToHClip(output.positionWS.xyz);
                output.uv = TRANSFORM_TEX(input.texcoord, _MainTex);
                
                return output;
            }
    
            // fragment Shader
            half4 Fragment(Varyings input) : SV_Target
            {
                float3 pos = input.positionWS - mul(unity_ObjectToWorld, float4(0,0,0,1)).xyz;

                float2 noiseUV1 = float2(pos.y*2, pos.z*0.2);
                float2 noiseUV2 = float2(pos.y*4, pos.x*0.2);
    
                noiseUV1.x += _Cutoff*2;
                noiseUV2.x += _Cutoff;	
    
                float yBound = _Bounds.y + 0.2;
    
                half a = SAMPLE_TEXTURE2D(_Noise, sampler_Noise, noiseUV1).x;
                half b = SAMPLE_TEXTURE2D(_Noise, sampler_Noise, noiseUV2).x;
    
                half topNoise = a*b*2;
                half botNoise =  SAMPLE_TEXTURE2D(_Noise, sampler_Noise, float2(pos.y*6, 0)).x;
    
                float cutoff = (_Cutoff-0.5)*yBound;
    
    
                half Edge = step(cutoff, pos.y);    // 当前边界
                half EdgeBot = smoothstep(cutoff - _EdgeSizeBot, cutoff, pos.y);    // 边界到底部部分
                half EdgeTop = smoothstep(cutoff + _EdgeSizeTop, cutoff, pos.y);    // 边界到顶部部分
    
                half3 glowBot = _EdgeColor1 * EdgeBot * (1-Edge);   // 边界到底部部分发光
                half3 glowTop = lerp(_EdgeColor2, _EdgeColor1, EdgeTop) * Edge; // 边界到顶部部分发光
                
                half4 albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv) * _Color;
                half3 emission = (glowBot * botNoise) + (glowBot * 0.5) + glowTop;
                clip(1-Edge + (topNoise*EdgeTop) - _Cutoff - _Cutoff2);
                
                half3 color = albedo.rgb + emission;
                half alpha = albedo.a;
                return float4(color, alpha);
            }        
        ENDHLSL
        
        Pass
        {
            Tags { "RenderPipeline" = "UniversalPipeline" "LightMode" = "AfterCopyCameraColor" "RenderType" = "TransparentCutout" }
            Cull Off
            HLSLPROGRAM
                #pragma vertex Vertex
                #pragma fragment Fragment
            ENDHLSL
        }
        
    }
}
