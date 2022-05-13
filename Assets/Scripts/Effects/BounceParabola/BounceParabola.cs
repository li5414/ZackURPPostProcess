using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceParabola : MonoBehaviour
{
    private LineRenderer _LineRenderer;

    // 物理预测信息
    // 物理碰撞LayerMask
    [SerializeField]
    private LayerMask _LayerMask;
    // 高度
    private float _MaxHeight = 3;
    // 起始点
    private Vector3 _StartPoint = Vector3.zero;
    // 终点
    private Vector3 _EndPoint = Vector3.zero;
    // 刚体 (仅用于预测)
    private Rigidbody _Rigidbody;
    // 预测时长
    private float _PredictDuration = 2f;
    
    // 脏标记
    private bool _Dirty = false;

    // 物理预测数据
    private PredictionTimeline _PredictionDatas;
    // 绘制采样点
    private List<Vector3> _Points = new List<Vector3>();
    
    void Awake()
    {
        this._LineRenderer = this.GetComponent<LineRenderer>();
        HideParabola();
    }

    /// <summary>
    /// 设置投掷最大高度
    /// </summary>
    /// <param name="height"></param>
    public void SetMaxHeight(float height)
    {
        this._MaxHeight = height;
        this._Dirty = true;
    }
    
    /// <summary>
    /// 设置起始点
    /// </summary>
    /// <param name="point"></param>
    public void SetStartPoint(Vector3 point)
    {
        this._StartPoint = point;
        this._Dirty = true;
    }

    /// <summary>
    /// 设置终点
    /// </summary>
    /// <param name="point"></param>
    public void SetEndPoint(Vector3 point)
    {
        this._EndPoint = point;
        this._Dirty = true;
    }

    /// <summary>
    /// 设置刚体
    /// </summary>
    /// <param name="rigidbody"></param>
    public void SetRigidbody(Rigidbody rigidbody)
    {
        this._Rigidbody = rigidbody;
        this._Dirty = true;
    }

    /// <summary>
    /// 设置预测时长
    /// </summary>
    /// <param name="duration"></param>
    public void SetPredictDuration(float duration)
    {
        this._PredictDuration = duration;
        this._Dirty = true;
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

    public void AddRigidbodyForce(Rigidbody rigidbody)
    {
        // 获取完刚体现有速度信息，才能给刚体加力
        Vector3 force = RigidbodyUtils.CalculateParabolaForce(rigidbody.mass, _StartPoint, _EndPoint, _MaxHeight);
        rigidbody.AddForce(force);
    }

    void Update()
    {
        if (!this._LineRenderer.enabled)
        {
            return;
        }
        
        if (this._Dirty)
        {
            this._Dirty = false;
            
            refreshPrediction();
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

    void refreshPrediction()
    {
        this._PredictionDatas =  PredictionSystem.Record.Prefabs.Add(_Rigidbody.gameObject, launch);

        int iterations = (int)(this._PredictDuration / Time.fixedDeltaTime);
        PredictionSystem.Simulate(iterations);
        
        _Points.Clear();
        for (int i = 0; i < this._PredictionDatas.Count; ++i)
        {
            this._Points.Add(this._PredictionDatas[i].Position);
        }

        PredictionSystem.Record.Prefabs.Remove(this._PredictionDatas);
        this._PredictionDatas = null;
    }
    // 物理模拟发射
    void launch(Rigidbody rigidbody)
    {
        AddRigidbodyForce(rigidbody);
        
        rigidbody.transform.position = _StartPoint;
        // rigidbody.transform.rotation = transform.rotation;

    }
    
}
