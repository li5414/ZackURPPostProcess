using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Plastic.Antlr3.Runtime.Misc;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;
using Zack.Editor;

namespace AnimationEventEditor
{
   public partial class AnimationEventEditor
   {
      // ==========================绘制动画 Group===============================
      // 绘制动画Hierarchy面板
      void OnGroupAnimationHierarchyGUI(int totalFrames)
      {
         using (new GUILayoutHorizontal(GUILayout.Height(k_ElementHeight)))
         {
            EditorUtils.CreateToggle(true, "动画", EditorParameters.k_Foldout);
            GUILayout.FlexibleSpace();
         }

         GUIStyle backgroundStyle = EditorParameters.k_BackgroundEven;
         using (new GUILayoutHorizontal(backgroundStyle, GUILayout.Height(k_ElementHeight)))
         {
            GUILayout.Label($"[0~{totalFrames}]", EditorParameters.k_Label);  // 注意GUIStyle会影响高度
         }
      }
      
      // 绘制动画Timelin面板
      void OnGroupAnimationTimelineGUI(int totalFrames)
      {
         GUILayout.Space(k_ElementHeight);
         GUIStyle backgroundStyle = EditorParameters.k_BackgroundEven;
         using (new GUILayoutHorizontal(backgroundStyle, GUILayout.Height(k_ElementHeight)))
         {
            drawTimelineData(0, totalFrames);
         }
      }
      // ==========================绘制动画 Group===============================

      // ==========================绘制动画事件 Group===============================
      // 绘制动画事件Hierarchy面板
      int OnGroupAnimationEventHierarchyGUI(AnimationClip clip, AnimationEvent[] events, int selectedItemIndex)
      {
         int newSelectedItemIndex = -1;   // 新选中的Item索引

         using (new GUILayoutHorizontal(GUILayout.Height(k_ElementHeight)))
         {
            EditorUtils.CreateToggle(true, "自定义回调", EditorParameters.k_Foldout);
            GUILayout.FlexibleSpace();
         }

         for (int i = 0; i < events.Length; ++i)
         {
            AnimationEvent evt = events[i];
            bool isSelected = selectedItemIndex==i;
            GUIStyle backgroundStyle = isSelected ? EditorParameters.k_BackgroundSelected : EditorParameters.k_BackgroundEven;
            using (new GUILayoutHorizontal(backgroundStyle, GUILayout.Height(k_ElementHeight)))
            {
               GUILayout.Label($"[{convertTime2Frame(clip, evt.time)}]", EditorParameters.k_Label);   // 注意GUIStyle会影响高度
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
                  AnimationEventEditorUtils.RemoveAnimationEvents(clip, events, newSelectedItemIndex);
                  UpdateAnimationInfo();
               });
            }
         }
         return newSelectedItemIndex;
      }

      // 绘制动画事件Timeline面板
      int OnGroupAnimationEventTimelineGUI(AnimationClip clip, AnimationEvent[] events, int selectedItemIndex)
      {
         int newSelectedItemIndex = -1;   // 新选中的Item索引

         GUILayout.Space(k_ElementHeight);
         for (int i = 0; i < events.Length; ++i)
         {
            AnimationEvent evt = events[i];
            bool isSelected = selectedItemIndex==i;
            GUIStyle backgroundStyle = isSelected ? EditorParameters.k_BackgroundSelected : EditorParameters.k_BackgroundEven;
            using (new GUILayoutHorizontal(backgroundStyle, GUILayout.Height(k_ElementHeight)))
            {
               int frame = convertTime2Frame(clip, evt.time);
               drawTimelineData(frame, frame);
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
                  AnimationEventEditorUtils.RemoveAnimationEvents(clip, events, newSelectedItemIndex);
                  UpdateAnimationInfo();
               });
            }
         }
         return newSelectedItemIndex;
      }
      
      // 绘制动画事件Inspector面板
      void OnGroupAnimationEventInspectorGUI(AnimationClip clip, int totalFrames, AnimationEvent[] events, AnimationEvent evt)
      {
         if (evt != null)
         {
            using (new GUIChangeCheck(() => { AnimationEventEditorUtils.SetAnimationEvents(clip, events); }))
            {
               int frame = convertTime2Frame(clip, evt.time);
               int curFrame = EditorUtils.CreateIntField("关键帧", frame, 0, totalFrames - 1);
               if (frame != curFrame)
               {
                  evt.time = curFrame / clip.frameRate;
               }

               evt.functionName = EditorUtils.CreateTextField("functionName", evt.functionName);
               evt.stringParameter = EditorUtils.CreateTextField("stringParameter", evt.stringParameter);
               evt.intParameter = EditorUtils.CreateIntField("intParameter", evt.intParameter);
               evt.floatParameter = EditorUtils.CreateFloatField("floatParameter", evt.floatParameter);
            }
         }
      }
      // ==========================绘制动画事件 Group===============================


      protected void drawTimelineData(int begin, int end)
      {
         int length = end - begin;
         Rect rect = EditorUtils.CalculateTimeRect(begin, end, k_ElementHeight);
            
         float halfWidth = rect.width / 2;
         if (length == 0) { halfWidth = 3; }   // 事件帧
         GUILayout.Space(rect.x);
//            using (new GUIColor(length > 0 ? GUI.color : Color.red))
         {
            GUILayout.Box(GUIContent.none, EditorParameters.k_TimelineBlockLeft, GUILayout.Width(halfWidth));
            GUILayout.Box(GUIContent.none, EditorParameters.k_TimelineBlockRight, GUILayout.Width(halfWidth));
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
      
   }
   

}

