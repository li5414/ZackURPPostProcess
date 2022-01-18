using System;
using UnityEngine;

namespace UnityEngine.Rendering.Universal
{
    [System.Serializable, VolumeComponentMenu("ZackPostProcess/Scanner")]
    public class Scanner : VolumeComponent, IPostProcessComponent
    {
        // ɨ������
        public enum ScannerType
        {
            // ����
            Sphere,
            // Բ����
            Cylinder,
            // ������
            Cube
        }
        [Serializable]
        public sealed class ScannerTypeParameter : VolumeParameter<ScannerType> { public ScannerTypeParameter(Scanner.ScannerType value, bool overrideState = false) : base(value, overrideState) { } }

        // �������
        [Tooltip("�������")]
        public BoolParameter enabled = new BoolParameter(true, true);
        // ɨ������
        [Tooltip("ɨ������")]
        public ScannerTypeParameter type = new ScannerTypeParameter(ScannerType.Sphere, true);
        // ����λ��
        [Tooltip("ȫϢɨ������λ��")]
        public Vector3Parameter center = new Vector3Parameter(Vector3.zero, true);
        //
        public ClampedFloatParameter weight = new ClampedFloatParameter(1, 0, 1, true);
        // ɨ��뾶
        [Tooltip("ȫϢɨ��뾶")]
        public FloatParameter radius = new FloatParameter(0f, true);
        // ɨ���߿��
        [Tooltip("ɨ���߿�ȣ���ֵӦ��С��radius")]
        public FloatParameter width = new FloatParameter(0.5f, true);
        // ɨ����������͸����
        [Tooltip("ɨ����������͸���ȣ���ֵӦ��С��width")]
        public FloatParameter centerAlpha = new FloatParameter(0f, true);
        // ɨ����ɫ
        [Tooltip("ɨ����ɫ")]
        public ColorParameter color = new ColorParameter(Color.green, true);
        //
        public TextureParameter texture = new TextureParameter(null, true);
        //
        public FloatParameter textureScale = new FloatParameter(1, true);


        public bool IsActive()
        {
            return enabled.value && radius.value>0;
        }

        public bool IsTileCompatible() => false;

    }

}

