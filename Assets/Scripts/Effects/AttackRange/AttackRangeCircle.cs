using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRangeCircle : MonoBehaviour
{
    // 攻击范围中心点位置(_ScannerCenter: 世界空间)
    private const string k_AttackRangeCenter = "_AttackRangeCenter";
    
    // 攻击范围参数(_AttackRangeCenter x:攻击范围范围(半径); y:攻击范围线宽度 z:攻击范围中心区域透明度)
    private const string k_AttackRangeParams = "_AttackRangeParams";
    // x: 攻击范围范围(半径)
    [SerializeField]
    private float _Radius = 1f;
    public float Radius => _Radius;
    // y: 攻击范围线宽度
    [SerializeField]
    private float _Width = 0.1f;
    public float Width => _Width;
    // z: 攻击范围中心区域透明度
    [SerializeField]
    private float _CenterAlpha = 0.01f;
    public float CenterAlpha => _CenterAlpha;

    // 攻击范围颜色
    private const string k_AttackRangeColor = "_AttackRangeColor";
    [SerializeField] 
    private Color _Color = Color.red;
    public Color Color => _Color;
    
    private MeshRenderer _MeshRenderer;
    private MaterialPropertyBlock _MaterialPropertyBlock;
    private Vector4 _TmpVector = new Vector4();
    private float _Scale = 0;
    
    void Awake()
    {
        _MeshRenderer = GetComponent<MeshRenderer>();
        _MaterialPropertyBlock = new MaterialPropertyBlock();
    }
    
    void Update()
    {
        updatePlaneScale();
        updateMaterialProperty();
    }

    /// <summary>
    /// 更新材质属性
    /// </summary>
    private void updateMaterialProperty()
    {
        // 攻击范围中心点位置(_ScannerCenter: 世界空间)
        _MaterialPropertyBlock.SetVector(k_AttackRangeCenter, transform.position);
        // 攻击范围参数(_AttackRangeCenter x:攻击范围范围(半径); y:攻击范围线宽度 z:攻击范围中心区域透明度)
        _TmpVector.x = _Radius;
        _TmpVector.y = _Width;
        _TmpVector.z = _CenterAlpha;
        _MaterialPropertyBlock.SetVector(k_AttackRangeParams, _TmpVector);
        // 攻击范围颜色
        _MaterialPropertyBlock.SetColor(k_AttackRangeColor, _Color);
        
        _MeshRenderer.SetPropertyBlock(_MaterialPropertyBlock);
    }

    /// <summary>
    /// 根据半径计算材质大小
    /// </summary>
    void updatePlaneScale()
    {
        float planeScale = _Radius * 1f;    // 半径转换到plane缩放
        if (_Scale != planeScale)
        {
            _Scale = planeScale;
            _TmpVector.x = planeScale;
            _TmpVector.y = planeScale;
            _TmpVector.z = planeScale;
            transform.localScale = _TmpVector;
        }
    }
    
}
