using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beam : MonoBehaviour
{
    // 模拟点
    class BeamPoint
    {
        // 位置
        public Vector3 position;
        // 方向
        public Vector3 direction;
        // 到起始点距离
        public float distance;
        
        public BeamPoint(Vector3 position, Vector3 direction, float distance)
        {
            this.position = position;
            this.direction = direction;
            this.distance = distance;
        }

        /// <summary>
        /// 更新当前光束点
        /// </summary>
        /// <param name="lastBeamPoint">前一个光束点</param>
        /// <param name="deltaDistance">deltaTime光束点移动距离</param>
        public void UpdateBeamPoint(BeamPoint lastBeamPoint, float deltaDistance)
        {
            float percent = Math.Min(deltaDistance / (distance - lastBeamPoint.distance), 1f);

            this.position = Vector3.Lerp(this.position, lastBeamPoint.position+lastBeamPoint.direction*(distance-lastBeamPoint.distance), percent);
            this.direction = Vector3.Lerp(this.direction, lastBeamPoint.direction, percent);
        }

        public void UpdateBeamPoint(Vector3 position, Vector3 direction)
        {
            this.position = position;
            this.direction = direction;
        }
    }
    
    // 光束gameObject
    [Header("Beam GameObjects")]
    [SerializeField]
    private GameObject _BeamStart;
    [SerializeField]
    private GameObject _Beam;
    [SerializeField]
    private GameObject _BeamEnd;
    
    // 光束细节参数
    [Header("Adjustable Variables")]
    public Vector3 _BeamEndOffset = Vector3.zero; //How far from the raycast hit point the end effect is positioned
    public float _TextureScrollSpeed = 8f; //How fast the texture scrolls along the beam
    public float _TextureLengthScale = 3; //Length of the beam texture
    public int _SimulatePointCount = 10;    // 模拟光束的点的个数
    public float _Speed = 1;    // 光束喷射速度
    
    // LineRenderer
    private LineRenderer _LineRenderer;
    // 模拟点
    private List<BeamPoint> _Points;
    // 模拟点间隔
    private float _PointInterval;
    
    // 可配参数
    // 最大距离
    private float _MaxDistance = 10;

    void Awake()
    {
        this._Points = new List<BeamPoint>(this._SimulatePointCount);
        
        this._LineRenderer = this._Beam.GetComponent<LineRenderer>();
    }

    public void SetBeamInfo(Transform parent)
    {
        // TODO:
        Vector3 bornOffset = Vector3.forward + Vector3.up;
        this._MaxDistance = 10;

        Vector3 start = parent.position + parent.transform.rotation * bornOffset;
        
        this.gameObject.transform.parent = parent;
        this.gameObject.transform.position = start;
        
        this._PointInterval = this._MaxDistance / (float)this._SimulatePointCount;
        this._Points.Clear();
        float distance;
        for (int i = 0; i < this._SimulatePointCount; ++i)
        {
            distance = this._PointInterval * i;
            this._Points.Add(new BeamPoint(start + transform.forward * distance, transform.forward, distance));
        }
    }

    public void UpdatePoints()
    {
        float deltaDistance = Time.deltaTime * this._Speed;
        int skipCount = (int)Math.Ceiling(deltaDistance / this._PointInterval);
        
        for (int i = this._SimulatePointCount - 1; i > 0; --i)
        {
            int lstPointIdx = i - skipCount;
            if (lstPointIdx < 0)
            {
                lstPointIdx = 0;
            }
            this._Points[i].UpdateBeamPoint(this._Points[lstPointIdx], deltaDistance);
        }

        this._Points[0].UpdateBeamPoint(transform.position, transform.forward);
    }

    void UpdateLineRenderer()
    {
        int showCount = this._SimulatePointCount - 1;
        
        Vector3 direction = this._Points[0].direction;
        Vector3 start = this._Points[0].position;
        RaycastHit hit;
        if (Physics.Raycast(start, direction, out hit, this._MaxDistance))
        {
            // end = hit.point - (dir.normalized * _BeamEndOffset);
            float distance = Vector3.Distance(start, hit.point);
            showCount = (int)Math.Min(Math.Ceiling(distance / this._PointInterval), this._SimulatePointCount-1);
        }
        Vector3 end = this._Points[showCount].position;

        
        // 提交光束顶点
        {
            this._LineRenderer.positionCount = showCount + 1;
            int i;
            for (i = 0; i <= showCount; ++i)
            {
                this._LineRenderer.SetPosition(i, this._Points[i].position);
            }
        
            this._LineRenderer.sharedMaterial.mainTextureScale = new Vector2(this._MaxDistance / _TextureLengthScale, 1);
            this._LineRenderer.sharedMaterial.mainTextureOffset -= new Vector2(Time.deltaTime * _TextureScrollSpeed, 0);
        }
        
        this._BeamStart.transform.position = start;
        this._BeamEnd.transform.position = end;
        this._BeamStart.transform.LookAt(end);
        this._BeamEnd.transform.LookAt(start);

    }

    // Update is called once per frame
    void Update()
    {
        UpdatePoints();
        UpdateLineRenderer();
    }
    
    // public void ShootBeamInDir(Vector3 start, Vector3 dir)
    // {
    //     this._LineRenderer.positionCount = 2;
    //     this._LineRenderer.SetPosition(0, start);
    //     this._BeamStart.transform.position = start;
    //     
    //     Vector3 end = Vector3.zero;
    //     RaycastHit hit;
    //     if (Physics.Raycast(start, dir, out hit))
    //         end = hit.point - (dir.normalized * _BeamEndOffset);
    //     else
    //         end = transform.position + (dir * _MaxDistance);
    //     
    //     this._BeamEnd.transform.position = end;
    //     this._LineRenderer.SetPosition(1, end);
    //     
    //     this._BeamStart.transform.LookAt(end);
    //     this._BeamEnd.transform.LookAt(start);
    //     
    //     float distance = Vector3.Distance(start, end);
    //     this._LineRenderer.sharedMaterial.mainTextureScale = new Vector2(distance / _TextureLengthScale, 1);
    //     this._LineRenderer.sharedMaterial.mainTextureOffset -= new Vector2(Time.deltaTime * _TextureScrollSpeed, 0);
    // }
}
