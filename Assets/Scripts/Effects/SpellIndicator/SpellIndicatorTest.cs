using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellIndicatorTest : MonoBehaviour
{
   public SpellIndicator _SpellIndicator;
   public SpellIndicator.SpellIndicatorType _Type = SpellIndicator.SpellIndicatorType.Line;

   RaycastHit _HitInfo;

   void Update()
   {
      // 切换指向类型
      if (Input.GetKeyDown(KeyCode.A))
      {
         _Type = SpellIndicator.SpellIndicatorType.Line;
      }
      else if (Input.GetKeyDown(KeyCode.B))
      {
         _Type = SpellIndicator.SpellIndicatorType.ScaleLine;
      }
      else if (Input.GetKeyDown(KeyCode.C))
      {
         _Type = SpellIndicator.SpellIndicatorType.Cone;
      }
      else if (Input.GetKeyDown(KeyCode.D))
      {
         _Type = SpellIndicator.SpellIndicatorType.Circle;
      }
      
      // 显示技能指向
      if (Input.GetMouseButton(0))
      {
         var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
         if (Physics.Raycast(ray.origin, ray.direction, out _HitInfo, 1000, 1 << LayerMask.NameToLayer("Ground")))
         {
            Vector3 endPoint = _HitInfo.point;
            Vector3 startPoint = transform.position;
            Vector3 direction = endPoint - startPoint;
            switch (_Type)
            {
               case SpellIndicator.SpellIndicatorType.Line:
                  _SpellIndicator.ShowSpellIndicator(SpellIndicator.SpellIndicatorType.Line, new Vector2(direction.x, direction.z));
                  break;
               
               case SpellIndicator.SpellIndicatorType.ScaleLine:
                  _SpellIndicator.ShowSpellIndicator(SpellIndicator.SpellIndicatorType.ScaleLine, new Vector2(direction.x, direction.z), direction.magnitude);
                  break;
               
               case SpellIndicator.SpellIndicatorType.Cone:
                  _SpellIndicator.ShowSpellIndicator(SpellIndicator.SpellIndicatorType.Cone, new Vector2(direction.x, direction.z));
                  break;
               
               case SpellIndicator.SpellIndicatorType.Circle:
                  _SpellIndicator.ShowSpellIndicator(SpellIndicator.SpellIndicatorType.Circle, new Vector2(direction.x, direction.z), direction.magnitude, 1f, 3f);
                  break;
            }
         }
      }
      else
      {
         _SpellIndicator.HideSpellIndicator();
      }
   }
}
