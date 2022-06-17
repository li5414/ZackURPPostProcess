using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellIndicator : MonoBehaviour
{
    // 技能指向类型
    public enum SpellIndicatorType
    {
        // 无
        None = 0,
        // 箭头
        Line,
        // 带缩放的箭头
        ScaleLine,
        // 扇区
        Cone,
        // 圆形
        Circle,
    }
    
    // 当前显示类型
    private SpellIndicatorType _CurrentType = SpellIndicatorType.None;
    
    // 箭头显示
    [SerializeField]
    private GameObject _LineSplat;
    // 默认大小
    [SerializeField]
    private Vector3 _LineSplatDefaultScale = Vector3.one;
    // 扇区显示
    [SerializeField]
    private GameObject _ConeSplat;
    // 默认大小
    [SerializeField]
    private Vector3 _ConeSplatDefaultScale = Vector3.one;
    // 圆形显示
    [SerializeField]
    private GameObject _CircleSplat;
    // 范围显示
    [SerializeField]
    private GameObject _RangeSplat;

    private Vector3 _TempVec = Vector3.zero;

    void Awake()
    {
        HideAllSpellIndicator();
    }
    
    /// <summary>
    /// 显示技能指向
    /// </summary>
    /// <param name="type">技能指向类型</param>
    /// <param name="direction">世界方向</param>
    /// <param name="distance">距离</param>
    /// <param name="radius">圆形半径</param>
    /// <param name="range">施法范围</param>
    public void ShowSpellIndicator(SpellIndicatorType type, Vector2 direction, float distance = 0f, float radius = 0f, float range = 0f)
    {
        HideSpellIndicator();

        // 标准化世界方向
        direction = direction.normalized;
        
        switch (type)
        {
        case SpellIndicatorType.Line:
            {
                // 箭头
                showLineSplat(direction);
            }
            break;
        case SpellIndicatorType.ScaleLine:
            {
                // 带缩放的箭头
                showScaleLineSplat(direction, distance);
            }
            break;
        case SpellIndicatorType.Cone:
            {
                // 扇区
                showConeSplat(direction);
            }
            break;
        case SpellIndicatorType.Circle:
            {
                // 圆形
                showCircleSplat(direction, distance, radius, range);
            }
            break;
        }
    }

    /// <summary>
    /// 显示箭头
    /// </summary>
    /// <param name="direction">世界方向</param>
    void showLineSplat(Vector2 direction)
    {
        this._CurrentType = SpellIndicatorType.Line;
        this._LineSplat.SetActive(true);

        Transform splatTransform = this._LineSplat.transform;
        splatTransform.localScale = this._LineSplatDefaultScale;
        _TempVec.x = direction.x;
        _TempVec.y = 0;
        _TempVec.z = direction.y;
        splatTransform.forward = _TempVec;
    }
    /// <summary>
    /// 显示缩放箭头
    /// </summary>
    /// <param name="direction">世界方向</param>
    /// <param name="distance">距离</param>
    void showScaleLineSplat(Vector2 direction, float distance)
    {
        this._CurrentType = SpellIndicatorType.ScaleLine;
        this._LineSplat.SetActive(true);

        Transform splatTransform = this._LineSplat.transform;
        _TempVec.x = _LineSplatDefaultScale.x;
        _TempVec.y = _LineSplatDefaultScale.y;
        _TempVec.z = distance;
        splatTransform.localScale = _TempVec;
        _TempVec.x = direction.x;
        _TempVec.y = 0;
        _TempVec.z = direction.y;
        splatTransform.forward = _TempVec;
    }
    /// <summary>
    /// 显示扇区
    /// </summary>
    /// <param name="direction">世界方向</param>
    void showConeSplat(Vector2 direction)
    {
        this._CurrentType = SpellIndicatorType.Cone;
        this._ConeSplat.SetActive(true);

        Transform splatTransform = this._ConeSplat.transform;
        splatTransform.localScale = this._ConeSplatDefaultScale;
        _TempVec.x = direction.x;
        _TempVec.y = 0;
        _TempVec.z = direction.y;
        splatTransform.forward = _TempVec;
    }
    /// <summary>
    /// 显示圆形
    /// </summary>
    /// <param name="direction">世界方向</param>
    /// <param name="distance">距离</param>
    /// <param name="radius">圆形半径</param>
    /// <param name="range">施法范围</param>
    void showCircleSplat(Vector2 direction, float distance, float radius, float range)
    {
        this._CurrentType = SpellIndicatorType.Circle;
        this._CircleSplat.SetActive(true);
        this._RangeSplat.SetActive(true);

        if (distance > range)
        {
            distance = range;
        }

        Transform splatTransform = this._CircleSplat.transform;
        _TempVec.x = radius*2;
        _TempVec.y = 1;
        _TempVec.z = radius*2;
        splatTransform.localScale = _TempVec;
        _TempVec.x = direction.x * distance;
        _TempVec.y = 0;
        _TempVec.z = direction.y * distance;
        splatTransform.position = transform.position + _TempVec;
        // 范围
        splatTransform = this._RangeSplat.transform;
        _TempVec.x = range*2;
        _TempVec.y = 1;
        _TempVec.z = range*2;
        splatTransform.localScale = _TempVec;
    }

    /// <summary>
    /// 隐藏技能指向
    /// </summary>
    public void HideSpellIndicator()
    {
        switch (this._CurrentType)
        {
        case SpellIndicatorType.Line:
        case SpellIndicatorType.ScaleLine:
            {
                this._LineSplat.SetActive(false);
            }
            break;
        case SpellIndicatorType.Cone:
            {
                this._ConeSplat.SetActive(false);
            }
            break;
        case SpellIndicatorType.Circle:
            {
                this._CircleSplat.SetActive(false);
                this._RangeSplat.SetActive(false);
            }
            break;
        }
        
        this._CurrentType = SpellIndicatorType.None;
    }

    void HideAllSpellIndicator()
    {
        this._CurrentType = SpellIndicatorType.None;
        
        this._LineSplat.SetActive(false);
        this._ConeSplat.SetActive(false);
        this._CircleSplat.SetActive(false);
        this._RangeSplat.SetActive(false);
    }
    
}
