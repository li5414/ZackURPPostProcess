using System.Diagnostics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ScannerPass : ScriptableRenderPass
{

    // Volume
    Scanner m_Scanner;

    // 材质相关
    const string k_ShaderName = "Zack_URP_Post-Process/Scanner";
    Material m_Material;
    static readonly int k_ShaderPropertyID_ScannerCenter = Shader.PropertyToID("_ScannerCenter");
    static readonly int k_ShaderPropertyID_ScannerParams = Shader.PropertyToID("_ScannerParams");
    static readonly int k_ShaderPropertyID_ScannerColor = Shader.PropertyToID("_ScannerColor");
    const string k_ShaderKeyword_ScannerTypeCylinder = "_SCANNER_TYPE_CYLINDER";
    const string k_ShaderKeyword_ScannerTypeCube = "_SCANNER_TYPE_CUBE";

    // 操作相关
    const string k_RenderTag = "Scanner Effects";
    const string k_SampleName = "Scanner";
    const string k_TempRTName = "TemporaryRenderTexture01";
    RenderTargetIdentifier m_Source;
    RenderTargetHandle m_Destination { get; set; }
    RenderTargetHandle m_temporaryColorTexture;

    public ScannerPass(RenderPassEvent evt)
    {
		renderPassEvent = evt;
		
        m_Material = CoreUtils.CreateEngineMaterial(Shader.Find(k_ShaderName));
        m_temporaryColorTexture.Init(k_TempRTName);
    }

    public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
    {

    }

    public void Setup(in RenderTargetIdentifier source, RenderTargetHandle destination)
    {
        this.m_Source = source;
        this.m_Destination = destination;
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (m_Material == null)
        {
            UnityEngine.Debug.LogError("Cannot find the post-process material !");
            return;
        }
        if (!renderingData.cameraData.postProcessEnabled) return;
        //通过队列来找到Scanner组件
        var stack = VolumeManager.instance.stack;
        m_Scanner = stack.GetComponent<Scanner>();
        if (m_Scanner == null) { return; }
        if (!m_Scanner.IsActive()) return;

        var cmd = CommandBufferPool.Get(k_RenderTag);
        //UnityEngine.Debug.Log("Execute渲染中！");
        Render(cmd, ref renderingData);
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    void Render(CommandBuffer cmd, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.isSceneViewCamera) return;
        var source = m_Source;


        m_Material.SetVector(k_ShaderPropertyID_ScannerCenter, new Vector4(m_Scanner.center.value.x, m_Scanner.center.value.y, m_Scanner.center.value.z, 0));
        m_Material.SetVector(k_ShaderPropertyID_ScannerParams, new Vector4(m_Scanner.radius.value, m_Scanner.width.value, m_Scanner.centerAlpha.value, 0));
        m_Material.SetColor(k_ShaderPropertyID_ScannerColor, m_Scanner.color.value);
        switch(m_Scanner.type.value)
        {
            case Scanner.ScannerType.Sphere:
                m_Material.DisableKeyword(k_ShaderKeyword_ScannerTypeCylinder);
                m_Material.DisableKeyword(k_ShaderKeyword_ScannerTypeCube);
                break;

            case Scanner.ScannerType.Cylinder:
                m_Material.EnableKeyword(k_ShaderKeyword_ScannerTypeCylinder);
                m_Material.DisableKeyword(k_ShaderKeyword_ScannerTypeCube);
                break;

            case Scanner.ScannerType.Cube:
                m_Material.EnableKeyword(k_ShaderKeyword_ScannerTypeCube);
                m_Material.DisableKeyword(k_ShaderKeyword_ScannerTypeCylinder);
                break;
        }

        cmd.BeginSample(k_SampleName);
        //不能读写同一个颜色target，创建一个临时的render Target去blit
        if (m_Destination == RenderTargetHandle.CameraTarget)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            cmd.GetTemporaryRT(m_temporaryColorTexture.id, opaqueDesc, FilterMode.Bilinear);
            Blit(cmd, source, m_temporaryColorTexture.Identifier());
            Blit(cmd, m_temporaryColorTexture.Identifier(), source, m_Material);
            cmd.ReleaseTemporaryRT(m_temporaryColorTexture.id);
        }
        else
        {
            Blit(cmd, source, m_Destination.Identifier(), m_Material);
        }
        cmd.EndSample(k_SampleName);
    }

    public override void FrameCleanup(CommandBuffer cmd)
    {
    }

}
