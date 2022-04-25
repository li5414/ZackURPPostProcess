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
                
                // 投掷
                {
                    GameObject go = GameObject.Instantiate(_ParabolableGameObject);
                    go.transform.position = startPoint;
                    // Parabolable parabolable = go.GetComponent<Parabolable>();
                    // parabolable.SetPoints(_Parabola.GetBezierPoints());
                    Rigidbody rigidbody = go.GetComponent<Rigidbody>();

                    Vector3 force = RigidbodyUtils.CalculateForce(rigidbody.mass, startPoint, endPoint, 4);

                    Debug.Log(force);
                    _Parabola.SetRigidbodyForce(rigidbody, startPoint, force);
                    
                    rigidbody.AddForce(force);
                }
            }
        }
    }
}
