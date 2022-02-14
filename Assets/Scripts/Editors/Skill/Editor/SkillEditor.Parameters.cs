using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Zack.Editor;

namespace Zack.Editor.Skill
{
    public partial class SkillEditor
    {
        //  节点列表面板宽度 (左)
        private const int k_HierarchyPanelWidth = 250;    
        // 详细属性面板宽度 (右)
        private const int k_InspectorPanelWidth = 250;
        // Timeline面板宽度 (中)
        private int k_TimelinePanelWidth = 0;
        // Timeline数据元素高度
        private const int k_ElementHeight = 20;
        
        // 是否点击第一帧
        private bool _IsClickFirstFrame = false;
        // 是否点击上一帧
        private bool _IsClickPreviousFrame = false;
        // 当前是否正在播放
        private bool _IsPlaying = false;
        // 是否点击下一帧
        private bool _IsClickNextFrame = false;
        // 是否点击最后一帧
        private bool _IsClickLastFrame = false;
        // 当前播放到第几帧
        private int _CurrentFrame = 99;
        
    }

}

