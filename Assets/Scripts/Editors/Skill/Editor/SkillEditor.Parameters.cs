using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skill.Editor
{
    public partial class SkillEditor
    {
        // 技能编辑器状态
        public enum SkillEditorState
        {
            // 编辑状态 （根据CurrentFrame来驱动动画）
            Edit,
            // 预览状态 (直接播放动画来驱动CurrentFrame更新)
            Preview,
        }
        // SkillAction类型列表 (与enum SkillActionType对应)
        private static readonly string[] k_SkillActionTypes =
        {
            "动画",
            "特效",
            "事件",
        };

        // Animator参数类型列表 (与enum AnimatorParameterType对应)
        private static readonly string[] k_SkillAnimationStates = 
        {
            "Attack1",
            "Attack2",
            "Skill",
        };
        
        // 人物列表
        private string[] _CharacterIDs;
        // 技能列表
        private string[] _SkillIDs;
        
        // 节点列表面板宽度 (左)
        private const int k_HierarchyPanelWidth = 250;    
        // 详细属性面板宽度 (右)
        private const int k_InspectorPanelWidth = 250;
        // Timeline面板宽度 (中)
        private int k_TimelinePanelWidth = 0;
        // Timeline数据元素高度
        private const int k_ElementHeight = 20;
        // 滚动层偏移
        private Vector2 _HierarchyScrollPosition;
        private Vector2 _InspectorScrollPosition;
        // 当前是否正在播放
        private bool _IsPlaying = false;
        // 当前播放到第几帧
        private int _CurrentFrame = 0;
        // 当前预览状态
        private SkillEditorState _EditorState = SkillEditorState.Edit;
        
        
        // 选择人物ID
        private string _SelectedCharacterID = string.Empty;
        // 选择技能文件
        private string _SelectedSkillID = string.Empty;
        
        // 人物guid
        private string _MainCharacterResourceGuid;
        private GameObject _MainCharacterResource;
        // 人物GameObject
        private GameObject _MainCharacter;
        // 人物Animator组件
        private Animator _Animator;
        
        
        // Skill Config
        private SkillConfig _SkillConfig;
        // Group列表
        List<Group> _Groups = new List<Group>();
        // 选中Group的index
        private int _SelectedGroupIndex = -1;
        // 选中Item的index
        private int _SelectedItemIndex = -1;

    }

}

