using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamTest : MonoBehaviour
{
   public Beam _Beam;

   RaycastHit _HitInfo;

   void Awake()
   {
//      Application.targetFrameRate = 20;
   }

   void Start()
   {
      this._Beam.gameObject.SetActive(false);   
   }

   void Update()
   {
      // 显示光束
      if (Input.GetMouseButtonDown(0))
      {
         var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
         if (Physics.Raycast(ray.origin, ray.direction, out _HitInfo, 1000, 1 << LayerMask.NameToLayer("Ground")))
         {
            Vector3 endPoint = _HitInfo.point;
            Vector3 startPoint = transform.position;
            Vector3 direction = endPoint - startPoint;
            direction.y = 0;

            this.transform.forward = direction;
            // this._Beam.ShootBeamInDir(startPoint, endPoint);
         }
         
         this._Beam.SetBeamInfo(this.transform);
         this._Beam.gameObject.SetActive(true);
      }
      if (Input.GetMouseButtonUp(0))
      {
         this._Beam.gameObject.SetActive(false);   
      }
      
      if (Input.GetMouseButton(0))
      {
         var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
         if (Physics.Raycast(ray.origin, ray.direction, out _HitInfo, 1000, 1 << LayerMask.NameToLayer("Ground")))
         {
            Vector3 endPoint = _HitInfo.point;
            Vector3 startPoint = transform.position;
            Vector3 direction = endPoint - startPoint;
            direction.y = 0;

            this.transform.forward = direction;
            // this._Beam.ShootBeamInDir(startPoint, endPoint);
         }
      }
      else
      {
         // _SpellIndicator.HideSpellIndicator();
      }
   }
}
