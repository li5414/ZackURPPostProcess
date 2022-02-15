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
        
        // 当前是否正在播放
        private bool _IsPlaying = false;
        // 当前播放到第几帧
        private int _CurrentFrame = 0;
        
        // 总时长
        private int _MaxFrameLength = 200;
        
        // Group列表
        List<Group> _Groups = new List<Group>();
        // 选中Group的index
        private int _SelectedGroupIndex = -1;
        // 选中Item的index
        private int _SelectedItemIndex = -1;
    }

}

