using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parabola : MonoBehaviour
{
    private LineRenderer _LineRenderer;

    // 物理预测信息
    // 物理碰撞LayerMask
    [SerializeField]
    private LayerMask _LayerMask;
    [SerializeField] 
    private float _MaxHeight = 3;
    // 每次预测的步长 (几次FixedUpdate)
    [SerializeField] 
    private int _PredictStep = 2;
    // 初始预测数量
    [SerializeField]
    private int _PredictCount = 5;
    // 后续每次预测数量 (直到达到最大值 或者 碰撞到物体)
    [SerializeField]
    private int _AddPredictCount = 5;
    // 最大预测数量
    [SerializeField]
    private int _MaxPredictCount = 200;
    // 物理预测数据记录
    private List<Vector3[]> _PredictDatas = new List<Vector3[]>();
    
    RaycastHit _HitInfo = new RaycastHit();

    // 绘制采样点
    private List<Vector3> _Points = new List<Vector3>();
    
    void Awake()
    {
        this._LineRenderer = this.GetComponent<LineRenderer>();
    }
    
    // Update is called once per frame
    public void PredictParabola(Rigidbody rigidbody, Vector3 startPoint, Vector3 endPoint)
    {
        // 计算刚体现有速度
        Vector3 velocity = !rigidbody.isKinematic ? rigidbody.velocity : Vector3.zero;
        // 计算刚体重力
        Vector3 gravity = (rigidbody.useGravity && !rigidbody.isKinematic) ? Physics.gravity : Vector3.zero;
        // 刚体质量
        float mass = rigidbody.mass;
        // 刚体阻力
        float drag = rigidbody.drag;
        // 碰撞检测两点的index
        int colStartIndex = 0; 
        int colEndIdx = 0;
        // 
        Vector3 point1, point2;
        Vector3 vector;
        
        Debug.Log(velocity);
        
        // 获取完刚体现有速度信息，才能给刚体加力
        Vector3 force = RigidbodyUtils.CalculateParabolaForce(rigidbody.mass, startPoint, endPoint, _MaxHeight);

        // 开始预测抛物线
        _PredictDatas.Clear();
        RigidbodyUtils.CalculateMovements(_PredictDatas, startPoint, velocity, gravity, _PredictCount, _PredictStep, force, mass, drag);

        // 持续预测直到发生碰撞或达到预测最大数量
        while (_PredictDatas.Count<_MaxPredictCount)
        {
            // 找出最新预测的起始点和结束点，做碰撞检测
            colStartIndex = colEndIdx;
            colEndIdx = _PredictDatas.Count-1;
            point1 = _PredictDatas[colStartIndex][0];   // start
            point2 = _PredictDatas[colEndIdx][0];   // end
            vector = point2 - point1;

            // 预测抛物线碰撞到物体
            if (Physics.Raycast(point1, vector.normalized, out _HitInfo, vector.magnitude, _LayerMask))
            {
                break;
            }
            
            // 没有碰撞到物体，继续预测
            Vector3[] lastPredictData = _PredictDatas[_PredictDatas.Count - 1]; // 0: position 1: velocity
            RigidbodyUtils.CalculateMovements(_PredictDatas, lastPredictData[0], lastPredictData[1], gravity, _AddPredictCount, _PredictStep, Vector3.zero, mass, drag);
        }

        // 划线数据
        _Points.Clear();
        _Points.Add(startPoint);
        for (int i = 0; i < _PredictDatas.Count; ++i)
        {
            _Points.Add(_PredictDatas[i][0]);
        }
    }

    public void AddRigidbodyForce(Rigidbody rigidbody, Vector3 startPoint, Vector3 endPoint)
    {
        // 获取完刚体现有速度信息，才能给刚体加力
        Vector3 force = RigidbodyUtils.CalculateParabolaForce(rigidbody.mass, startPoint, endPoint, _MaxHeight);
        rigidbody.AddForce(force);
    }

    void Update()
    {
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

    public void SetPoints(Vector3[] points)
    {
        _Points.Clear();
        _Points.AddRange(points);
    }
    
}
