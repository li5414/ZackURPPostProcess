using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Plastic.Antlr3.Runtime.Misc;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;
using Zack.Editor;

namespace Skill.Editor
{
   public partial class SkillEditor
   {
      public class Group
      {
         protected enum GroupHierarchyStyle
         {
            // "[start~end]"
            ShowStartEnd,
            // "[start]"
            OnlyStart,
         }
         
         // 组标题
         protected string _Title = String.Empty;
         // 组展开
         protected bool _Foldout = true;
         // action列表
         public List<SkillAction> actions = new ListStack<SkillAction>();
         // Hierarchy显示样式
         protected GroupHierarchyStyle _HierarchyStyle = GroupHierarchyStyle.ShowStartEnd;

         /// <summary>
         /// 组UI
         /// </summary>
         public void OnGroupGUI()
         {
            using (new GUILayoutHorizontal(GUILayout.Height(k_ElementHeight)))
            {
               EditorUtils.CreateToggle(ref this._Foldout, this._Title, EditorParameters.k_Foldout);
               GUILayout.FlexibleSpace();
            }
         }

         /// <summary>
         /// 绘制Hierarchy面板
         /// </summary>
         /// <param name="isGroupSelected"></param>
         /// <param name="selectedItemIndex"></param>
         /// <returns>返回数值大于-1，表示当前选中了该group中的index</returns>
         public virtual int OnGroupHierarchyGUI(bool isGroupSelected, int selectedItemIndex)
         {
            if (!this._Foldout)
            {
               return -1;
            }

            int newSelectedItemIndex = -1;   // 新选中的Item索引
            SkillAction action;
            for (int i = 0; i < this.actions.Count; ++i)
            {
               action = this.actions[i];

               bool isSelected = isGroupSelected && selectedItemIndex==i;
               GUIStyle backgroundStyle = isSelected ? EditorParameters.k_BackgroundSelected : EditorParameters.k_BackgroundEven;
               using (new GUILayoutHorizontal(backgroundStyle, GUILayout.Height(k_ElementHeight)))
               {
                  switch (this._HierarchyStyle)
                  {
                  case GroupHierarchyStyle.ShowStartEnd:
                     GUILayout.Label($"[{action.timelineData.start}~{action.timelineData.end}]", EditorParameters.k_Label);   // 注意GUIStyle会影响高度
                     break;
                  case GroupHierarchyStyle.OnlyStart:
                     GUILayout.Label($"[{action.timelineData.start}]", EditorParameters.k_Label);   // 注意GUIStyle会影响高度
                     break;
                  }
                  GUILayout.FlexibleSpace();
               }

               var rect = GUILayoutUtility.GetLastRect();
               if (handleElementClick(rect))
               {
                  newSelectedItemIndex = i;
               }
            }

            return newSelectedItemIndex;
         }

         /// <summary>
         /// 绘制timeline面板
         /// </summary>
         /// <param name="isGroupSelected"></param>
         /// <param name="selectedItemIndex"></param>
         /// <returns>返回数值大于-1，表示当前选中了该group中的index</returns>
         public virtual int OnGroupTimelineGUI(bool isGroupSelected, int selectedItemIndex)
         {
            if (!this._Foldout)
            {
               return -1;
            }

            int newSelectedItemIndex = -1;   // 新选中的Item索引
            GUILayout.Space(k_ElementHeight);
            SkillAction action;
            for (int i = 0; i < this.actions.Count; ++i)
            {
               action = this.actions[i];
               
               bool isSelected = isGroupSelected && selectedItemIndex==i;
               GUIStyle backgroundStyle = isSelected ? EditorParameters.k_BackgroundSelected : EditorParameters.k_BackgroundEven;
               using (new GUILayoutHorizontal(backgroundStyle, GUILayout.Height(k_ElementHeight)))
               {
                  drawTimelineData(action.timelineData);
               }
               
               var rect = GUILayoutUtility.GetLastRect();
               if (handleElementClick(rect))
               {
                  newSelectedItemIndex = i;
               }
            }
            
           
            return newSelectedItemIndex;
         }

         /// <summary>
         /// 绘制Inspector面板
         /// </summary>
         /// <param name="selectedItemIndex"></param>
         public virtual void OnInspectorGUI(int selectedItemIndex, int maxFrameLength)
         {
            if (selectedItemIndex < this.actions.Count)
            {
               SkillAction action = this.actions[selectedItemIndex];
               TimelineData timelineData = action.timelineData;
               
               using (new GUILayoutVertical(EditorParameters.k_WindowBackground))
               {
                  EditorUtils.CreateIntField("起始", ref timelineData.start, 0, maxFrameLength);
                  EditorUtils.CreateIntField("结束", timelineData.end);
                  EditorUtils.CreateIntField("时长", ref timelineData.length, 0, maxFrameLength-timelineData.start);
               }
            }
         }

         /// <summary>
         /// 点击检测
         /// </summary>
         /// <param name="rect"></param>
         /// <returns></returns>
         protected bool handleElementClick(Rect rect)
         {
            Event e = Event.current;
            if (e.button == 0 && e.type == EventType.MouseUp && rect.Contains(e.mousePosition))
            {
               GUI.FocusControl(null);
               e.Use();

               return true;
            }

            return false;
         }
         
         /// <summary>
         /// 绘制timeline条
         /// </summary>
         /// <param name="timelineData"></param>
         protected void drawTimelineData(TimelineData timelineData)
         {
            int length = timelineData.length;
            Rect rect = EditorUtils.CalculateTimeRect(timelineData.start, timelineData.end, k_ElementHeight);

            float halfWidth = rect.width / 2;
            GUILayout.Space(rect.x);
//            using (new GUIColor(length > 0 ? GUI.color : Color.red))
            {
               GUILayout.Box(GUIContent.none, EditorParameters.k_TimelineBlockLeft, GUILayout.Width(halfWidth));
               GUILayout.Box(GUIContent.none, EditorParameters.k_TimelineBlockRight, GUILayout.Width(halfWidth));
            }
         }

      }

      public class SkillAnimationGroup : Group
      {

         public SkillAnimationGroup()
         {
            this._Title = "动画";
         }

         public override int OnGroupHierarchyGUI(bool isGroupSelected, int selectedItemIndex)
         {
            return base.OnGroupHierarchyGUI(isGroupSelected, selectedItemIndex);
         }

         public override int OnGroupTimelineGUI(bool isGroupSelected, int selectedItemIndex)
         {
            return base.OnGroupTimelineGUI(isGroupSelected, selectedItemIndex);
         }

         public override void OnInspectorGUI(int selectedItemIndex, int maxFrameLength)
         {
            base.OnInspectorGUI(selectedItemIndex, maxFrameLength);
            
            if (selectedItemIndex < this.actions.Count)
            {
               SkillAnimationAction action = this.actions[selectedItemIndex] as SkillAnimationAction;
               
               EditorUtils.CreateText("参数列表:", EditorParameters.k_BoldLabel);
               // 参数列表
               for (int i = 0; i < action.parameters.Count; ++i)
               {
                  // 绘制Animator参数
                  drawAnimatorParameter(action.parameters, i);
               }
               EditorUtils.CreateButton("添加", EditorParameters.k_ACButton, () =>
               {
                  action.parameters.Add(new AnimatorParameter());
               }, GUILayout.Height(k_ElementHeight));
            }
         }

         /// <summary>
         /// 绘制Animator参数
         /// </summary>
         /// <param name="parameter"></param>
         /// <param name="allParameters"></param>
         void drawAnimatorParameter(List<AnimatorParameter> allParameters, int index)
         {
            AnimatorParameter parameter = allParameters[index];
               
            using (new GUILayoutVertical(EditorParameters.k_WindowBackground))
            {
               using (new GUILayoutHorizontal(GUILayout.Height(k_ElementHeight)))
               {
                  EditorUtils.CreateLabel($"参数{index}");
                  GUILayout.FlexibleSpace();
                  EditorUtils.CreateButton("移除", EditorParameters.k_ACButton, () =>
                  {
                     allParameters.Remove(parameter);
                  }, GUILayout.Width(50));
               }
               using (new GUILayoutHorizontal())
               {
                  EditorUtils.CreateLabel("变量类型");
                  GUILayout.FlexibleSpace();

                  EditorUtils.CreateButton(parameter.type.ToString(), EditorParameters.k_DropDownButton, () =>
                  {
                     EditorUtils.CreateMenu(SkillEditor.k_AnimatorParameterTypes, -1, (sindex) =>
                     {
                        parameter.type = (AnimatorParameterType) sindex;
                     });
                  }, GUILayout.Width(83));
               }
               EditorUtils.CreateTextField("变量名:", ref parameter.name);
               
               switch (parameter.type)
               {
                  case AnimatorParameterType.Trigger:
                     {
                     }
                     break;
            
                  case AnimatorParameterType.Bool:
                     {
                        EditorUtils.CreateBoolField("变量值:", ref parameter.bvalue);
                     }
                     break;
            
                  case AnimatorParameterType.Int:
                     {
                        EditorUtils.CreateIntField("变量值:", ref parameter.ivalue);
                     }
                     break;
            
                  case AnimatorParameterType.Float:
                     {
                        EditorUtils.CreateFloatField("变量值:", ref parameter.fvalue);
                     }
                     break;
               }

            }
         }
         
      }
      
      public class SkillEffectGroup : Group
      {

         public SkillEffectGroup()
         {
            this._Title = "特效";
         }

         public override int OnGroupHierarchyGUI(bool isGroupSelected, int selectedItemIndex)
         {
            return base.OnGroupHierarchyGUI(isGroupSelected, selectedItemIndex);
         }

         public override int OnGroupTimelineGUI(bool isGroupSelected, int selectedItemIndex)
         {
            return base.OnGroupTimelineGUI(isGroupSelected, selectedItemIndex);
         }

         public override void OnInspectorGUI(int selectedItemIndex, int maxFrameLength)
         {
            base.OnInspectorGUI(selectedItemIndex, maxFrameLength);
            
            if (selectedItemIndex < this.actions.Count)
            {
               SkillEffectAction action = this.actions[selectedItemIndex] as SkillEffectAction;
               
               using (new GUILayoutVertical(EditorParameters.k_WindowBackground))
               {
                  if (action.mainObject==null && action.guid!=string.Empty)
                  {
                     action.mainObject = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(action.guid));
                  }
                  
                  action.mainObject = (GameObject)EditorGUILayout.ObjectField("特效:", action.mainObject, typeof(GameObject), false);
                  action.guid = EditorUtils.GetGameObjectGUID(action.mainObject);
               }
            }
         }
         
      }
      
      public class SkillEventGroup : Group
      {

         public SkillEventGroup()
         {
            this._Title = "事件";
            this._HierarchyStyle = GroupHierarchyStyle.OnlyStart;
         }

         public override int OnGroupHierarchyGUI(bool isGroupSelected, int selectedItemIndex)
         {
            return base.OnGroupHierarchyGUI(isGroupSelected, selectedItemIndex);
         }

         public override int OnGroupTimelineGUI(bool isGroupSelected, int selectedItemIndex)
         {
            return base.OnGroupTimelineGUI(isGroupSelected, selectedItemIndex);
         }

         public override void OnInspectorGUI(int selectedItemIndex, int maxFrameLength)
         {
            base.OnInspectorGUI(selectedItemIndex, maxFrameLength);
            
            if (selectedItemIndex < this.actions.Count)
            {
               SkillEventAction action = this.actions[selectedItemIndex] as SkillEventAction;
               // 事件没有时长一说
               if (action.timelineData.length > 0)
               {
                  action.timelineData.length = 0;
               }
               
               using (new GUILayoutVertical(EditorParameters.k_WindowBackground))
               {
                  EditorUtils.CreateTextField("事件名称", ref action.eventName);
                  EditorUtils.CreateTextField("事件参数", ref action.eventParams);
               }
            }
         }
         
      }
      
      
      
   }
   

}

