using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyHurt : MonoBehaviour
{
    /// <summary>
    /// 受击特效时长
    /// </summary>
    [SerializeField]
    private float _HurtDuration = 0.2f;

    /// <summary>
    /// 受击特效颜色
    /// </summary>
    [SerializeField]
    private Color _HurtColor = Color.red;

    /// <summary>
    /// 受击特效影响到的渲染器
    /// </summary>
    [SerializeField]
    private List<Renderer> _AllRenderers;

    /// <summary>
    /// 受击特效粒子
    /// </summary>
    [SerializeField]
    private GameObject _BloodEffect;
    

    // shader property id
    private readonly int k_ShaderProperties_HurtColor = Shader.PropertyToID("_HurtColor");
    private readonly int k_ShaderProperties_HurtParameter = Shader.PropertyToID("_HurtParameter");
    // mpb
    private static MaterialPropertyBlock _MaterialPropertyBlock;
    private static Vector4 _TempVector4 = Vector4.zero;

    private void Awake()
    {
        // TODO： 测试代码
        InvokeRepeating("showHurtEffect", -1, 3);
        
        _BloodEffect.SetActive(false);
    }

    void showHurtEffect()
    {
        if (_MaterialPropertyBlock == null)
        {
            _MaterialPropertyBlock = new MaterialPropertyBlock();
        }
        
        _MaterialPropertyBlock.SetColor(k_ShaderProperties_HurtColor, _HurtColor);
        _TempVector4.x = Time.time;
        _TempVector4.y = 1 / Math.Max(_HurtDuration, float.MinValue);
        _MaterialPropertyBlock.SetVector(k_ShaderProperties_HurtParameter, _TempVector4);

        for (int i = 0; i < this._AllRenderers.Count; ++i)
        {
            Renderer renderer = this._AllRenderers[i];
            if (renderer)
            {
                renderer.SetPropertyBlock(_MaterialPropertyBlock);
            }
        }
        
        _BloodEffect.SetActive(false);
        _BloodEffect.SetActive(true);
    }
    
}
