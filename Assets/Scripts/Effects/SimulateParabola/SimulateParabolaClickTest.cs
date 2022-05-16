using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulateParabolaClickTest : MonoBehaviour
{
    [SerializeField] 
    private GameObject _ParabolableGameObject;
    [SerializeField]
    private SimulateParabola _Parabola;
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
                _Parabola.SetPoints(new []{startPoint, midPoint, endPoint});
                // if (Vector3.Distance(startPoint, endPoint) < 3)
                // {
                //     _Parabola.SetBezierPointCount(50);
                // }
                // else
                {
                    _Parabola.SetBezierPointCount(20);
                }
                
                // 投掷
                {
                    GameObject go = GameObject.Instantiate(_ParabolableGameObject);
                    go.transform.position = startPoint;
                    Parabolable parabolable = go.GetComponent<Parabolable>();
                    parabolable.SetPoints(_Parabola.GetBezierPoints());
                }
            }
        }
    }
}
