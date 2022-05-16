using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulateParabola : MonoBehaviour
{
    private LineRenderer _LineRenderer;

    // 采样频率
    [SerializeField]
    private int _PointCountPerSecond = 20;
    // 曲线上的点
    private List<Vector3> _Points = new List<Vector3>();
    // 发生回弹LayerMask
    [SerializeField] 
    private LayerMask _BounceLayerMask;
    // 地面LayerMask
    [SerializeField] 
    private LayerMask _GroundLayerMask;

    // 起始点 
    private Vector3 _StartPoint = Vector3.zero;
    // 结束点
    private Vector3 _EndPoint = Vector3.zero;
    // 最大高度
    private float _MaxHeight = 5f;
    // 采样时长
    private float _Duration = 1f;
    // 脏标记
    private bool _Dirty = true;

    // 采样点
    public List<Vector3> Points => this._Points;
    // 每次采样间隔
    public float Interval => 1f / (float)(this._PointCountPerSecond - 1);


    void Awake()
    {
        this._LineRenderer = this.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!this._LineRenderer.enabled)
        {
            return;
        }
        
        if (this._Dirty)
        {
            this._Dirty = false;
            CalculateBezierPoints();
        }
        
        if (_Points.Count > 0)
        {
            _LineRenderer.SetVertexCount(_Points.Count);
            _LineRenderer.SetPositions(_Points.ToArray());
        }
        else
        {
            _LineRenderer.SetVertexCount(0);
        }
    }

    public void ShowParabola()
    {
        this._LineRenderer.enabled = true;
        this._Dirty = true;
    }

    public void HideParabola()
    {
        this._LineRenderer.enabled = false;
        this._Points.Clear();
        this._LineRenderer.SetVertexCount(0);
    }
    
    public void SetPointCountPerSecond(int cnt)
    {
        this._PointCountPerSecond = cnt;
        this._Dirty = true;
    }
    public void SetStartPoint(Vector3 startPoint)
    {
        this._StartPoint = startPoint;
        this._Dirty = true;
    }
    public void SetEndPoint(Vector3 endPoint)
    {
        this._EndPoint = endPoint;
        this._Dirty = true;
    }
    public void SetMaxHeight(float height)
    {
        this._MaxHeight = height;
        this._Dirty = true;
    }
    public void SetDuration(float duration)
    {
        this._Duration = duration;
        this._Dirty = true;
    }

    RaycastHit _HitInfo = new RaycastHit();
    void CalculateBezierPoints()
    {
        _Points.Clear();

        // 每次采样间隔
        float interval = Interval;
        // 当前抛物线初始速度
        Vector3 initialVelocity = SimulateParabolaUtils.CalculateParabolaInitialVelocity(this._StartPoint, this._EndPoint, this._MaxHeight);
        // 当前抛物线时间
        float time = 0;
        // 总耗时
        float totalTime = 0;
        // 抛物线起点
        Vector3 startPoint = this._StartPoint;
        // 总采样次数
        int sampleCount = (int)this._Duration * this._PointCountPerSecond;
        
        this._Points.Add(startPoint);
        time += interval;
        totalTime += interval;
        
        for (int i = 1; i < sampleCount; ++i)
        {
            Vector3 point = SimulateParabolaUtils.SampleParabolaPoint(startPoint, initialVelocity, time);
            
            // 碰撞检测
            Vector3 origin = this._Points[i - 1];
            Vector3 vector = point - origin;
            if (Physics.Raycast(origin, vector.normalized, out _HitInfo, vector.magnitude, _BounceLayerMask))
            {
                // 碰撞到墙体，反弹
                float percent = Math.Min(Vector3.Distance(_HitInfo.point, origin) / vector.magnitude, 1);
                float t = interval * percent;
                time += t;
                totalTime += t;
            
                // 反弹
                startPoint = _HitInfo.point;
                this._Points.Add(startPoint);
                initialVelocity = SimulateParabolaUtils.CalculateBounceVelocity(initialVelocity, time, _HitInfo.normal);
                time = 0;
            }
            else if (Physics.Raycast(origin, vector.normalized, out _HitInfo, vector.magnitude, _GroundLayerMask))
            {
                // 碰撞到地面，结束
                this._Points.Add(_HitInfo.point);
                break;
            }
            else
            {
                // 无碰撞
                this._Points.Add(point);

                time += interval;
                totalTime += interval;
            }
        }
        
    }
    
}
