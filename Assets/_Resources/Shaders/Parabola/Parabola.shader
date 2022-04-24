Shader "ZackURP/Effect/Parabola" {
 Properties {
        _Color ("Color", Color) = (0.5,0.5,0.5,0.5)
        // mainTex
        _MainTex ("MainTex", 2D) = "white" {}
        // maskTex
        _MaskTex ("MaskTex", 2D) = "white" {} 
        // mainTex uv移动速度
        _MainSpeedU ("MainSpeedU", Float ) = 0
        _MainSpeedV ("MainSpeedV", Float ) = 0
        // maskTex uv移动速度
        _MaskSpeedU ("MaskSpeedU", Float ) = 0
        _MaskSpeedV ("MaskSpeedV", Float ) = 0 
    }
    SubShader {
        HLSLINCLUDE
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            CBUFFER_START(UnityPerMaterial)
                half4 _Color;
        
                float4 _MainTex_ST;
                float4 _MaskTex_ST;

                half _MainSpeedU;
                half _MainSpeedV;
        
                half _MaskSpeedU;
                half _MaskSpeedV;
            CBUFFER_END

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            TEXTURE2D(_MaskTex);
            SAMPLER(sampler_MaskTex);

            struct Attributes {
                float4 positionOS : POSITION;
                float2 texcoord0 : TEXCOORD0;
                half4 vertexColor : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            struct Varyings {
                float4 positionCS : SV_POSITION;
                float2 texcoord0 : TEXCOORD0;
                float2 movingUV :TEXCOORD1;
                float2 movingUV1 :TEXCOORD2;
                half4 vertexColor : TEXCOORD3;
                half4 color:TEXCOORD4;
            };

            Varyings Vertex(Attributes input)
            {
                Varyings output;
                output.texcoord0 = input.texcoord0;
                output.vertexColor = input.vertexColor;

                float4 time = _Time;
                output.movingUV = half2(_MainSpeedU,_MainSpeedV) * time.g + input.texcoord0;
                output.movingUV =TRANSFORM_TEX(output.movingUV, _MainTex);

                output.movingUV1 = half2(_MaskSpeedU,_MaskSpeedV) * time.g + input.texcoord0;
                output.movingUV1 =TRANSFORM_TEX(output.movingUV1, _MaskTex);

                output.color = input.vertexColor*_Color*2;
                output.positionCS = TransformObjectToHClip(input.positionOS);
                return output;
            }

            half4 Fragment(Varyings input) : SV_Target
            {
                half4 _MainTexVar = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.movingUV);
                half4 _MaskTex_var = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, input.movingUV1);
                half3 finalColor = _MainTexVar.rgb * input.color.rgb;
           
                return half4(finalColor, _MainTexVar.a * input.color.a* _MaskTex_var.r  );
            }
        
        ENDHLSL
        
        Tags { "IgnoreProjector"="True" "Queue"="Transparent" "RenderType"="Transparent" }
        Pass {
       
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            HLSLPROGRAM
                #pragma vertex Vertex
                #pragma fragment Fragment
            ENDHLSL
            
        }
        
    }
}
