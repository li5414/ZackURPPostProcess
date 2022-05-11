#ifndef ZACK_URP_MATH_INCLUDE
#define ZACK_URP_MATH_INCLUDE

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderVariablesFunctions.hlsl"

// 模型空间(ObjectSpace) -> 相机空间(ViewSpace)
float4 TransformObjectToView(float3 positionOS)
{
    return mul(GetWorldToViewMatrix(), mul(GetObjectToWorldMatrix(), float4(positionOS, 1.0)));
}

// 获取屏幕坐标
float4 ComputeGrabScreenPos(float4 positionCS) 
{
#if UNITY_UV_STARTS_AT_TOP
    float scale = -1.0;
    #else
    float scale = 1.0;
#endif
    float4 o = positionCS * 0.5f;
    o.xy = float2(o.x, o.y*scale) + o.w;
#ifdef UNITY_SINGLE_PASS_STEREO
    o.xy = TransformStereoScreenSpaceTex(o.xy, positionCS.w);
#endif
    o.zw = positionCS.zw;
    return o;
}

#endif // ZACK_URP_MATH_INCLUDE