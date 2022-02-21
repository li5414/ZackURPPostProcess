using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.Plastic.Antlr3.Runtime.Misc;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;
using Zack.Editor;

namespace Skill.Editor
{
   public partial class SkillEditor
   {
      // 绘制组件
      
      // timescale组件
      bool drawTimescaleComponent(TimescaleComponent component)
      {
         if (component == null)
         {
            return false;
         }
         
         using (new GUILayoutVertical(EditorStyles.helpBox, GUILayout.Height(k_ElementHeight)))
         {
            drawComponentProperties(component);
         }

         {
            Rect rect = GUILayoutUtility.GetLastRect();
            Event e = Event.current;
            if (e.button == 1 && e.type == EventType.MouseUp && rect.Contains(e.mousePosition))
            {
               GUI.FocusControl(null);
               e.Use();
            
               EditorUtils.CreateMenu(new string[]{"删除"}, -1, (index) =>
               {
                  Debug.Log("删除");      
               });
            }
         }
         
         return true;
      }

      void drawComponentProperties<T>(T component) where T : SkillComponent
      {
         // Title
         EditorUtils.CreateText((component as T).GetDescripthion(), EditorParameters.k_BoldLabel);
         
         // 属性列表
         Type type = component.GetType();
         FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
         foreach (var field in fields)
         {
            var desc = field.GetDescripthion();
            if (desc == null || desc == string.Empty)
            {
               continue;
            }

            if (field.FieldType.IsEnum)
            {
               using (new GUILayoutHorizontal())
               {
                  EditorUtils.CreateLabel(desc);
                  GUILayout.FlexibleSpace();
                  Enum value = (Enum)field.GetValue(component);
                  EditorUtils.CreateButton(value.GetDescription(), EditorParameters.k_DropDownButton, () =>
                  {
                     EditorUtils.CreateMenu(value, (index) =>
                     {
                        field.SetValue(component, index);
                     });
                  }, GUILayout.Width(83));
               }
            }
            else if (field.FieldType == typeof(float))
            {
               float value = EditorUtils.CreateFloatField(desc, (float)field.GetValue(component));
               field.SetValue(component, value);
            }
            else if (field.FieldType == typeof(int))
            {
               int value = EditorUtils.CreateIntField(desc, (int)field.GetValue(component));
               field.SetValue(component, value);
            }
            
               
         }
      }
      
      
      
      
   }
   

}

