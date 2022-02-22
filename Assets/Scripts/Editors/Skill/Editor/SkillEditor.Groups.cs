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
            // 自定义 (显示_HierarchyText内容)
            Custom,
         }
         
         // 组标题
         protected string _Title = String.Empty;
         // 组展开
         protected bool _Foldout = true;
         // action列表
         public List<SkillAction> actions = new ListStack<SkillAction>();
         // Hierarchy显示样式
         protected GroupHierarchyStyle _HierarchyStyle = GroupHierarchyStyle.ShowStartEnd;
         protected string _HierarchyText = string.Empty;
         // 是否可编辑timeline
         protected bool _CanEditTimeline = true;

         /// <summary>
         /// 组UI
         /// </summary>
         public void OnGroupGUI(SkillEditor window)
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
         public virtual int OnGroupHierarchyGUI(SkillEditor window, bool isGroupSelected, int selectedItemIndex)
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
                  case GroupHierarchyStyle.Custom:
                     GUILayout.Label(this._HierarchyText, EditorParameters.k_Label);   // 注意GUIStyle会影响高度
                     break;
                  }
                  GUILayout.FlexibleSpace();
               }

               var rect = GUILayoutUtility.GetLastRect();
               if (handleElementClick(rect))
               {
                  // 鼠标左击
                  newSelectedItemIndex = i;
               }
               else if (handleElementClick(rect, 1))
               {
                  // 鼠标右击
                  newSelectedItemIndex = i;
                  EditorUtils.CreateMenu(new string[]{"删除"}, -1, (index) =>
                  {
                     actions.RemoveAt(newSelectedItemIndex);
                  });
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
         public virtual int OnGroupTimelineGUI(SkillEditor window, bool isGroupSelected, int selectedItemIndex)
         {
            GUILayout.Space(k_ElementHeight);
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
                  drawTimelineData(action.timelineData);
               }
               
               var rect = GUILayoutUtility.GetLastRect();
               if (handleElementClick(rect))
               {
                  // 鼠标左击
                  newSelectedItemIndex = i;
               }
               else if (handleElementClick(rect, 1))
               {
                  // 鼠标右击
                  newSelectedItemIndex = i;
                  EditorUtils.CreateMenu(new string[]{"删除"}, -1, (index) =>
                  {
                     actions.RemoveAt(newSelectedItemIndex);
                  });
               }
            }
            
           
            return newSelectedItemIndex;
         }

         /// <summary>
         /// 绘制Inspector面板
         /// </summary>
         /// <param name="selectedItemIndex"></param>
         public virtual void OnInspectorGUI(SkillEditor window, int selectedItemIndex, int maxFrameLength)
         {
            if (selectedItemIndex < this.actions.Count)
            {
               SkillAction action = this.actions[selectedItemIndex];
               TimelineData timelineData = action.timelineData;
               
               using (new GUILayoutVertical(EditorParameters.k_WindowBackground))
               {
                  if (this._CanEditTimeline)
                  {
                     EditorUtils.CreateIntField("起始", ref timelineData.start, 0, maxFrameLength);
                     EditorUtils.CreateIntFieldDisable("结束", timelineData.end);
                     EditorUtils.CreateIntField("时长", ref timelineData.length, 0, maxFrameLength-timelineData.start);
                  }
                  else
                  {
                     EditorUtils.CreateIntFieldDisable("起始", timelineData.start);
                     EditorUtils.CreateIntFieldDisable("结束", timelineData.end);
                     EditorUtils.CreateIntFieldDisable("时长", timelineData.length);
                  }

               }
            }
         }

         /// <summary>
         /// 点击检测
         /// </summary>
         /// <param name="rect"></param>
         /// <param name="buttonId">0:鼠标左键, 1:鼠标右键</param>
         /// <returns></returns>
         protected bool handleElementClick(Rect rect, int buttonId = 0)
         {
            Event e = Event.current;
            if (e.button == buttonId && e.type == EventType.MouseUp && rect.Contains(e.mousePosition))
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
            rect.x -= EditorParameters.k_TickGap / 2;
            
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
            this._CanEditTimeline = false;
         }

         public override int OnGroupHierarchyGUI(SkillEditor window, bool isGroupSelected, int selectedItemIndex)
         {
            return base.OnGroupHierarchyGUI(window, isGroupSelected, selectedItemIndex);
         }

         public override int OnGroupTimelineGUI(SkillEditor window, bool isGroupSelected, int selectedItemIndex)
         {
            return base.OnGroupTimelineGUI(window, isGroupSelected, selectedItemIndex);
         }

         public override void OnInspectorGUI(SkillEditor window, int selectedItemIndex, int maxFrameLength)
         {
            base.OnInspectorGUI(window, selectedItemIndex, maxFrameLength);
            
            if (selectedItemIndex < this.actions.Count)
            {
               SkillAnimationAction action = this.actions[selectedItemIndex] as SkillAnimationAction;
               using (new GUILayoutVertical(EditorParameters.k_WindowBackground, GUILayout.Height(k_ElementHeight)))
               {
                  // 动画
                  using (new GUILayoutHorizontal())
                  {
                     using (new GUIChangeCheck(() =>
                     {
                        window.UpdateAnimationActions();
                     }))
                     {
                        // 选择状态
                        EditorUtils.CreateText("选择动画:", EditorParameters.k_Label);
                        GUILayout.FlexibleSpace();
                        EditorUtils.CreateButton(action.state.GetDescription(), EditorParameters.k_DropDownButton, () =>
                        {
                           EditorUtils.CreateMenu<SkillAnimatorState>(-1, (index) =>
                           {
                              action.state = (SkillAnimatorState) index;
                              window.UpdateAnimationActions();
                           });
                        }, GUILayout.Width(83));
                     }
                  }
                  
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

         public override int OnGroupHierarchyGUI(SkillEditor window, bool isGroupSelected, int selectedItemIndex)
         {
            return base.OnGroupHierarchyGUI(window, isGroupSelected, selectedItemIndex);
         }

         public override int OnGroupTimelineGUI(SkillEditor window, bool isGroupSelected, int selectedItemIndex)
         {
            return base.OnGroupTimelineGUI(window, isGroupSelected, selectedItemIndex);
         }

         public override void OnInspectorGUI(SkillEditor window, int selectedItemIndex, int maxFrameLength)
         {
            base.OnInspectorGUI(window, selectedItemIndex, maxFrameLength);
            
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
//                  Debug.Log(AssetDatabase.GetLabels(action.mainObject)[0]);
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

         public override int OnGroupHierarchyGUI(SkillEditor window, bool isGroupSelected, int selectedItemIndex)
         {
            return base.OnGroupHierarchyGUI(window, isGroupSelected, selectedItemIndex);
         }

         public override int OnGroupTimelineGUI(SkillEditor window, bool isGroupSelected, int selectedItemIndex)
         {
            return base.OnGroupTimelineGUI(window, isGroupSelected, selectedItemIndex);
         }

         public override void OnInspectorGUI(SkillEditor window, int selectedItemIndex, int maxFrameLength)
         {
            base.OnInspectorGUI(window, selectedItemIndex, maxFrameLength);
            
            if (selectedItemIndex < this.actions.Count)
            {
               SkillEventAction action = this.actions[selectedItemIndex] as SkillEventAction;
               // 事件没有时长一说
               if (action.timelineData.length > 0)
               {
                  action.timelineData.length = 0;
               }
                  
               drawEventComponents(window, action);
            }
         }

         // 绘制组件列表
         void drawEventComponents(SkillEditor window, SkillEventAction action)
         {
            GenericMenu menu = new GenericMenu();
            
            // timescaleComponent
            if (!window.drawTimescaleComponent(action))
            {
               menu.AddItem(new GUIContent(action.timescaleComponent.GetDescripthion()), false, () =>
               {
                  action.timescaleComponent = new TimescaleComponent();
               });
            }
            
            
            EditorUtils.CreateButton("添加组件", EditorParameters.k_ACButton, () =>
            {
               menu.ShowAsContext();
            }, GUILayout.Height(k_ElementHeight));
         }
         
         
      }
      
      public class SkillCustomEventGroup : Group
      {
         public SkillCustomEventGroup()
         {
            this._Title = "自定义回调";
            this._HierarchyStyle = GroupHierarchyStyle.OnlyStart;
         }

         public override int OnGroupHierarchyGUI(SkillEditor window, bool isGroupSelected, int selectedItemIndex)
         {
            return base.OnGroupHierarchyGUI(window, isGroupSelected, selectedItemIndex);
         }

         public override int OnGroupTimelineGUI(SkillEditor window, bool isGroupSelected, int selectedItemIndex)
         {
            return base.OnGroupTimelineGUI(window, isGroupSelected, selectedItemIndex);
         }

         public override void OnInspectorGUI(SkillEditor window, int selectedItemIndex, int maxFrameLength)
         {
            base.OnInspectorGUI(window, selectedItemIndex, maxFrameLength);
            
            if (selectedItemIndex < this.actions.Count)
            {
               SkillCustomEventAction action = this.actions[selectedItemIndex] as SkillCustomEventAction;
               // 事件没有时长一说
               if (action.timelineData.length > 0)
               {
                  action.timelineData.length = 0;
               }

               EditorUtils.CreateTextField("functionName", ref action.functionName);
            }
         }

         
      }
      
      
      
   }
   

}

