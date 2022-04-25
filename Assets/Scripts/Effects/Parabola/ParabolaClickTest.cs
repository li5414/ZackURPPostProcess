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
        if (Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.LeftShift))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray.origin, ray.direction, out _HitInfo, 1000, 1<<LayerMask.NameToLayer("Ground")))
            {
                Vector3 startPoint = transform.position;
                Vector3 endPoint = _HitInfo.point;
                Vector3 midPoint = Vector3.Lerp(startPoint, endPoint, 0.5f);
                midPoint.y = 5;
                // _Parabola.SetPoints(new []{startPoint, midPoint, endPoint});
                // // if (Vector3.Distance(startPoint, endPoint) < 3)
                // // {
                // //     _Parabola.SetBezierPointCount(50);
                // // }
                // // else
                // {
                //     _Parabola.SetBezierPointCount(20);
                // }
                
                // 投掷
                {
                    GameObject go = GameObject.Instantiate(_ParabolableGameObject);
                    go.transform.position = startPoint;
                    // Parabolable parabolable = go.GetComponent<Parabolable>();
                    // parabolable.SetPoints(_Parabola.GetBezierPoints());
                    Rigidbody rigidbody = go.GetComponent<Rigidbody>();

                    Vector3 direction = endPoint - startPoint;
                    Vector3 force = direction*50 + Vector3.up*400;
                    List<Vector3[]> movements = RigidbodyUtils.CalculateMovements(rigidbody, 50, 1, Vector3.zero, force);
                    Vector3[] points = new Vector3[movements.Count];
                    for (int i = 0; i < movements.Count; ++i)
                    {
                        points[i] = movements[i][0];
                    } 
                    _Parabola.SetPoints(points);
                    
                    rigidbody.AddForce(force);
                }
            }
        }
    }
}
