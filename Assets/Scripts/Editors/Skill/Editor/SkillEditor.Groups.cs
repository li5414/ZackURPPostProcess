using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Plastic.Antlr3.Runtime.Misc;
using UnityEditor;
using UnityEngine;
using Zack.Editor;

namespace Zack.Editor.Skill
{
   public partial class SkillEditor
   {
      public class Group
      {
         public string title = String.Empty;
         public bool foldout = true;
         public List<SkillAction> actions = new ListStack<SkillAction>();

         public void OnGroupGUI()
         {
            using (new GUILayoutHorizontal(GUILayout.Height(k_ElementHeight)))
            {
               EditorUtils.CreateToggle(ref foldout, title, EditorParameters.k_Foldout);
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
            return -1;
         }

         /// <summary>
         /// 绘制timeline面板
         /// </summary>
         /// <param name="isGroupSelected"></param>
         /// <param name="selectedItemIndex"></param>
         /// <returns>返回数值大于-1，表示当前选中了该group中的index</returns>
         public virtual int OnGroupTimelineGUI(bool isGroupSelected, int selectedItemIndex)
         {
            return -1;
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
            using (new GUIHandlesColor(length > 0 ? GUI.color : Color.red))
            {
               GUILayout.Box(GUIContent.none, EditorParameters.k_TimelineBlockLeft, GUILayout.Width(halfWidth), GUILayout.Height(k_ElementHeight));
               GUILayout.Box(GUIContent.none, EditorParameters.k_TimelineBlockRight, GUILayout.Width(halfWidth), GUILayout.Height(k_ElementHeight));
            }
         }

      }

      public class SkillAnimationGroup : Group
      {

         public SkillAnimationGroup()
         {
            title = "动画";
         }

         public override int OnGroupHierarchyGUI(bool isGroupSelected, int selectedItemIndex)
         {
            if (!foldout)
            {
               return -1;
            }

            int newSelectedItemIndex = -1;   // 新选中的Item索引
            SkillAnimationAction action;
            for (int i = 0; i < this.actions.Count; ++i)
            {
               action = this.actions[i] as SkillAnimationAction;

               bool isSelected = isGroupSelected && selectedItemIndex==i;
               GUIStyle backgroundStyle = isSelected ? EditorParameters.k_BackgroundSelected : EditorParameters.k_BackgroundEven;
               using (new GUILayoutHorizontal(backgroundStyle, GUILayout.Height(k_ElementHeight)))
               {
                  GUILayout.Label($"[{action.timelineData.start}~{action.timelineData.end}]", EditorParameters.k_Label);   // 注意GUIStyle会影响高度
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

         public override int OnGroupTimelineGUI(bool isGroupSelected, int selectedItemIndex)
         {
            if (!foldout)
            {
               return -1;
            }

            int newSelectedItemIndex = -1;   // 新选中的Item索引
            GUILayout.Space(k_ElementHeight);
            SkillAnimationAction action;
            for (int i = 0; i < this.actions.Count; ++i)
            {
               action = this.actions[i] as SkillAnimationAction;
               
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

         public override void OnInspectorGUI(int selectedItemIndex, int maxFrameLength)
         {
            base.OnInspectorGUI(selectedItemIndex, maxFrameLength);
         }
         
      }
   }
   

}

