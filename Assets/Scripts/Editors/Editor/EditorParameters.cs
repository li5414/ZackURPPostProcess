using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Zack.Editor
{
    /// <summary>
    /// 编辑器参数
    /// </summary>
    public class EditorParameters
    {
        // Toolbar高度
        public const int k_ToolbarHeight = 18;
        // Timeline刻度尺每点间隔（包含小点）
        public const float k_TickGap = 10;
        // Timeline刻度尺小刻度数量
        public const int k_TickUnit = 5;
        // Timeline刻度尺水平偏移
        public const float k_RulerOffsetX = 10;
        
        
        // GUIContent
        // 播放按钮
        public static readonly GUIContent k_PlayFramesContent = EditorGUIUtility.IconContent("Animation.Play", "Play");
        // 上一帧
        public static readonly GUIContent k_PreviousFrameContent = EditorGUIUtility.IconContent("Animation.PrevKey", "Previous Key Frame");
        // 下一帧
        public static readonly GUIContent k_NextFrameContent = EditorGUIUtility.IconContent("Animation.NextKey", "Next Key Frame");
        // 第一帧
        public static readonly GUIContent k_FirstFrameContent = EditorGUIUtility.IconContent("Animation.FirstKey", "First Key Frame");
        // 最后一帧
        public static readonly GUIContent k_LastFrameContent = EditorGUIUtility.IconContent("Animation.LastKey", "Last Key Frame");
        // 循环
        public static readonly GUIContent k_LoopFrameContent = EditorGUIUtility.IconContent("preAudioLoopOff", "Loop");
        
        // GUIStyle
        public static readonly GUIStyle k_Label = new GUIStyle("label");
        public static readonly GUIStyle k_TimelineRuler = new GUIStyle("MeTimeLabel");
        public static readonly GUIStyle k_WindowBackground = new GUIStyle("OL box NoExpand");
        public static readonly GUIStyle k_BackgroundSelected = new GUIStyle("OL SelectedRow");
        public static readonly GUIStyle k_BackgroundEven = new GUIStyle("OL EntryBackEven");
        public static readonly GUIStyle k_TimelineBlockLeft = new GUIStyle("MeTransOnLeft");
        public static readonly GUIStyle k_TimelineBlockRight = new GUIStyle("MeTransOnRight");
        public static readonly GUIStyle k_TimelineFrameTag = new GUIStyle("Grad Down Swatch");
        public static readonly GUIStyle k_Foldout = new GUIStyle("foldout");

        static EditorParameters()
        {
            // k_Label
            k_Label.alignment = TextAnchor.LowerCenter;
            k_Label.stretchHeight = true;
            k_Label.margin.top = 0;
            k_Label.margin.bottom = 0;
            // k_BackgroundSelected
            k_BackgroundSelected.fixedHeight = 0;
            k_BackgroundSelected.stretchHeight = true;
            // k_BackgroundEven
            k_BackgroundEven.fixedHeight = 0;
            k_BackgroundEven.stretchHeight = true;
            // k_TimelineBlockLeft
            k_TimelineBlockLeft.fixedHeight = 0;
            k_TimelineBlockLeft.fixedWidth = 0;
            k_TimelineBlockLeft.stretchWidth = true;
            k_TimelineBlockLeft.stretchHeight = true;
            // k_TimelineBlockRight
            k_TimelineBlockRight.fixedHeight = 0;
            k_TimelineBlockRight.stretchHeight = true;
            
            RectOffset offset = new RectOffset(0,0,0,0);
            // k_WindowBackground
            k_WindowBackground.margin = offset;
            k_WindowBackground.padding = offset;
            k_WindowBackground.border = offset;
            k_WindowBackground.padding.top = 1;
            // k_BackgroundSelected 
            k_BackgroundSelected.margin = offset;
            k_BackgroundSelected.padding = offset;
            k_BackgroundSelected.border = offset;
            // k_BackgroundEven
            k_BackgroundEven.margin = offset;
            k_BackgroundEven.padding = offset;
            k_BackgroundEven.border = offset;

            k_Foldout.margin.top = 0;
            k_Foldout.margin.bottom = 0;
            k_Foldout.margin.left -= 10;
            k_Foldout.margin.right += 2;
            k_Foldout.fixedHeight = 20;
        }
    }

}

