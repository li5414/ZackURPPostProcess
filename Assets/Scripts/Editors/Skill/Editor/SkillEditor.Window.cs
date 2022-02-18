using System.Collections;
using System.Collections.Generic;
using System.IO;
using Codice.Client.Commands;
using UnityEditor;
using UnityEngine;
using Zack.Editor;

namespace Skill.Editor
{
    public partial class SkillEditor : EditorWindow
    {
        [MenuItem("Editor/SkillEditor", isValidateFunction:false)]
        static void OpenSkillEditor()
        {
            if (EditorApplication.isCompiling || EditorUtility.scriptCompilationFailed)
            {
                EditorUtility.DisplayDialog("Error", "Compile Error", "Ok");
                return;
            }
            
//            EditorWindow.GetWindow<SkillEditor>().Start();
            EditorWindow.GetWindow<SkillEditor>().Show();
        }

        void Awake()
        {
            EditorParameters.k_Foldout.fixedHeight = k_ElementHeight;
            this.minSize = new Vector2(k_HierarchyPanelWidth + k_InspectorPanelWidth + 500, 500);

            // 遍历角色
            this._CharacterIDs = browserCharacters();
            
            // 创建新场景
            createNewScene();
        }


        void OnGUI()
        {
            this.k_TimelinePanelWidth = (int)position.width - k_HierarchyPanelWidth - k_InspectorPanelWidth;

            // 工具栏
            drawTopToolbar();
            if (this._SkillConfig != null)
            {
                // timeline工具栏
                drawHierarchyToolbar();
                // timeline数据
                drawHierarchy();      
            }

        }

        void Update()
        {
//            Debug.Log("========" + Time.deltaTime);
        }

        private string[] menus = {"1", "2", "zack", "tom", "haha", "czxv"};
        private int aaa = 0;
        private string tmpPath = string.Empty;
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
                        EditorUtils.CreateMenu(this._CharacterIDs, -1, (sindex) =>
                        {
                            this._SelectedCharacterID = this._CharacterIDs[sindex];
                            
                            // 加载角色
                            loadMainCharacter(Path.Combine(Parameters.k_CharacterPrefabAssetPath, $"{this._SelectedCharacterID}/{this._SelectedCharacterID}.prefab"));
                            // 更新技能列表
                            this._SkillIDs = browserSkills();
                        });
                    }, GUILayout.Width(83));
                    
                    EditorUtils.CreateText("技能:", EditorParameters.k_Label, GUILayout.Width(40));
                    EditorUtils.CreateButton(this._SelectedSkillID, EditorParameters.k_DropDownButton, () =>
                    {
                        EditorUtils.CreateMenu(this._SkillIDs, -1, (sindex) =>
                        {
                            this._SelectedSkillID = this._SkillIDs[sindex];
                            // 读取技能
                            ReadConfig(Path.Combine(Parameters.k_SkillConfigFilePath, $"{this._SelectedCharacterID}/{this._SelectedSkillID}"));    // TODO
                        });
                    }, GUILayout.Width(83));
                
                    EditorUtils.CreateButton("添加", EditorStyles.toolbarDropDown, () =>
                    {
                        EditorUtils.CreateMenu(k_SkillActionTypes, -1, (index) =>
                        {
                            AddSkillAction((SkillActionType)index);
                        });
                    }, GUILayout.Width(150));
                    
                    EditorUtils.CreateButton("保存", EditorStyles.toolbarButton, () =>
                    {
                        if (this._SkillConfig != null)
                        {
                            SaveConfig(Path.Combine(Parameters.k_SkillConfigFilePath, $"{this._SelectedCharacterID}/{this._SelectedSkillID}"));
                        }
                    }, GUILayout.Width(100));
                }
                

            }
        }

        void drawHierarchyToolbar()
        {
            using (new GUILayoutHorizontal(EditorStyles.toolbar, GUILayout.Height(EditorParameters.k_ToolbarHeight)))
            {
                using (new GUIChangeCheck(() =>
                {
                    UpdateAnimationActions();
                }))
                {
                    // 选择状态
                    EditorUtils.CreateText("选择动画:", EditorParameters.k_Label, GUILayout.Width(50));
                    EditorUtils.CreateButton(k_SkillAnimationStates[(int)this._SkillConfig.animatorState], EditorParameters.k_DropDownButton, () =>
                    {
                        EditorUtils.CreateMenu(k_SkillAnimationStates, -1, (sindex) =>
                        {
                            this._SkillConfig.animatorState = (SkillAnimatorState) sindex;
                            UpdateAnimationActions();
                        });
                    }, GUILayout.Width(83));
                }
            }
            
            // position中存储的是当前窗口的位置和大小
            using (new GUILayoutHorizontal(EditorStyles.toolbar))
            {
                // 按钮
                EditorUtils.CreateButton(EditorParameters.k_FirstFrameContent, EditorStyles.toolbarButton, () =>
                {
                    selectFrame(0);
                });
                EditorUtils.CreateButton(EditorParameters.k_PreviousFrameContent, EditorStyles.toolbarButton, () =>
                {
                    selectFrame(this._CurrentFrame - 1);
                });
                EditorUtils.CreateToggle(ref _IsPlaying, EditorParameters.k_PlayFramesContent, EditorStyles.toolbarButton);
                EditorUtils.CreateButton(EditorParameters.k_NextFrameContent, EditorStyles.toolbarButton, () =>
                {
                    selectFrame(this._CurrentFrame + 1);
                });
                EditorUtils.CreateButton(EditorParameters.k_LastFrameContent, EditorStyles.toolbarButton, () =>
                {
                    selectFrame(this._SkillConfig.totalFrames);
                });

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
                    EditorUtils.CreateIntField("总时长", this._SkillConfig.totalFrames);  
                }
                
                
                GUILayout.FlexibleSpace();
            }
        }

        /// <summary>
        /// 数据编辑
        /// </summary>
        void drawHierarchy()
        {
            int newSelectedItemIndex = -1;    // 新选中的Item索引
            
            using (new GUILayoutHorizontal(EditorParameters.k_WindowBackground))
            {
                // hierarchy面板
                using (new GUILayoutVertical(GUILayout.Width(k_HierarchyPanelWidth)))
                {
                    using (new GUILayoutScrollView(_HierarchyScrollPosition))
                    {
                        Group group;
                        for (int i=0; i<this._Groups.Count; ++i)
                        {
                            group = this._Groups[i];
                            group.OnGroupGUI();
                            newSelectedItemIndex = group.OnGroupHierarchyGUI(this._SelectedGroupIndex==i, this._SelectedItemIndex);
                            if (newSelectedItemIndex >= 0)
                            {
                                this._SelectedGroupIndex = i;
                                this._SelectedItemIndex = newSelectedItemIndex;
                            }
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
                            Group group;
                            for (int i=0; i<this._Groups.Count; ++i)
                            {
                                group = this._Groups[i];
                                newSelectedItemIndex = group.OnGroupTimelineGUI(this._SelectedGroupIndex==i, this._SelectedItemIndex);
                                if (newSelectedItemIndex >= 0)
                                {
                                    this._SelectedGroupIndex = i;
                                    this._SelectedItemIndex = newSelectedItemIndex;
                                }
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
            if (this._SelectedGroupIndex != -1 && this._SelectedItemIndex != -1 && this._Groups.Count>this._SelectedGroupIndex)
            {
                this._Groups[this._SelectedGroupIndex].OnInspectorGUI(this._SelectedItemIndex, this._SkillConfig.totalFrames);
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

            if (frame > this._SkillConfig.totalFrames)
            {
                frame = this._SkillConfig.totalFrames;
            }
            
            Debug.Log("========frame=======" + frame);
            this._IsPlaying = false;
            this._CurrentFrame = frame;
            
        }
        
       
        
    
    }

}

