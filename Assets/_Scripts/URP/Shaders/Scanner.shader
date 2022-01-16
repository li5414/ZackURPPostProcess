Shader "Zack_URP_Post-Process/Scanner"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_ScannerCenter ("Scanner Center Position", vector) = (0, 0, 0, 0)
		_ScannerParams ("Scanner Parameters", vector) = (1, 1, 0, 0)
		_ScannerColor("Scanner Color", Color) = (0, 1, 0, 0)
    }
    SubShader
    {
		Tags{ "RenderPipeline" = "UniversalPipeline" "IgnoreProjector" = "True"}

		HLSLINCLUDE

		#pragma multi_compile _ _SCANNER_TYPE_CYLINDER _SCANNER_TYPE_CUBE

		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

		struct Attributes
		{
			float4 positionOS   : POSITION;
			float2 texcoord : TEXCOORD0;
			UNITY_VERTEX_INPUT_INSTANCE_ID
		};
		struct Varyings
		{
			half4  positionCS   : SV_POSITION;
			half2  uv           : TEXCOORD0;
			UNITY_VERTEX_OUTPUT_STEREO
		};

		// Depth
		TEXTURE2D_X_FLOAT(_CameraDepthTexture);
		SAMPLER(sampler_CameraDepthTexture);
		// MainTex
		TEXTURE2D(_MainTex);
		SAMPLER(sampler_MainTex);
		float4 _MainTex_ST;
		// ɨ�����ĵ�λ��(_ScannerCenter: ����ռ�)
		float3 _ScannerCenter;
		// ɨ�����(_ScannerCenter x:ɨ�跶Χ(�뾶); y:ɨ���߿�� z:ɨ����������͸����)
		float3 _ScannerParams;
		// ɨ����ɫ
		float4 _ScannerColor;

		Varyings vert(Attributes input)
		{
			Varyings output;
			UNITY_SETUP_INSTANCE_ID(input);
			UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

			output.positionCS = TransformObjectToHClip(input.positionOS);
			output.uv = TRANSFORM_TEX(input.texcoord, _MainTex);
			return output;
		}

		float4 frag(Varyings input) : SV_Target
		{
			// ������Ȼ�ȡƬ����������
			float deviceDepth = SAMPLE_TEXTURE2D_X(_CameraDepthTexture, sampler_CameraDepthTexture, input.uv.xy).r;
#if !UNITY_REVERSED_Z
			deviceDepth = 2 * deviceDepth - 1; 
#endif
			float3 positionWS = ComputeWorldSpacePosition(input.uv, deviceDepth, UNITY_MATRIX_I_VP);

			// ����ɨ�跶Χ
			// ɨ���ڻ��뾶
			float radiusInner = _ScannerParams.x;
			// ɨ���⻷�뾶
			float radiusOuter = radiusInner + _ScannerParams.y;
			// ��ǰƬ�ε�ɨ�����ĵ����
#if _SCANNER_TYPE_CYLINDER
			float curDistance = distance(_ScannerCenter.xz, positionWS.xz);
#elif _SCANNER_TYPE_CUBE
			float curDistance = max(abs(_ScannerCenter.x - positionWS.x), abs(_ScannerCenter.z - positionWS.z));
#else
			float curDistance = distance(_ScannerCenter.xyz, positionWS);
#endif
			// ���ݾ���������
			float validDistance = lerp(radiusInner, curDistance, step(curDistance, radiusOuter));
			validDistance = lerp(radiusInner + _ScannerParams.z, validDistance, step(radiusInner + _ScannerParams.z, curDistance));
			float percent = (validDistance - radiusInner) / _ScannerParams.y;

			float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);

			return  lerp(color, _ScannerColor, percent);	//float4(linearDepth, linearDepth, linearDepth, linearDepth);
		}

		ENDHLSL

		Pass
        {
			Name "ScreenSpaceShadows"
			ZTest Always
			ZWrite Off
			Cull Off

			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			ENDHLSL
        }

    }
}
