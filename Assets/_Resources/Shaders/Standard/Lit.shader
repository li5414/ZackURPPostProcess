Shader "ZackURP/Standard/Lit"
{
    Properties
    {
        // Specular vs Metallic workflow
        [HideInInspector] _WorkflowMode("WorkflowMode", Float) = 1.0
        
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}
        [MainColor][HDR] _BaseColor("Base Color", Color) = (1,1,1,1)
        
        // 金属度
        _Metallic("Metallic", Range(0.0, 1.0)) = 0.0    // 与_MetallicGlossMap互斥
        _MetallicGlossMap("Metallic", 2D) = "white" {}  // 与_Metallic互斥
        // 光滑度
        _Smoothness("Smoothness", Range(0.0, 1.0)) = 0.5
        
        // 法线贴图
        _BumpScale("Scale", Float) = 1.0
        _BumpMap("Normal Map", 2D) = "bump" {}
        
        // 遮挡贴图
        _OcclusionStrength("Strength", Range(0.0, 1.0)) = 1.0
        _OcclusionMap("Occlusion", 2D) = "white" {}
        
        // 裁切
        [HideInInspector]_Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5 
        
        // Blending state
        [HideInInspector] _Surface("__surface", Float) = 0.0
        [HideInInspector] _Blend("__blend", Float) = 0.0
        [HideInInspector] _AlphaClip("__clip", Float) = 0.0
        [HideInInspector] _SrcBlend("__src", Float) = 1.0
        [HideInInspector] _DstBlend("__dst", Float) = 0.0
        [HideInInspector] _ZWrite("__zw", Float) = 1.0
        [HideInInspector] _Cull("__cull", Float) = 2.0
        
        // 受伤特效
        [HideInInspector] _UseHurt("__hurt", Float) = 0.0
        _HurtMap("Hurt Map", 2D) = "black" {}   // 注意WarpMode要为Clamp，不能是Repeat。且边界灰度为0
        _HurtColor("Hurt Color", Color) = (1,1,1,1)
        [HideInInspector]_HurtParameter("Hurt Parameters", vector) = (0,0,0,0)
        
    }
    SubShader
    {
        HLSLINCLUDE
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                // 颜色
                half4 _BaseColor;
                // 金属度
                float _Metallic;
                // 光滑度
                float _Smoothness;
                // 法线缩放
                half _BumpScale;
                // 遮挡强度
                half _OcclusionStrength;
                // 透明度裁切
                float _Cutoff;

                // 受伤特效
                // 受伤颜色
                half4 _HurtColor;
                // 受伤时间段(x:start y:1/duration)
                float2 _HurtParameter;
            CBUFFER_END

            // _BaseMap
            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            // _BumpMap
            TEXTURE2D(_BumpMap);
            SAMPLER(sampler_BumpMap);

            // _MetallicGlossMap
            TEXTURE2D(_MetallicGlossMap);
            SAMPLER(sampler_MetallicGlossMap);

            // _OcclusionMap
            TEXTURE2D(_OcclusionMap);
            SAMPLER(sampler_OcclusionMap);

            half4 SampleAlbedoAlpha(float2 uv, TEXTURE2D_PARAM(albedoAlphaMap, sampler_albedoAlphaMap))
            {
                return SAMPLE_TEXTURE2D(albedoAlphaMap, sampler_albedoAlphaMap, uv);
            }

            half Alpha(half albedoAlpha, half4 color, half cutoff)
            {
            #if !defined(_SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A) && !defined(_GLOSSINESS_FROM_BASE_ALPHA)
                half alpha = albedoAlpha * color.a;
            #else
                half alpha = color.a;
            #endif
            
            #if defined(_ALPHATEST_ON)
                clip(alpha - cutoff);
            #endif
            
                return alpha;
            }

            half3 SampleNormal(float2 uv, TEXTURE2D_PARAM(bumpMap, sampler_bumpMap), half scale = 1.0h)
            {
                #ifdef _NORMALMAP
                    half4 n = SAMPLE_TEXTURE2D(bumpMap, sampler_bumpMap, uv);
                    #if BUMP_SCALE_NOT_SUPPORTED
                        return UnpackNormal(n);
                    #else
                        return UnpackNormalScale(n, scale);
                    #endif
                #else
                    return half3(0.0h, 0.0h, 1.0h);
                #endif
            }

            half SampleOcclusion(float2 uv)
            {
                #ifdef _OCCLUSIONMAP
                // TODO: Controls things like these by exposing SHADER_QUALITY levels (low, medium, high)
                #if defined(SHADER_API_GLES)
                    return SAMPLE_TEXTURE2D(_OcclusionMap, sampler_OcclusionMap, uv).g;
                #else
                    half occ = SAMPLE_TEXTURE2D(_OcclusionMap, sampler_OcclusionMap, uv).g;
                    return LerpWhiteTo(occ, _OcclusionStrength);
                #endif
                #else
                    return 1.0;
                #endif
            }

            #ifdef _SPECULAR_SETUP
                #define SAMPLE_METALLICSPECULAR(uv) SAMPLE_TEXTURE2D(_SpecGlossMap, sampler_SpecGlossMap, uv)
            #else
                #define SAMPLE_METALLICSPECULAR(uv) SAMPLE_TEXTURE2D(_MetallicGlossMap, sampler_MetallicGlossMap, uv)
            #endif

            half4 SampleMetallicSpecGloss(float2 uv, half albedoAlpha)
            {
                half4 specGloss;

            #ifdef _METALLICSPECGLOSSMAP
                specGloss = SAMPLE_METALLICSPECULAR(uv);
                #ifdef _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
                    specGloss.a = albedoAlpha * _Smoothness;
                #else
                    specGloss.a *= _Smoothness;
                #endif
            #else // _METALLICSPECGLOSSMAP
                #if _SPECULAR_SETUP
                    specGloss.rgb = _SpecColor.rgb;
                #else
                    specGloss.rgb = _Metallic.rrr;
                #endif

                #ifdef _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
                    specGloss.a = albedoAlpha * _Smoothness;
                #else
                    specGloss.a = _Smoothness;
                #endif
            #endif

                return specGloss;
            }

        ENDHLSL
        
        
        Tags{"RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "UniversalMaterialType" = "Lit" }

        // ------------------------------------------------------------------
        //  Forward pass. Shades all light in a single pass. GI + emission + Fog
        Pass
        {
            // Lightmode matches the ShaderPassName set in UniversalRenderPipeline.cs. SRPDefaultUnlit and passes with
            // no LightMode tag are also rendered by Universal Render Pipeline
            Name "ForwardLit"
            Tags{"LightMode" = "UniversalForward"}

            Blend[_SrcBlend][_DstBlend]
            ZWrite[_ZWrite]
            Cull[_Cull]

            HLSLPROGRAM

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Assets/Scripts/URP/ShaderLibrary/Math.hlsl"

            // 受伤特效
            // HurtMap
            TEXTURE2D(_HurtMap);
            SAMPLER(sampler_HurtMap);

            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
                float2 lightmapUV : TEXCOORD1;  // lightmap
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                half4 color : COLOR;
                float2 uv : TEXCOORD0;
                DECLARE_LIGHTMAP_OR_SH(lightmapUV, vertexSH, 1);   // lightmap
                float3 positionWS : TEXCOORD2;
                float3 normalWS : TEXCOORD3;
                float3 tangentWS : TEXCOORD4;
                float3 bitangentWS : TEXCOORD5;
                float3 viewDirectionWS : TEXCOORD6;
                #ifdef _ADDITIONAL_LIGHTS_VERTEX
				    half4 fogFactorAndVertexLight : TEXCOORD7; // x: fogFactor, yzw: vertex light
				#else
				    half  fogFactor : TEXCOORD7;
				#endif
                float4 shadowCoord : TEXCOORD8;

                // 受伤特效
                float2 hurtParam : TEXCOORD9;

                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings Vertex(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input)
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

            	VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS);

                half fogFactor = 0;
			    #if !defined(_FOG_FRAGMENT)
			        fogFactor = ComputeFogFactor(vertexInput.positionCS.z);
			    #endif

                output.positionWS = vertexInput.positionWS;
                output.positionCS = vertexInput.positionCS;
    
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);
                output.normalWS = normalInput.normalWS;
                output.tangentWS = normalInput.tangentWS;
                output.bitangentWS = normalInput.bitangentWS;
                output.viewDirectionWS = GetCameraPositionWS() - output.positionWS;
    
                output.uv.xy = TRANSFORM_TEX(input.uv, _BaseMap);
    
                // lightmap
                OUTPUT_LIGHTMAP_UV(input.lightmapUV, unity_LightmapST, output.lightmapUV);
                OUTPUT_SH(output.normalWS, output.vertexSH);
                // Shadow
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
				    output.shadowCoord = GetShadowCoord(vertexInput);
				#endif
            	// fogFactor
				#ifdef _ADDITIONAL_LIGHTS_VERTEX
				    half3 vertexLight = VertexLighting(vertexInput.positionWS, output.normalWS);
					output.fogFactorAndVertexLight = half4(fogFactor, vertexLight);
				#else
				    output.fogFactor = fogFactor;
				#endif

                // 受伤特效
                #if _USE_HURT
                    float hurtTimestamp = _HurtParameter.x;
                    float oneDivHurtDuration = _HurtParameter.y;
                    output.hurtParam = float2((_Time.y-hurtTimestamp)*oneDivHurtDuration, 0.5);
                #endif
                
                return output;
            }

            half4 Fragment(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                
                half4 albedoAlpha = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv.xy);

                // // Must match Universal ShaderGraph master node
                // struct SurfaceData
                // {
                //     half3 albedo;
                //     half3 specular;
                //     half  metallic;
                //     half  smoothness;
                //     half3 normalTS;
                //     half3 emission;
                //     half  occlusion;
                //     half  alpha;
                //     half  clearCoatMask;
                //     half  clearCoatSmoothness;
                // };
                SurfaceData surfaceData;
                half4 specGloss = SampleMetallicSpecGloss(input.uv, albedoAlpha.a);
                surfaceData.albedo = albedoAlpha.rgb * _BaseColor.rgb;
                #if _SPECULAR_SETUP
                    outSurfaceData.metallic = 1.0h;
                    outSurfaceData.specular = specGloss.rgb;
                #else
                    surfaceData.metallic = specGloss.r;
                    surfaceData.specular = half3(0.0h, 0.0h, 0.0h);
                #endif
                // surfaceData.metallic = _Metallic;
                surfaceData.smoothness = specGloss.a;
                surfaceData.specular = 0;
                surfaceData.normalTS = SampleNormal(input.uv, TEXTURE2D_ARGS(_BumpMap, sampler_BumpMap), _BumpScale);//half3(0, 1, 0);
                surfaceData.occlusion = SampleOcclusion(input.uv);;
                surfaceData.emission = 0;
                surfaceData.alpha = Alpha(albedoAlpha.a, _BaseColor, _Cutoff);
                surfaceData.clearCoatMask = 0;
                surfaceData.clearCoatSmoothness = 0;

                // struct InputData
                // {
                //     float3  positionWS;
                //     half3   normalWS;
                //     half3   viewDirectionWS;
                //     float4  shadowCoord;
                //     half    fogCoord;
                //     half3   vertexLighting;
                //     half3   bakedGI;
                //     float2  normalizedScreenSpaceUV;
                //     half4   shadowMask;
                // };
                InputData inputData;
                inputData.positionWS = input.positionWS;
                #if defined(_NORMALMAP) || defined(_DETAIL)
                    inputData.normalWS = TransformTangentToWorld(surfaceData.normalTS, half3x3(input.tangentWS, input.bitangentWS, input.normalWS));
                #else
                    inputData.normalWS = input.normalWS;
                #endif
                inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);
                inputData.viewDirectionWS = SafeNormalize(input.viewDirectionWS);
                #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
                    inputData.shadowCoord = input.shadowCoord;
                #elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
                    inputData.shadowCoord = TransformWorldToShadowCoord(inputData.positionWS);
                #else
                    inputData.shadowCoord = float4(0, 0, 0, 0);
                #endif
                #ifdef _ADDITIONAL_LIGHTS_VERTEX
                    inputData.fogCoord = input.fogFactorAndVertexLight.x;
                    inputData.vertexLighting = input.fogFactorAndVertexLight.yzw;
                #else
			        inputData.fogCoord = input.fogFactor;
            	#endif
                inputData.bakedGI = SAMPLE_GI(input.lightmapUV, input.vertexSH, inputData.normalWS);
                inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(input.positionCS);
                inputData.shadowMask = SAMPLE_SHADOWMASK(input.lightmapUV);

                // half4 UniversalFragmentPBR(InputData inputData, half3 albedo, half metallic, half3 specular, half smoothness, half occlusion, half3 emission, half alpha)
                half4 finalColor = UniversalFragmentPBR(inputData, surfaceData);
                finalColor.rgb = MixFog(finalColor.rgb, inputData.fogCoord);
                finalColor.a = 1;   //OutputAlpha(finalColor, _Surface);

                // 受伤特效
                #if _USE_HURT
                    half4 hurtColor = lerp(float4(1,1,1,1), _HurtColor, SAMPLE_TEXTURE2D(_HurtMap, sampler_HurtMap, input.hurtParam).r);
                    finalColor *= hurtColor;
                    // finalColor *= _HurtColor;
                #endif
                
                
                return finalColor;
            }

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _ALPHAPREMULTIPLY_ON
            #pragma shader_feature_local_fragment _EMISSION
            #pragma shader_feature_local_fragment _METALLICSPECGLOSSMAP
            #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature_local_fragment _OCCLUSIONMAP
            #pragma shader_feature_local _PARALLAXMAP
            #pragma shader_feature_local _ _DETAIL_MULX2 _DETAIL_SCALED
            #pragma shader_feature_local_fragment _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature_local_fragment _ENVIRONMENTREFLECTIONS_OFF
            #pragma shader_feature_local_fragment _SPECULAR_SETUP
            #pragma shader_feature_local _RECEIVE_SHADOWS_OFF
            // 受伤特效
            #pragma shader_feature_local _USE_HURT

            // -------------------------------------
            // Universal Pipeline keywords
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
            #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
            #pragma multi_compile _ SHADOWS_SHADOWMASK

            // -------------------------------------
            // Unity defined keywords
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile_fog

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON

            #pragma vertex Vertex
            #pragma fragment Fragment


            ENDHLSL
        }

        Pass
        {
            Name "ShadowCaster"
            Tags{"LightMode" = "ShadowCaster"}

            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull[_Cull]

            HLSLPROGRAM

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON

            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment

            // #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"

            #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
            ENDHLSL
        }

        
        Pass
        {
            Name "DepthOnly"
            Tags{"LightMode" = "DepthOnly"}

            ZWrite On
            ColorMask 0
            Cull[_Cull]

            HLSLPROGRAM

            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON

            #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"
            ENDHLSL
        }

        // This pass is used when drawing to a _CameraNormalsTexture texture
        Pass
        {
            Name "DepthNormals"
            Tags{"LightMode" = "DepthNormals"}

            ZWrite On
            Cull[_Cull]

            HLSLPROGRAM

            #pragma vertex DepthNormalsVertex
            #pragma fragment DepthNormalsFragment

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON

            #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthNormalsPass.hlsl"
            ENDHLSL
        }

//        // This pass it not used during regular rendering, only for lightmap baking.
//        Pass
//        {
//            Name "Meta"
//            Tags{"LightMode" = "Meta"}
//
//            Cull Off
//
//            HLSLPROGRAM
//
//            #pragma vertex UniversalVertexMeta
//            #pragma fragment UniversalFragmentMeta
//
//            #pragma shader_feature_local_fragment _SPECULAR_SETUP
//            #pragma shader_feature_local_fragment _EMISSION
//            #pragma shader_feature_local_fragment _METALLICSPECGLOSSMAP
//            #pragma shader_feature_local_fragment _ALPHATEST_ON
//            #pragma shader_feature_local_fragment _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
//            #pragma shader_feature_local _ _DETAIL_MULX2 _DETAIL_SCALED
//
//            #pragma shader_feature_local_fragment _SPECGLOSSMAP
//
//            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
//            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitMetaPass.hlsl"
//
//            ENDHLSL
//        }
        
        
    }
    
    CustomEditor "Zack.UniversalRP.ShaderGUI.LitShader"

}
