Shader "ZackURP/Effect/SpellIndicator"
{
    Properties 
    {
        _Color ("Main Color", Color) = (1,1,1,1)
		_BaseMap ("Main Texture", 2D) = "black" {}
    }
    SubShader 
    {
        HLSLINCLUDE
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Assets/Scripts/URP/ShaderLibrary/Math.hlsl"

            struct Attributes
		    {
			    float4 positionOS : POSITION;
			    float2 texcoord : TEXCOORD0;
			    UNITY_VERTEX_INPUT_INSTANCE_ID
		    };
		    struct Varyings
		    {
			    half4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
		    };
		    
            CBUFFER_START(UnityPerMaterial)
                half4 _Color;
            CBUFFER_END
		    
		    // _BaseMap
            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            Varyings Vertex(Attributes input)
            {
                Varyings output;
            	UNITY_SETUP_INSTANCE_ID(input);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.texcoord;
            	
                return output;
            }

            half4 Fragment(Varyings input) : SV_Target
            {
            	half4 tex = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv);
                half4 finalColor = tex * _Color;

            	return finalColor;
            }
        
        ENDHLSL
        
        Tags { "IgnoreProjector"="True" "Queue"="Transparent" "RenderType"="Transparent" }
        Pass 
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Back
            ZWrite Off
        	ZTest Off
            HLSLPROGRAM
                #pragma vertex Vertex
                #pragma fragment Fragment
            ENDHLSL
            
        }
        
    }
}