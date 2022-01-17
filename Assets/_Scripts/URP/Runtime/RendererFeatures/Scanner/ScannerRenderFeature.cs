using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Zack.UniversalRP.PostProcessing
{
    public class ScannerRenderFeature : ScriptableRendererFeature
    {
        public RenderPassEvent evt = RenderPassEvent.AfterRenderingTransparents;
        // Pass
        ScannerPass m_ScriptablePass;

        public override void Create()
        {
            m_ScriptablePass = new ScannerPass(evt);
        }

        public override void AddRenderPasses(UnityEngine.Rendering.Universal.ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            var dest = RenderTargetHandle.CameraTarget;
            //m_ScriptablePass.Setup(renderer.cameraColorTarget, renderer.cameraDepthTarget, dest);
            m_ScriptablePass.Setup(renderer.cameraColorTarget, dest);
            renderer.EnqueuePass(m_ScriptablePass);
        }

    }
}



