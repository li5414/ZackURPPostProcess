Shader "ZackURP/Effect/DirectionalDissolve"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}

		[Vector3(1)] _DissolveDir("Dissolve Direction", Vector) = (0,1,0)
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
                
                // 消融方向
                float3 _DissolveDir;
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
                float worldFactor : TEXCOORD1;
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
                output.uv.xy = TRANSFORM_TEX(input.texcoord, _MainTex);
				output.uv.zw = TRANSFORM_TEX(input.texcoord, _DissTex) + frac(_DissTex_Scroll.xy * _Time.x);

                // 原理解析:
                // 有的时候，溶解需要有一定的方向性，比如纸张从左下角往右上角燃烧的效果。设clip = dissolve - _Clip，那么我们可以在clip的基础上，给一个方向相关的因子，
                // 即clip = clip + worldFactor。其中worldFactor就是方向相关的因子。
                
                // 其中originPositionWS为物体质心所在的坐标值，因为世界变换矩阵的w行向量即为世界位置（详细请搜索世界矩阵介绍和推导相关文章）。positionWS为当前顶点所处世界位置，在Unity里需要注意这里不允许动态合批，否则positionWS就失效了。
                // 那么positionWS - originPositionWS就可以求得顶点偏离模型质心的偏移向量，最后偏移向量在_DissolveDir方向上的投影长度就是我们所需要的方向因子，因为投影越长，那么在_DissolveDir方向上像素离质心越远。
                
                // 模型中心点的世界坐标
				float3 originPositionWS = float3(GetObjectToWorldMatrix()[0].w, GetObjectToWorldMatrix()[1].w, GetObjectToWorldMatrix()[2].w);
				// 顶点到中心点在世界坐标系下的偏移
				float3 offset = positionWS - originPositionWS;
				output.worldFactor = dot(normalize(_DissolveDir), offset);
                
                return output;
            }
    
            // fragment Shader
            half4 Fragment(Varyings input) : SV_Target
            {
                // albedo
                half4 albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv.xy);
            
                // dissolve
				half dissolve = SAMPLE_TEXTURE2D(_DissTex, sampler_DissTex, input.uv.zw).r;
				dissolve = dissolve + input.worldFactor * _DissolveFactor;
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
