using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.Rendering.Universal
{
    public class Parameters
    {
        /// <summary>
        /// 相机渲染纹理，包含透明物体(与CopyColorPass的结果m_OpaqueColor对标)
        /// </summary>
        public static RenderTargetHandle CameraColor
        {
            get { return m_CameraColor; }
        }
        static RenderTargetHandle m_CameraColor;

        public static readonly string k_CameraColorName = "_ZURPCameraColorTexture";

    }
}

