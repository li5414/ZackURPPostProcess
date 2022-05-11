Shader "ZackURP/Effect/Blood" 
{
	Properties 
	{
		_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_ColorStrength ("Color strength", Float) = 1.0
		_MainTex ("Particle Texture", 2D) = "white" {}
		_InvFade ("Soft Particles Factor", Float) = 0.5
	}

	SubShader 
	{
		HLSLINCLUDE
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Assets/Scripts/URP/ShaderLibrary/Math.hlsl"

			CBUFFER_START(UnityPerMaterial)
				half4 _TintColor;
				half _ColorStrength;
				float _InvFade;
				float4 _MainTex_ST;
            CBUFFER_END

			// MainTex
		    TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
			// Depth
			TEXTURE2D_X_FLOAT(_CameraDepthTexture);
			SAMPLER(sampler_CameraDepthTexture);

			struct Attributes
			{
				float4 positionOS : POSITION;
				half4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct Varyings
			{
				float4 positionCS : POSITION;
				half4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				#ifdef SOFTPARTICLES_ON
					float4 positionNDC : TEXCOORD1;
				#endif
			};

			Varyings Vertex (Attributes input)
			{
				Varyings output = (Varyings)0;

				output.positionCS = TransformObjectToHClip(input.positionOS);
				#ifdef SOFTPARTICLES_ON
					output.positionNDC = GetVertexPositionInputs(input.positionOS).positionNDC;	// ComputeScreenPos
	                // COMPUTE_EYEDEPTH(o.projPos.z); => COMPUTE_EYEDEPTH(o) o = -UnityObjectToViewPos(v.vertex).z
	                output.positionNDC.z = -TransformObjectToView(input.positionOS).z;
				#endif
				output.color = input.color;
				output.texcoord = TRANSFORM_TEX(input.texcoord, _MainTex);
				
				return output;
			}
			
			half4 Fragment (Varyings input) : SV_Target
			{
				#ifdef SOFTPARTICLES_ON
					if(_InvFade > 0.0001)
					{
		                float sceneZ = max(0,LinearEyeDepth (SAMPLE_TEXTURE2D_X(_CameraDepthTexture, sampler_CameraDepthTexture, input.positionNDC).r, _ZBufferParams) - _ProjectionParams.g);
		                float partZ = max(0,input.positionNDC.z - _ProjectionParams.g);
						half fade = saturate (_InvFade * (sceneZ-partZ));
						input.color.a *= fade;
					}
				#endif

				half4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.texcoord);
				half4 col = 2.0f * input.color * tex;
				col.rgb *= _TintColor.rgb;
				col.a = saturate(col.a * _TintColor.a);

				return half4(col.rgb * _ColorStrength, col.a);
			}
		
		ENDHLSL
		
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off 
		ZWrite Off

		Pass
		{	
			Tags { "RenderPipeline" = "UniversalPipeline" "LightMode"="UniversalForward" }

			HLSLPROGRAM
	            #pragma vertex Vertex
	            #pragma fragment Fragment
				#pragma multi_compile_particles
				#pragma fragmentoption ARB_precision_hint_fastest
			ENDHLSL 
		}
	}
	
}
