using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnimationEventEditor
{
    public partial class AnimationEventEditor
    {
        // 人物列表
        private string[] _CharacterIDs;
        
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
        // 当前是否正在播放 (用于编辑器模式控制查看动画)
        private bool _IsPlaying = false;
        // 是否循环播放
        private bool _IsLoop = true;
        // 当前播放到第几帧
        private int _CurrentFrame = 0;
        // 当前动画计时器
        private float _AnimationTimer = 0;
        
        // 选择人物ID
        private string _SelectedCharacterID = string.Empty;
        // 选择动画
        private AEEditorAnimatorState _SelectedState = AEEditorAnimatorState.Attack1;
        
        // 人物guid
        private string _MainCharacterResourceGuid;
        private GameObject _MainCharacterResource;
        // 人物GameObject
        private GameObject _MainCharacter;
        // 人物Animator组件
        private Animator _Animator;
        // 当前播放动画的layer
        private int _AnimatorLayer = 0;
        // 当前选择的动画文件
        private AnimationClip _AnimationClip = null;
        // 
        private bool _ApplyRootMotion = false;
        // 当前动画总帧数
        private int _TotalFrames = 0;
        // 当前所有动画事件
        private AnimationEvent[] _AnimationEvents = null;
        
        // 选中Item的index
        private int _SelectedItemIndex = -1;

    }

}

