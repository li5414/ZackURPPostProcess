using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using MB.PhysicsPrediction;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class BounceParabolaClickTest : MonoBehaviour
{
    [SerializeField] 
    private GameObject _ParabolableGameObject;
    [SerializeField]
    private Parabola _Parabola;
    RaycastHit _HitInfo = new RaycastHit();

    private Vector3 _Force = Vector3.zero;
    private PredictionTimeline _Timeline;

    void Awake()
    {
    }

    void Launch(GameObject gameObject)
    {
        var rigidbody = gameObject.GetComponent<Rigidbody>();

        // var relativeForce = transform.TransformVector(force.Vector);

        rigidbody.AddForce(_Force);

        rigidbody.transform.position = transform.position;
        rigidbody.transform.rotation = transform.rotation;
    }

    void Update()
    {
        // 投掷
        if (Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray.origin, ray.direction, out _HitInfo, 1000, 1<<LayerMask.NameToLayer("Ground")))
            {
                Vector3 endPoint = _HitInfo.point;
                Vector3 startPoint = transform.position;
                {
                    GameObject go = GameObject.Instantiate(_ParabolableGameObject);
                    go.transform.position = startPoint;
                    Rigidbody rigidbody = go.GetComponent<Rigidbody>();
                    _Parabola.AddRigidbodyForce(rigidbody, startPoint, endPoint);
                }
            }
        }

        // 预测
        if (Input.GetMouseButtonDown(1))
        {
            _Timeline =  PredictionSystem.Record.Prefabs.Add(_ParabolableGameObject, Launch);
        }
        else if (Input.GetMouseButtonUp(1))
        {
            PredictionSystem.Record.Prefabs.Remove(_Timeline);
            _Timeline = null;
        }
        if (Input.GetMouseButton(1))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray.origin, ray.direction, out _HitInfo, 1000, 1<<LayerMask.NameToLayer("Ground")))
            {
                Vector3 startPoint = transform.position;
                Vector3 endPoint = _HitInfo.point;
                {
                    Rigidbody rigidbody = _ParabolableGameObject.GetComponent<Rigidbody>();
                    _Force = RigidbodyUtils.CalculateParabolaForce(rigidbody.mass, startPoint, endPoint, _Parabola.MaxHeight);

                    if (_Timeline != null)
                    {
                        PredictionSystem.Simulate(200);

                        Vector3[] points = new Vector3[_Timeline.Count];
                        for (int i = 0; i < _Timeline.Count; ++i)
                        {
                            points[i] = _Timeline[i].Position;
                        }
                        _Parabola.SetPoints(points);
                    }

                }
                
            }
        }

       


    }
}
