Shader "ZackURP/Effect/AttackRangeCircle"
{
    Properties 
    {
		_AttackRangeCenter ("Attack Range Center Position", vector) = (0, 0, 0, 0)
		_AttackRangeParams ("Attack Range Parameters", vector) = (1, 1, 0, 0)
		_AttackRangeColor("Attack Range Color", Color) = (0, 1, 0, 0)
    }
    SubShader 
    {
        HLSLINCLUDE
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Assets/Scripts/URP/ShaderLibrary/Math.hlsl"

			CBUFFER_START(UnityPerMaterial)
				// 攻击范围中心点位置(_AttackRangeCenter: 世界空间)
				float4 _AttackRangeCenter;
				// 攻击范围参数(_AttackRangeCenter x:攻击范围范围(半径); y:攻击范围线宽度 z:攻击范围中心区域透明度)
				float4 _AttackRangeParams;
				// 攻击范围颜色
				half4 _AttackRangeColor;
            CBUFFER_END
			
			// _CameraDepthTexture
			TEXTURE2D_X_FLOAT(_CameraDepthTexture);
			SAMPLER(sampler_CameraDepthTexture);

            struct Attributes
		    {
			    float4 positionOS : POSITION;
			    float2 texcoord : TEXCOORD0;
			    UNITY_VERTEX_INPUT_INSTANCE_ID
		    };
		    struct Varyings
		    {
			    half4 positionCS : SV_POSITION;
		        half3 positionWS : TEXCOORD0;
				float4 grabPos : TEXCOORD2;
			    UNITY_VERTEX_OUTPUT_STEREO
		    };

            Varyings Vertex(Attributes input)
            {
                Varyings output;
            	UNITY_SETUP_INSTANCE_ID(input);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

            	VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);

                output.positionCS = vertexInput.positionCS;
            	output.positionWS = vertexInput.positionWS;
            	// 获取抓屏屏幕坐标
                output.grabPos = ComputeGrabScreenPos(output.positionCS);
            	
                return output;
            }

            half4 Fragment(Varyings input) : SV_Target
            {
            	float2 uv = input.grabPos.xy / input.grabPos.w;
				// 根据深度获取片段世界坐标
				float deviceDepth = SAMPLE_TEXTURE2D_X(_CameraDepthTexture, sampler_CameraDepthTexture, uv).r;
				#if !UNITY_REVERSED_Z
					deviceDepth = 2 * deviceDepth - 1; 
				#endif
				float3 positionWS = ComputeWorldSpacePosition(uv, deviceDepth, UNITY_MATRIX_I_VP);
            	
                // 计算攻击范围
				// 攻击范围外环半径
				float radiusOuter = _AttackRangeParams.x;
				// 攻击范围内环半径
				float radiusInner = radiusOuter - _AttackRangeParams.y;
				// 当前片段到攻击范围中心点距离
				// float curDistance = distance(_AttackRangeCenter.xz, positionWS.xz);
	           	float curDistance = distance(_AttackRangeCenter.xyz, positionWS);
				// 根据距离做裁切
				float validDistance = lerp(radiusInner, curDistance, step(curDistance, radiusOuter));
				validDistance = lerp(radiusInner + _AttackRangeParams.z, validDistance, step(radiusInner + _AttackRangeParams.z, curDistance));
				float percent = (validDistance - radiusInner) / _AttackRangeParams.y;
				percent = lerp(0, percent, step(positionWS.y, radiusInner)); 

            	half4 finalColor = half4(_AttackRangeColor.rgb, percent);
            	return finalColor;
            }
        
        ENDHLSL
        
        Tags { "IgnoreProjector"="True" "Queue"="Transparent" "RenderType"="Transparent" }
        Pass 
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off
        	ZTest Off
            HLSLPROGRAM
                #pragma vertex Vertex
                #pragma fragment Fragment
            ENDHLSL
            
        }
        
    }
}