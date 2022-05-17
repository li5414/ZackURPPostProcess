using System.Collections;
using System.Collections.Generic;
using System.IO;
using Codice.Client.Commands;
using UnityEditor;
using UnityEngine;
using Zack.Editor;

namespace AnimationEventEditor
{
    public partial class AnimationEventEditor : EditorWindow
    {
        [MenuItem("Editor/动画事件编辑", isValidateFunction:false)]
        static void OpenSkillEditor()
        {
            if (EditorApplication.isCompiling || EditorUtility.scriptCompilationFailed)
            {
                EditorUtility.DisplayDialog("Error", "Compile Error", "Ok");
                return;
            }

            EditorWindow.GetWindow<AnimationEventEditor>().Close();
            EditorWindow.GetWindow<AnimationEventEditor>().Show();
        }
        
        void Awake()
        {
            EditorParameters.k_Foldout.fixedHeight = k_ElementHeight;
            this.minSize = new Vector2(k_HierarchyPanelWidth + k_InspectorPanelWidth + 500, 500);
            this.title = "动画事件编辑";

            // 遍历角色
            this._CharacterIDs = browserCharacters();

            if (!EditorApplication.isPlaying)
            {
                // 创建新场景
                createNewScene();
            }
        }

        public void OnDestroy()
        {
            EditorApplication.ExitPlaymode();
        }

        void OnGUI()
        {
            this.k_TimelinePanelWidth = (int)position.width - k_HierarchyPanelWidth - k_InspectorPanelWidth;

            // 工具栏
            drawTopToolbar();
            // timeline工具栏
            drawHierarchyToolbar();
            // timeline数据
            drawHierarchy();      
       
        }

        void Update()
        {
            if (_IsPlaying && this._AnimationClip!=null)
            {
                this._AnimationTimer += Time.deltaTime;
                float frameDuration = 1f / this._AnimationClip.frameRate;
                if (this._AnimationTimer >= frameDuration)
                {
                    this._AnimationTimer -= frameDuration;
                    
                    ++this._CurrentFrame;
                    if (this._CurrentFrame >= this._TotalFrames)
                    {
                        if (this._IsLoop)
                        {
                            this._CurrentFrame = 0;
                        }
                        else
                        {
                            this._CurrentFrame = this._TotalFrames - 1;
                            this._IsPlaying = false;
                        }
                    }
                }
                
                this.Repaint();
            }
            
            updateAnimation();
        }

        void drawTopToolbar()
        {
            using (new GUILayoutHorizontal(EditorStyles.toolbar))
            {
                using (new GUIChangeCheck(() => { Debug.Log("==change=="); }))
                {
                    // 选择状态
                    EditorUtils.CreateText("角色:", EditorParameters.k_Label, GUILayout.Width(40));
                    EditorUtils.CreateButton(this._SelectedCharacterID, EditorParameters.k_DropDownButton, () =>
                    {
                        int seletedIndex = getSelectedIndex(this._SelectedCharacterID, this._CharacterIDs);
                        EditorUtils.CreateMenu(this._CharacterIDs, seletedIndex, (sindex) =>
                        {
                            this._SelectedCharacterID = this._CharacterIDs[sindex];
                            
                            // 加载角色
                            LoadMainCharacter(Path.Combine(Skill.Parameters.k_CharacterPrefabAssetPath, $"{this._SelectedCharacterID}/{this._SelectedCharacterID}.prefab"));
                        });
                    }, GUILayout.Width(83));
                    
                    EditorUtils.CreateText("状态基:", EditorParameters.k_Label, GUILayout.Width(40));
                    EditorUtils.CreateButton(this._SelectedState, EditorParameters.k_DropDownButton, () =>
                    {
                        int seletedIndex = getSelectedIndex(this._SelectedState, this._StateNames);
                        EditorUtils.CreateMenu(this._ShowStateNames, seletedIndex, (index) =>
                        {
                            this._SelectedState = this._StateNames[index];
                            this._AnimatorLayer = this._StateLayers[index];
                            UpdateAnimationInfo();
                        });
                    }, GUILayout.Width(83));

                }
                

            }
        }

        void drawHierarchyToolbar()
        {
            using (new GUILayoutHorizontal(EditorStyles.toolbar, GUILayout.Height(EditorParameters.k_ToolbarHeight)))
            {
                EditorUtils.CreateButton("添加事件", EditorStyles.toolbarDropDown, () =>
                {
                    EditorUtils.CreateMenu<AEEditorActionType>(-1, (index) =>
                    {
                        addAnimationEventAction((AEEditorActionType)index);
                    });
                }, GUILayout.Width(150));
            }
            
            // position中存储的是当前窗口的位置和大小
            using (new GUILayoutHorizontal(EditorStyles.toolbar))
            {
                // 按钮
                {
                    // 编辑模式
                    EditorUtils.CreateButton(EditorParameters.k_FirstFrameContent, EditorStyles.toolbarButton, () =>
                    {
                        selectFrame(0);
                    });
                    EditorUtils.CreateButton(EditorParameters.k_PreviousFrameContent, EditorStyles.toolbarButton, () =>
                    {
                        selectFrame(this._CurrentFrame - 1);
                    });
                    EditorUtils.CreateButton(EditorParameters.k_PlayFramesContent, EditorStyles.toolbarButton, () =>
                    {
                        if (this._CurrentFrame > this._TotalFrames)
                        {
                            this.selectFrame(0);
                        }
                        this._IsPlaying = true;
                        
//                        PreviewSkill();
                    });
                    EditorUtils.CreateButton(EditorParameters.k_NextFrameContent, EditorStyles.toolbarButton, () =>
                    {
                        selectFrame(this._CurrentFrame + 1);
                    });
                    EditorUtils.CreateButton(EditorParameters.k_LastFrameContent, EditorStyles.toolbarButton, () =>
                    {
                        selectFrame(this._TotalFrames);
                    });
                }
                

                // 当前帧数
                EditorUtils.CreateLabel("当前");
                EditorUtils.CreateText(_CurrentFrame.ToString(), EditorStyles.textField, GUILayout.Width(40));
              
                
                // timeline刻度尺
                using (new GUILayoutArea(new Rect(k_HierarchyPanelWidth, EditorParameters.k_ToolbarHeight*2+5, k_TimelinePanelWidth, EditorParameters.k_ToolbarHeight)))
                {
                    Rect rulerRect = new Rect(0, 0, k_TimelinePanelWidth, EditorParameters.k_ToolbarHeight);
                    drawTimelineRuler(rulerRect);
                }

                // 总时长
                using (new GUILayoutArea(new Rect(k_HierarchyPanelWidth+k_TimelinePanelWidth+10, EditorParameters.k_ToolbarHeight*2+5, k_InspectorPanelWidth, EditorParameters.k_ToolbarHeight)))
                {
                    EditorUtils.CreateIntFieldDisable("总时长", this._TotalFrames);  
                }
                
                
                GUILayout.FlexibleSpace();
            }
        }

        /// <summary>
        /// 数据编辑
        /// </summary>
        void drawHierarchy()
        {
            if (!this._AnimationClip)
            {
                return;
            }
            
            int newSelectedItemIndex = -1;    // 新选中的Item索引
            
            using (new GUILayoutHorizontal(EditorParameters.k_WindowBackground))
            {
                // hierarchy面板
                using (new GUILayoutVertical(GUILayout.Width(k_HierarchyPanelWidth)))
                {
                    using (new GUILayoutScrollView(_HierarchyScrollPosition))
                    {
                        // 动画
                        OnGroupAnimationHierarchyGUI(this._TotalFrames);
                        
                        // 自定义事件列表
                        newSelectedItemIndex = OnGroupAnimationEventHierarchyGUI(this._AnimationClip,
                            this._AnimationEvents, this._SelectedItemIndex);
                        if (newSelectedItemIndex >= 0)
                        {
                            this._SelectedItemIndex = newSelectedItemIndex;
                        }

                        if (this._HierarchyScrollPosition.y > 0 && Event.current.type != EventType.Repaint)
                        {
                            GUILayout.Space(EditorParameters.k_ToolbarHeight);
                        }
                    }
                }

                // timeline面板
                using (new GUILayoutVertical(GUILayout.Width(k_TimelinePanelWidth)))
                {
                    using (new GUILayoutVertical())
                    {
                        using (new GUILayoutScrollView(ref _HierarchyScrollPosition))
                        {
                            // 动画
                            OnGroupAnimationTimelineGUI(this._TotalFrames);
                            
                            // 自定义事件列表
                            newSelectedItemIndex = OnGroupAnimationEventTimelineGUI(this._AnimationClip,
                                this._AnimationEvents, this._SelectedItemIndex);
                            if (newSelectedItemIndex >= 0)
                            {
                                this._SelectedItemIndex = newSelectedItemIndex;
                            }
                        }
                    }
                }
                // 绘制timeline线
                drawFrameLine();
                
                // inspector面板
                using (new GUILayoutVertical(EditorParameters.k_FrameBackground, GUILayout.Width(k_InspectorPanelWidth)))
                {
                    using (new GUILayoutScrollView(ref this._InspectorScrollPosition))
                    {
                        drawInspector();
                    }
                }

            }
            
        }

        void drawInspector()
        {
            EditorUtils.CreateText("属性面板", EditorParameters.k_BoldLabel);
            if (this._AnimationEvents!=null && this._AnimationEvents.Length>0 && this._SelectedItemIndex!=-1)
            {
                OnGroupAnimationEventInspectorGUI(this._AnimationClip, this._TotalFrames, this._AnimationEvents, this._AnimationEvents[this._SelectedItemIndex]);
            }
            else
            {
                OnInspectorGUI();
            }
        }

        void OnInspectorGUI()
        {
            using (new GUILayoutVertical(EditorParameters.k_WindowBackground, GUILayout.Height(k_ElementHeight)))
            {
               
            }
        }

        /// <summary>
        /// 绘制当前帧的竖线
        /// </summary>
        void drawFrameLine()
        {
            float x = k_HierarchyPanelWidth + EditorUtils.CalculateFrameToPosition(this._CurrentFrame) - this._HierarchyScrollPosition.x;

            if (x<k_HierarchyPanelWidth || x>k_HierarchyPanelWidth + k_TimelinePanelWidth)
            {
                return;
            }

            using (new GUILayoutArea(new Rect(x-5, EditorParameters.k_ToolbarHeight*2+5, 10, 20)))
            {
                GUILayout.Box(GUIContent.none, EditorParameters.k_TimelineFrameTag);
            }
            
            Vector2 p1 = new Vector2(x, EditorParameters.k_ToolbarHeight*3-10);
            Vector2 p2 = new Vector2(x, position.width);

            using (new GUIHandlesColor(Color.white))
            {
                Handles.DrawLine(p1, p2);
            }
        }
        

        /// <summary>
        /// 绘制Timeline刻度尺
        /// </summary>
        /// <param name="rect"></param>
        void drawTimelineRuler(Rect rect)
        {
            float width = rect.width;
            float height = rect.height;
            
            using (new GUILayoutHorizontal(EditorStyles.toolbar, GUILayout.Width(width), GUILayout.Height(height)))
            {
//                GUILayout.FlexibleSpace();

                float count = (width + _HierarchyScrollPosition.x) / EditorParameters.k_TickGap;
                count -= count % EditorParameters.k_TickUnit;
                float y1 = height * 0.5f;
                float y2 = height * 0.9f;
                float y3 = height;
                
                Color c1 = Color.white;
//                Color c2 = new Color(c1.r, c1.g, c1.b, c1.a*0.5f);
                Rect labelRect = new Rect(Vector2.zero, Vector2.one);
                GUIContent content = new GUIContent();
                float xOffset = rect.xMin - _HierarchyScrollPosition.x;
                using (new GUIHandlesColor(c1))
                {
                    for (int i = 0; i <= count; ++i)
                    {
                        float x = xOffset + EditorParameters.k_RulerOffsetX + i * EditorParameters.k_TickGap;
                        if (x < rect.xMin + EditorParameters.k_RulerOffsetX)
                            continue;

                        Vector2 p1;
                        if (i % EditorParameters.k_TickUnit == 0)
                        {
                            // 大刻度
                            p1 = new Vector2(x, y1);
//                            Handles.color = c1;
                            // 刻度值
                            content.text = string.Format("{0}", i);
                            labelRect.position = new Vector2(p1.x + 2, 0);
                            labelRect.size = EditorParameters.k_TimelineRuler.CalcSize(content);
                            GUI.Label(labelRect, content, EditorParameters.k_TimelineRuler);
                        }
                        else
                        {
                            // 小刻度
                            p1 = new Vector2(x, y2);
//                            Handles.color = c2;
                        }
                        var p2 = new Vector2(x, y3);
                        // 绘制刻度
                        Handles.DrawLine(p1, p2);
                    }
                }
            }
            
            Event e = Event.current;
            if (e.button == 0 && e.type == EventType.MouseUp && rect.Contains(e.mousePosition))
            {
                GUI.FocusControl(null);
                e.Use();
                    
                int frame = EditorUtils.CalculatePositionToFrame(e.mousePosition.x+this._HierarchyScrollPosition.x);
                selectFrame(frame);
            }

        }

        void selectFrame(int frame)
        {

            if (frame < 0)
            {
                frame = 0;
            }

            if (frame > this._TotalFrames)
            {
                frame = this._TotalFrames;
            }
            
            this._IsPlaying = false;
            this._CurrentFrame = frame;
            
        }
        
       
        
    
    }

}

