using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class BounceParabolaClickTest : MonoBehaviour
{
    [SerializeField] 
    private GameObject _ParabolableGameObject;
    [SerializeField]
    private BounceParabola _Parabola;
    RaycastHit _HitInfo = new RaycastHit();


    void Awake()
    {
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
                    
                    _Parabola.SetStartPoint(startPoint);
                    _Parabola.SetEndPoint(endPoint);
                    
                    Rigidbody rigidbody = go.GetComponent<Rigidbody>();
                    _Parabola.AddRigidbodyForce(rigidbody);
                }
            }
        }

        // 预测
        if (Input.GetMouseButtonDown(1))
        {
            _Parabola.SetMaxHeight(3);
            _Parabola.SetRigidbody(this._ParabolableGameObject.GetComponent<Rigidbody>());
            _Parabola.ShowParabola();
        }
        else if (Input.GetMouseButtonUp(1))
        {
            // _Parabola.HideParabola();
        }
        if (Input.GetMouseButton(1))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray.origin, ray.direction, out _HitInfo, 1000, 1<<LayerMask.NameToLayer("Ground")))
            {
                Vector3 startPoint = transform.position;
                Vector3 endPoint = _HitInfo.point;
                {
                    _Parabola.SetStartPoint(startPoint);
                    _Parabola.SetEndPoint(endPoint);

                }
                
            }
        }

       


    }
}
