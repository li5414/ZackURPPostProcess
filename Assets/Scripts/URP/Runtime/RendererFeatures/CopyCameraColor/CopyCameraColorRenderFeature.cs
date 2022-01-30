using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.Universal.Internal;

namespace Zack.UniversalRP.PostProcessing
{
    public class CopyCameraColorRenderFeature : ScriptableRendererFeature
    {
        public RenderPassEvent evt = RenderPassEvent.AfterRenderingTransparents;
        // Pass
        CopyCameraColorPass m_ScriptablePass;

        public override void Create()
        {
            m_ScriptablePass = new CopyCameraColorPass(evt);
            Parameters.CameraColor.Init(Parameters.k_CameraColorName);
        }

        public override void AddRenderPasses(UnityEngine.Rendering.Universal.ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            Downsampling downsamplingMethod = UniversalRenderPipeline.asset.opaqueDownsampling;
            m_ScriptablePass.Setup(renderer.cameraColorTarget, Parameters.CameraColor, downsamplingMethod);
            renderer.EnqueuePass(m_ScriptablePass);
        }

    }
}



