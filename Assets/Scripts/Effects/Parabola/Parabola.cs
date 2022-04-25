using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parabola : MonoBehaviour
{
    private LineRenderer _LineRenderer;

    // 原始点 (3个点绘制一段曲线: start, highest, end)
    private List<Vector3> _Points = new List<Vector3>();

    // Bezier曲线上的点
    [SerializeField]
    private int _BezierPointCount = 10;
    private List<Vector3> _BezierPoints = new List<Vector3>();

    // 发生回弹LayerMask
    [SerializeField] 
    private LayerMask _ReflectLayerMask;
    
    void Awake()
    {
        this._LineRenderer = this.GetComponent<LineRenderer>();

        CalculateBezierPoints();
    }

    // Update is called once per frame
    void Update()
    {
        if (_BezierPoints.Count > 0)
        {
            _LineRenderer.SetVertexCount(_BezierPoints.Count);
            _LineRenderer.SetPositions(_BezierPoints.ToArray());
        }
        else
        {
            _LineRenderer.SetVertexCount(0);
        }
    }

    public void SetPoints(Vector3[] points)
    {
        _Points.Clear();
        _Points.AddRange(points);
        // 重新计算        
        CalculateBezierPoints();
    }

    public void SetBezierPointCount(int count)
    {
        _BezierPointCount = count;
        // 重新计算        
        CalculateBezierPoints();
    }
    
    public Vector3[] GetBezierPoints()
    {
        return this._BezierPoints.ToArray();
    }

    RaycastHit _HitInfo = new RaycastHit();
    void CalculateBezierPoints()
    {
        _BezierPoints.Clear();
        int bezierCount = _Points.Count / 3;
        for (int i = 0; i < bezierCount; ++i)
        {
            Vector3 pos1 = _Points[i * 3];
            Vector3 pos2 = _Points[i * 3 + 1];
            Vector3 pos3 = _Points[i * 3 + 2];
            for (float rIdx = 0; rIdx <= _BezierPointCount; ++rIdx)
            {
                float ratio = rIdx / _BezierPointCount;
                var tangentLineVertex1 = Vector3.Lerp(pos1, pos2, ratio);
                var tangentLineVertex2 = Vector3.Lerp(pos2, pos3, ratio);
                var bezierpoint = Vector3.Lerp(tangentLineVertex1, tangentLineVertex2, ratio);
                _BezierPoints.Add(bezierpoint);
            }
            // Vector3[] bezierPoints = BezierUtils.GetThreePowerBeizerList(pos1, pos2, pos3, pos4, _BezierPointCount);
            // _BezierPoints.AddRange(bezierPoints);
        }
        // 判断线段是否发生碰撞
        for (int i = 0; i < _BezierPoints.Count - 1; ++i)
        {
            Vector3 origin = _BezierPoints[i];
            Vector3 direction = _BezierPoints[i + 1] - origin;
            if (Physics.Raycast(origin, direction.normalized, out _HitInfo, direction.magnitude, _ReflectLayerMask))
            {
                ParabolaUtils.GetReflectSymmetryPoints(origin, _HitInfo.point, _HitInfo.normal, _BezierPoints, i, _BezierPoints.Count);
                _BezierPoints.Insert(i, _HitInfo.point);
                break;
            }

        }
    }
    
    
}
