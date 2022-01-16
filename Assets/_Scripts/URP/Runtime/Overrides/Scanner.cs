using System;
using UnityEngine;

namespace UnityEngine.Rendering.Universal
{
    [System.Serializable, VolumeComponentMenu("ZackPostProcess/Scanner")]
    public class Scanner : VolumeComponent, IPostProcessComponent
    {
        // 扫描类型
        public enum ScannerType
        {
            // 球体
            Sphere,
            // 圆柱体
            Cylinder,
            // 立方体
            Cube
        }
        [Serializable]
        public sealed class ScannerTypeParameter : VolumeParameter<ScannerType> { public ScannerTypeParameter(Scanner.ScannerType value, bool overrideState = false) : base(value, overrideState) { } }

        // 后处理开关
        [Tooltip("开启后处理")]
        public BoolParameter enabled = new BoolParameter(true, true);
        // 扫描类型
        [Tooltip("扫描类型")]
        public ScannerTypeParameter type = new ScannerTypeParameter(ScannerType.Sphere, true);
        // 中心位置
        [Tooltip("全息扫描中心位置")]
        public Vector3Parameter center = new Vector3Parameter(Vector3.zero, true);
        // 扫描半径
        [Tooltip("全息扫描半径")]
        public FloatParameter radius = new FloatParameter(1, true);
        // 扫描线宽度
        [Tooltip("扫描线宽度，该值应当小于radius")]
        public FloatParameter width = new FloatParameter(0.5f, true);
        // 扫描中心区域透明度
        [Tooltip("扫描中心区域透明度，该值应当小于width")]
        public FloatParameter centerAlpha = new FloatParameter(0f, true);
        // 扫描颜色
        [Tooltip("扫描颜色")]
        public ColorParameter color = new ColorParameter(Color.green, true);

        public bool IsActive()
        {
            return enabled.value && radius.value>0;
        }

        public bool IsTileCompatible() => false;

    }

}

