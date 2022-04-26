using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class ParabolaClickTest : MonoBehaviour
{
    [SerializeField] 
    private GameObject _ParabolableGameObject;
    [SerializeField]
    private Parabola _Parabola;
    RaycastHit _HitInfo = new RaycastHit();

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
        if (Input.GetMouseButton(1))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray.origin, ray.direction, out _HitInfo, 1000, 1<<LayerMask.NameToLayer("Ground")))
            {
                Vector3 startPoint = transform.position;
                Vector3 endPoint = _HitInfo.point;
                {
                    Rigidbody rigidbody = _ParabolableGameObject.GetComponent<Rigidbody>();
                    _Parabola.PredictParabola(rigidbody, startPoint, endPoint);
                }
                
            }
        }


    }
}
