using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellIndicatorTest : MonoBehaviour
{
   public SpellIndicator _SpellIndicator;
   public SpellIndicator.SpellIndicatorType _Type = SpellIndicator.SpellIndicatorType.Line;

   RaycastHit _HitInfo = new RaycastHit();

   private GUIStyle k_Label;
   void Awake()
   {
      k_Label = new GUIStyle("label");
      k_Label.fontSize = 20;
      k_Label.normal.textColor = Color.blue;
      k_Label.alignment = TextAnchor.LowerLeft;
      k_Label.stretchHeight = true;
      k_Label.margin.top = 0;
      k_Label.margin.bottom = 0;
   }

   void OnGUI()
   {
      float y = 0;
      addPropertyGUI("按下鼠标左键控制拖动，显示技能指向。", ref y);
      addPropertyGUI("按A键: 切换箭头类型", ref y);
      addPropertyGUI("按B键: 切换带缩放的箭头类型", ref y);
      addPropertyGUI("按C键: 切换扇区类型", ref y);
      addPropertyGUI("按D键: 切换圆形类型", ref y);
   }

   private float margin = 10;
   private float height = 30;
   private float width = Screen.width;
   void addPropertyGUI(string content, ref float y)
   {
      GUI.Label(new Rect(margin, y, width, height), content, k_Label);
      y += height;
   }

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
