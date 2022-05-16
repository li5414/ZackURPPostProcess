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
        if (Input.GetMouseButtonDown(1) && !Input.GetKey(KeyCode.LeftShift))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray.origin, ray.direction, out _HitInfo, 1000, 1<<LayerMask.NameToLayer("Ground")))
            {
                Vector3 startPoint = transform.position;
                Vector3 endPoint = _HitInfo.point;
                _Parabola.SetStartPoint(startPoint);
                _Parabola.SetEndPoint(endPoint);
                _Parabola.SetMaxHeight(4.5f);
                _Parabola.SetDuration(2f);

            }
        }
        
        if (Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.LeftShift))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray.origin, ray.direction, out _HitInfo, 1000, 1<<LayerMask.NameToLayer("Ground")))
            {
                Vector3 startPoint = transform.position;
                Vector3 endPoint = _HitInfo.point;
                // 投掷
                {
                    GameObject go = GameObject.Instantiate(_ParabolableGameObject);
                    go.transform.position = startPoint;
                    SimulateParabolable parabolable = go.GetComponent<SimulateParabolable>();
                    parabolable.SetPoints(_Parabola.Interval, _Parabola.Points.ToArray(), 1.3f);
                }
            }
        }
    }
}
