Shader "Zack_URP_Post-Process/Scanner"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_ScannerCenter ("Scanner Center Position", vector) = (0, 0, 0, 0)
		_ScannerParams ("Scanner Parameters", vector) = (1, 1, 0, 0)
		_ScannerColor("Scanner Color", Color) = (0, 1, 0, 0)
		_ScannerTex("Texture", 2D) = "white" {}
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
		// 扫描中心点位置(_ScannerCenter: 世界空间)
		float3 _ScannerCenter;
		// 扫描参数(_ScannerCenter x:扫描范围(半径); y:扫描线宽度 z:扫描中心区域透明度)
		float3 _ScannerParams;
		// 扫描颜色
		float4 _ScannerColor;
		// 扫描线纹理
		TEXTURE2D(_ScannerTex);
		SAMPLER(sampler_ScannerTex);

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
			// 根据深度获取片段世界坐标
			float deviceDepth = SAMPLE_TEXTURE2D_X(_CameraDepthTexture, sampler_CameraDepthTexture, input.uv.xy).r;
#if !UNITY_REVERSED_Z
			deviceDepth = 2 * deviceDepth - 1; 
#endif
			float3 positionWS = ComputeWorldSpacePosition(input.uv, deviceDepth, UNITY_MATRIX_I_VP);

			// 计算扫描范围
			// 扫描内环半径
			float radiusInner = _ScannerParams.x;
			// 扫描外环半径
			float radiusOuter = radiusInner + _ScannerParams.y;
			// 当前片段到扫描中心点距离
#if _SCANNER_TYPE_CYLINDER
			float curDistance = distance(_ScannerCenter.xz, positionWS.xz);
#elif _SCANNER_TYPE_CUBE
			float curDistance = max(abs(_ScannerCenter.x - positionWS.x), abs(_ScannerCenter.z - positionWS.z));
#else
			float curDistance = distance(_ScannerCenter.xyz, positionWS);
#endif
			// 根据距离做裁切
			float validDistance = lerp(radiusInner, curDistance, step(curDistance, radiusOuter));
			validDistance = lerp(radiusInner + _ScannerParams.z, validDistance, step(radiusInner + _ScannerParams.z, curDistance));
			float percent = (validDistance - radiusInner) / _ScannerParams.y;

			//float3 modulo = pixelWorldPos - _MeshWidth*floor(pixelWorldPos/_MeshWidth);
			//modulo = modulo/_MeshWidth;
			//SAMPLE_TEXTURE2D(_ScannerTex, sampler_ScannerTex, scannerUV)

			float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
			float4 scannerColor = _ScannerColor;
			float2 scannerUV = abs((positionWS-_ScannerCenter).xz);
			scannerColor = _ScannerColor * SAMPLE_TEXTURE2D(_ScannerTex, sampler_ScannerTex, scannerUV).r;

			return  lerp(color, scannerColor, percent);	//float4(linearDepth, linearDepth, linearDepth, linearDepth);
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
