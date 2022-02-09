Shader "ZackURP/Effect/CenterDissolve"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}

		[Vector2(1)] _DissolveCenter("Dissolve Center Position", Vector) = (0,1,0)
		_DissolveFactor("World Space Dissolve Factor", float) = 0.1

		_DissTex("Dissolve Texture", 2D) = "white"{}

		_EdgeWidth("Edge Wdith", float) = 0
		[HDR]_DissolveEdgeColor("Dissolve Edge Color", Color) = (0.0, 0.0, 0.0, 0)
		_Smoothness("Smoothness", Range(0.001, 1)) = 0.2

		[ScaleOffset] _DissTex_Scroll("Scroll", Vector) = (0, 0, 0, 0)

		_Clip("Clip", float) = 0
    }
    SubShader
    {
        HLSLINCLUDE
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _DissTex_ST;
                
                // 消融中心位置
                float3 _DissolveCenter;
                // 消融因子
                half _DissolveFactor;
                // 裁剪值
                half _Clip;
                // 边缘平滑度
                half _Smoothness;
                // 边缘颜色
                float4 _DissolveEdgeColor;
                // 边缘宽度
                float _EdgeWidth;
                // 消融纹理滚动
                float2 _DissTex_Scroll;
            CBUFFER_END
            
            // _MainTex
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            // _DissTex
            TEXTURE2D(_DissTex);
            SAMPLER(sampler_DissTex);
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            struct Varyings
            {
                half4 positionCS : SV_POSITION;
                half4 uv : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float3 dissolveCenterWS : TEXCOORD2;
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            // vertex Shader
            Varyings Vertex(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                
                // 有的时候，溶解需要有一定的方向性，比如纸张从左下角往右上角燃烧的效果。设clip = dissolve - _Clip，
                // 那么我们可以在clip的基础上，给一个方向相关的因子，即clip = clip + worldFactor。其中worldFactor就是方向相关的因子。
                output.positionWS = TransformObjectToWorld(input.positionOS.xyz);
                output.positionCS = mul(GetWorldToHClipMatrix(), float4(output.positionWS.xyz, 1.0));
                output.uv.xy = TRANSFORM_TEX(input.texcoord, _MainTex);
				output.uv.zw = TRANSFORM_TEX(input.texcoord, _DissTex) + frac(_DissTex_Scroll.xy * _Time.x);
				output.dissolveCenterWS = TransformObjectToWorld(_DissolveCenter);
                
                return output;
            }
    
            // fragment Shader
            half4 Fragment(Varyings input) : SV_Target
            {
                // albedo
                half4 albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv.xy);
            
                // dissolve
				half dissolve = SAMPLE_TEXTURE2D(_DissTex, sampler_DissTex, input.uv.zw).r;
				dissolve = dissolve + distance(input.positionWS, input.dissolveCenterWS) * _DissolveFactor;
				clip(dissolve - _Clip);
                // edge
				float edge_area = saturate(1 - saturate((dissolve - _Clip - _EdgeWidth) / _Smoothness));
				edge_area *= _DissolveEdgeColor.a;
				// color
                half4 color = albedo;
                color.rgb = color.rgb * (1 - edge_area) + _DissolveEdgeColor.rgb * edge_area;

                return color;
            }
        ENDHLSL
    
		Tags { "Queue" = "Geometry" }

        Pass
        {
            Tags { "RenderPipeline" = "UniversalPipeline" "RenderType" = "Transparent" }
            Blend SrcAlpha OneMinusSrcAlpha
            HLSLPROGRAM
                #pragma vertex Vertex
                #pragma fragment Fragment
            ENDHLSL
        }
      
    }
}
