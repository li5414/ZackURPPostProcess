using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.Mathematics;
using Unity.Plastic.Antlr3.Runtime.Misc;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;
using Zack.Editor;
using Object = System.Object;

namespace Skill.Editor
{
   public partial class SkillEditor
   {
      // 绘制组件
      // =========================SkillEffectAction的组件=======================================
      bool drawEffectComponent(SkillEffectAction action, string fieldName)
      {
         SkillComponent component = null;

         var field = action.GetType().GetField(fieldName,
            BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
         if (field != null)
         {
            component = field.GetValue(action) as SkillComponent;
         }

         if (component == null)
         {
            return false;
         }

         using (new GUILayoutVertical(EditorStyles.helpBox, GUILayout.Height(k_ElementHeight)))
         {
            // 属性
            drawComponentProperties(component);
         }
         
         // 点击事件
         if(HitTest())
         {
            EditorUtils.CreateMenu(new string[]{"删除"}, -1, (index) =>
            {
               if (field != null)
               {
                  field.SetValue(action, null);
               }
            });
         }
         
         return true;
      }
      // =========================SkillEffectAction的组件=======================================

      
      // =========================SkillEventAction的组件=======================================
      bool drawEventComponent(SkillEventAction action, string fieldName)
      {
         SkillComponent component = null;

         var field = action.GetType().GetField(fieldName,
            BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
         if (field != null)
         {
            component = field.GetValue(action) as SkillComponent;
         }

         if (component == null)
         {
            return false;
         }

         using (new GUILayoutVertical(EditorStyles.helpBox, GUILayout.Height(k_ElementHeight)))
         {
            // 属性
            drawComponentProperties(component);
         }
         
         // 点击事件
         if(HitTest())
         {
            EditorUtils.CreateMenu(new string[]{"删除"}, -1, (index) =>
            {
               if (field != null)
               {
                  field.SetValue(action, null);
               }
            });
         }
         
         return true;
      }
      // =========================SkillEventAction的组件=======================================

      bool HitTest()
      {
         Rect rect = GUILayoutUtility.GetLastRect();
         Event e = Event.current;
         if (e.button == 1 && e.type == EventType.MouseUp && rect.Contains(e.mousePosition))
         {
            GUI.FocusControl(null);
            e.Use();
            return true;
         }

         return false;
      }

      void drawComponentProperties<T>(T component) where T : SkillComponent
      {
         // 属性列表
         Type type = component.GetType();
         // Title
         EditorUtils.CreateText(type.GetDescripthion(), EditorParameters.k_BoldLabel);
         
         FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
         foreach (var field in fields)
         {
            // 属性描述
            var desc = field.GetDescripthion();
            if (desc == null || desc == string.Empty)
            {
               continue;
            }

            // 绘制属性
            if (field.FieldType.IsEnum)
            {
               // 枚举类型属性
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
            else if (field.FieldType == typeof(SkillAsset))
            {
               SkillAsset value = (SkillAsset)field.GetValue(component);
               if (value == null)
               {
                  value = new SkillAsset();
               }
               if (value.mainObject==null && value.guid!=string.Empty)
               {
                  value.mainObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(AssetDatabase.GUIDToAssetPath(value.guid));
               }

               SkillAssetType assetType = EditorUtils.GetSkillAssetType(field);
               Type unityAssetType;
               switch (assetType)
               {
                  case SkillAssetType.GameObject:
                     unityAssetType = typeof(GameObject);
                     break;
                  case SkillAssetType.AudioClip:
                     unityAssetType = typeof(AudioClip);
                     break;
                  default:
                     unityAssetType = typeof(GameObject);
                     break;
               }
                  
               value.mainObject = EditorGUILayout.ObjectField(desc, value.mainObject, unityAssetType, false);
               value.guid = EditorUtils.GetGameObjectGUID(value.mainObject);
               value.type = assetType;
               // 更新资源ab信息
               EditorUtils.UpdateSkillAssetBundleInfo(value);
               field.SetValue(component, value);
            }
            else if (field.FieldType == typeof(string))
            {
               // string类型属性
               string value = EditorUtils.CreateTextField(desc, (string)field.GetValue(component));
               field.SetValue(component, value);
            }
            else if (field.FieldType == typeof(Vector4))
            {
               // Vector4类型属性
               Vector4 value = EditorUtils.CreateVector4Field(desc, (Vector4)field.GetValue(component), GUILayout.Width(200));
               field.SetValue(component, value);
            }
            else if (field.FieldType == typeof(Vector3))
            {
               // Vector3类型属性
               Vector3 value = EditorUtils.CreateVector3Field(desc, (Vector3)field.GetValue(component), GUILayout.Width(150));
               field.SetValue(component, value);
            }
            else if (field.FieldType == typeof(Vector2))
            {
               // Vector2类型属性
               Vector2 value = EditorUtils.CreateVector2Field(desc, (Vector2)field.GetValue(component), GUILayout.Width(100));
               field.SetValue(component, value);
            }
            else if (field.FieldType == typeof(float))
            {
               // float类型属性
               float value = EditorUtils.CreateFloatField(desc, (float)field.GetValue(component));
               // 范围
               RangeAttribute attribute = EditorUtils.GetFieldAttribute<RangeAttribute>(field);
               if (attribute != null)
               {
                  value = math.clamp(value, attribute.min, attribute.max);
               }
               field.SetValue(component, value);
            }
            else if (field.FieldType == typeof(int))
            {
               // int类型属性
               int value = EditorUtils.CreateIntField(desc, (int)field.GetValue(component));
               // 范围
               RangeAttribute attribute = EditorUtils.GetFieldAttribute<RangeAttribute>(field);
               if (attribute != null)
               {
                  value = math.clamp(value, (int)attribute.min, (int)attribute.max);
               }
               field.SetValue(component, value);
            }
            else if (field.FieldType == typeof(bool))
            {
               // bool类型属性
               bool value = EditorUtils.CreateBoolField(desc, (bool)field.GetValue(component));
               field.SetValue(component, value);
            }
            else if (field.FieldType == typeof(List<SkillAsset>))
            {
               // List<SkillAsset>类型属性
               EditorUtils.CreateLabel(desc);
               
               // test list
               List<SkillAsset> value = (List<SkillAsset>) field.GetValue(component);
               if (value == null)
               {
                  value = new List<SkillAsset>();
               }
               EditorUtils.CreateListField(value, item =>
               {
                  if (item.mainObject==null && item.guid!=string.Empty)
                  {
                     item.mainObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(AssetDatabase.GUIDToAssetPath(item.guid));
                  }

                  SkillAssetType assetType = EditorUtils.GetSkillAssetType(field);
                  Type unityAssetType;
                  switch (assetType)
                  {
                     case SkillAssetType.GameObject:
                        unityAssetType = typeof(GameObject);
                        break;
                     case SkillAssetType.AudioClip:
                        unityAssetType = typeof(AudioClip);
                        break;
                     default:
                        unityAssetType = typeof(GameObject);
                        break;
                  }
                  
                  item.mainObject = EditorGUILayout.ObjectField(desc, item.mainObject, unityAssetType, false);
                  item.guid = EditorUtils.GetGameObjectGUID(item.mainObject);
                  item.type = assetType;
               });
               field.SetValue(component, value);
            }
            
               
         }

         
         // PrefabComponent类型的对象需要更新Transform信息按钮
         if (typeof(PrefabComponent).IsInstanceOfType(component))
         {
            PrefabComponent prefabComponent = component as PrefabComponent;
            
            EditorUtils.CreateButton("复制选中节点信息", EditorParameters.k_ACButton, () =>
            {
               if (Selection.transforms.Length > 0)
               {
                  Transform transform = Selection.transforms[0];
                  prefabComponent.position = transform.localPosition;
                  prefabComponent.eulerAngles = transform.localEulerAngles;
                  prefabComponent.scale = transform.localScale;
               }
            }, GUILayout.Height(k_ElementHeight));
            
            EditorUtils.CreateButton(prefabComponent.prefabObject.runtimeObject ? "销毁预览" : "模型预览", EditorParameters.k_ACButton, () =>
            {
                  if (prefabComponent.prefabObject.runtimeObject)
                  {
                     // 销毁预览
                     GameObject.Destroy(prefabComponent.prefabObject.runtimeObject);
                     prefabComponent.prefabObject.runtimeObject = null;
                  }
                  else
                  {
                     // 模型预览
                     if (prefabComponent.prefabObject.mainObject)
                     {
                        GameObject go = GameObject.Instantiate(prefabComponent.prefabObject.mainObject, Vector3.zero, Quaternion.identity) as GameObject;
                        prefabComponent.BindGameObject(this._MainCharacter, go);
                     }
                  }
            });
            
            // 更新模型位置
            if (prefabComponent.prefabObject.runtimeObject)
            {
               prefabComponent.BindGameObject(this._MainCharacter, prefabComponent.prefabObject.runtimeObject as GameObject);
            }
         }
      }
      // =========================SkillEventAction的组件=======================================

      
      
      
   }
   

}

