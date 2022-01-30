using System;

namespace UnityEngine.Rendering.Universal.Internal
{
    /// <summary>
    /// Copy the given color buffer to the given destination color buffer.
    ///
    /// You can use this pass to copy a color buffer to the destination,
    /// so you can use it later in rendering. For example, you can copy
    /// the opaque texture to use it for distortion effects.
    /// </summary>
    public class CopyCameraColorPass : ScriptableRenderPass
    {
        const string k_SampleName = "CopyCameraColor";
        static readonly int k_ShaderPropertyID_CameraColor = Shader.PropertyToID(Parameters.k_CameraColorName);

        int m_SampleOffsetShaderHandle;
        Material m_SamplingMaterial;
        Downsampling m_DownsamplingMethod;
        Material m_CopyColorMaterial;

        private RenderTargetIdentifier source { get; set; }
        private RenderTargetHandle destination { get; set; }

        /// <summary>
        /// Create the CopyColorPass
        /// </summary>
        public CopyCameraColorPass(RenderPassEvent evt)
        {
            base.profilingSampler = new ProfilingSampler(nameof(CopyColorPass));

            m_SamplingMaterial = CoreUtils.CreateEngineMaterial("Hidden/Universal Render Pipeline/Sampling");
            m_CopyColorMaterial = CoreUtils.CreateEngineMaterial("Hidden/Universal Render Pipeline/Blit");
            m_SampleOffsetShaderHandle = Shader.PropertyToID("_SampleOffset");
            renderPassEvent = evt;
            m_DownsamplingMethod = Downsampling.None;
        }

        /// <summary>
        /// Configure the pass with the source and destination to execute on.
        /// </summary>
        /// <param name="source">Source Render Target</param>
        /// <param name="destination">Destination Render Target</param>
        public void Setup(RenderTargetIdentifier source, RenderTargetHandle destination, Downsampling downsampling)
        {
            this.source = source;
            this.destination = destination;
            m_DownsamplingMethod = downsampling;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;
            descriptor.msaaSamples = 1;
            descriptor.depthBufferBits = 0;
            if (m_DownsamplingMethod == Downsampling._2xBilinear)
            {
                descriptor.width /= 2;
                descriptor.height /= 2;
            }
            else if (m_DownsamplingMethod == Downsampling._4xBox || m_DownsamplingMethod == Downsampling._4xBilinear)
            {
                descriptor.width /= 4;
                descriptor.height /= 4;
            }

            cmd.GetTemporaryRT(destination.id, descriptor, m_DownsamplingMethod == Downsampling.None ? FilterMode.Point : FilterMode.Bilinear);
        }

        /// <inheritdoc/>
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (m_SamplingMaterial == null)
            {
                Debug.LogErrorFormat("Missing {0}. {1} render pass will not execute. Check for missing reference in the renderer resources.", m_SamplingMaterial, GetType().Name);
                return;
            }

            if (m_CopyColorMaterial == null)
            {
                Debug.LogErrorFormat("Missing {0}. {1} render pass will not execute. Check for missing reference in the renderer resources.", m_CopyColorMaterial, GetType().Name);
                return;
            }

            CommandBuffer cmd = CommandBufferPool.Get();
            cmd.BeginSample(k_SampleName);
            {
                RenderTargetIdentifier opaqueColorRT = destination.Identifier();

                switch (m_DownsamplingMethod)
                {
                    case Downsampling.None:
                        Blit(cmd, source, opaqueColorRT, m_CopyColorMaterial, 0);
                        break;
                    case Downsampling._2xBilinear:
                        Blit(cmd, source, opaqueColorRT, m_CopyColorMaterial, 0);
                        break;
                    case Downsampling._4xBox:
                        m_SamplingMaterial.SetFloat(m_SampleOffsetShaderHandle, 2);
                        Blit(cmd, source, opaqueColorRT, m_SamplingMaterial, 0);
                        break;
                    case Downsampling._4xBilinear:
                        Blit(cmd, source, opaqueColorRT, m_CopyColorMaterial, 0);
                        break;
                }
            }
            cmd.EndSample(k_SampleName);

            // 设置全局Camera Color Texture
            cmd.SetGlobalTexture(k_ShaderPropertyID_CameraColor, destination.Identifier());
            
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        /// <inheritdoc/>
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            if (cmd == null)
                throw new ArgumentNullException("cmd");

            if (destination != RenderTargetHandle.CameraTarget)
            {
                cmd.ReleaseTemporaryRT(destination.id);
                destination = RenderTargetHandle.CameraTarget;
            }
        }
    }
}
