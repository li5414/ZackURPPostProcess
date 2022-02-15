using System.Collections;
using System.Collections.Generic;
using Codice.Client.Commands;
using UnityEditor;
using UnityEngine;
using Zack.Editor;

namespace Zack.Editor.Skill
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
            
            EditorWindow.GetWindow<SkillEditor>().Start();
            EditorWindow.GetWindow<SkillEditor>().Show();
        }

        void Start()
        {
            EditorParameters.k_Foldout.fixedHeight = k_ElementHeight;

            // TODO
            var group = new SkillAnimationGroup();
            group.actions.Add(new SkillAnimationAction(Random.Range(0, 200), Random.Range(1, 20)));
            group.actions.Add(new SkillAnimationAction(Random.Range(0, 200), Random.Range(1, 20)));
            group.actions.Add(new SkillAnimationAction(Random.Range(0, 200), Random.Range(1, 20)));
            this._Groups.Add(group);
            this._Groups.Add(group);
            this._Groups.Add(group);
        }

        void SaveConfig()
        {
            SkillConfig config = new SkillConfig();;
            config.animations = new List<SkillAnimationAction>();
            SkillAnimationGroup group = this._Groups[0] as SkillAnimationGroup;
            for (int i = 0; i < group.actions.Count; ++i)
            {
                config.animations.Add(group.actions[i] as SkillAnimationAction);
            }
            
            JsonUtils.SerializeObjectToFile(config, "D://xxx.json");
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

        private string[] menus = {"1", "2", "zack", "tom", "haha", "czxv"};
        private int aaa = 0;
        private string tmpPath = string.Empty;
        void drawTopToolbar()
        {
            using (new GUILayoutHorizontal(EditorStyles.toolbar))
            {
                using (new GUIChangeCheck(() => { Debug.Log("==change=="); }))
                {
                    EditorUtils.CreateLabel("路径:", GUILayout.Width(40));
                    EditorUtils.CreateText(ref tmpPath, EditorStyles.textField, true);
                
                    EditorUtils.CreateButton("打开", EditorStyles.toolbarButton, () =>
                    {
//                        Debug.Log("=====打开====="+tmpPath);
                        SaveConfig();
                    }, GUILayout.Width(100));
                
                
                    EditorUtils.CreateButton(menus[aaa], EditorStyles.toolbarDropDown, () =>
                    {
                        Debug.Log("=====菜单=====");

                        EditorUtils.CreateMenu(menus, aaa, (index) =>
                        {
                            Debug.Log(menus[index]);
                            aaa = index;
                        });
                    }, GUILayout.Width(150));
                }
                

            }
        }

        // 滚动层偏移
        private Vector2 _ScrollPosition;
        void drawHierarchyToolbar()
        {
            // position中存储的是当前窗口的位置和大小
            using (new GUILayoutHorizontal(EditorStyles.toolbar))
            {
                // 按钮
                EditorUtils.CreateButton(EditorParameters.k_FirstFrameContent, EditorStyles.toolbarButton, () =>
                {
                    SelectFrame(0);
                });
                EditorUtils.CreateButton(EditorParameters.k_PreviousFrameContent, EditorStyles.toolbarButton, () =>
                {
                    SelectFrame(this._CurrentFrame - 1);
                });
                EditorUtils.CreateToggle(ref _IsPlaying, EditorParameters.k_PlayFramesContent, EditorStyles.toolbarButton);
                EditorUtils.CreateButton(EditorParameters.k_NextFrameContent, EditorStyles.toolbarButton, () =>
                {
                    SelectFrame(this._CurrentFrame + 1);
                });
                EditorUtils.CreateButton(EditorParameters.k_LastFrameContent, EditorStyles.toolbarButton, () =>
                {
                    SelectFrame(this._MaxFrameLength);
                });

                // 当前帧数
                EditorUtils.CreateLabel("当前");
                EditorUtils.CreateText(_CurrentFrame.ToString(), EditorStyles.textField, GUILayout.Width(40));
              
                
                // timeline刻度尺
                using (new GUILayoutArea(new Rect(k_HierarchyPanelWidth, EditorParameters.k_ToolbarHeight, k_TimelinePanelWidth, EditorParameters.k_ToolbarHeight)))
                {
                    Rect rulerRect = new Rect(0, 0, k_TimelinePanelWidth, EditorParameters.k_ToolbarHeight);
                    drawTimelineRuler(rulerRect);
                }

                // 总时长
                using (new GUILayoutArea(new Rect(k_HierarchyPanelWidth+k_TimelinePanelWidth+10, EditorParameters.k_ToolbarHeight+3, k_InspectorPanelWidth, EditorParameters.k_ToolbarHeight)))
                {
                    EditorUtils.CreateIntField("总时长", ref this._MaxFrameLength, 0, int.MaxValue);  
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
                    using (new GUILayoutScrollView(_ScrollPosition))
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

                        if (this._ScrollPosition.y > 0 && Event.current.type != EventType.Repaint)
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
                        using (new GUILayoutScrollView(ref _ScrollPosition))
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
                    drawInspector();
                }

            }
            
        }

        void drawInspector()
        {
            EditorUtils.CreateLabel("属性面板");
            if (this._SelectedGroupIndex != -1 && this._SelectedItemIndex != -1 && this._Groups.Count>this._SelectedGroupIndex)
            {
                this._Groups[this._SelectedGroupIndex].OnInspectorGUI(this._SelectedItemIndex, this._MaxFrameLength);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void drawFrameLine()
        {
            float x = k_HierarchyPanelWidth + EditorUtils.CalculateFrameToPosition(this._CurrentFrame) - this._ScrollPosition.x;

            if (x<k_HierarchyPanelWidth || x>k_HierarchyPanelWidth + k_TimelinePanelWidth)
            {
                return;
            }

            using (new GUILayoutArea(new Rect(x-5, EditorParameters.k_ToolbarHeight, 10, 20)))
            {
                GUILayout.Box(GUIContent.none, EditorParameters.k_TimelineFrameTag);
            }
            
            Vector2 p1 = new Vector2(x, EditorParameters.k_ToolbarHeight*2-10);
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

                float count = (width + _ScrollPosition.x) / EditorParameters.k_TickGap;
                count -= count % EditorParameters.k_TickUnit;
                float y1 = height * 0.5f;
                float y2 = height * 0.9f;
                float y3 = height;
                
                Color c1 = Color.white;
                Color c2 = new Color(c1.r, c1.g, c1.b, c1.a*0.5f);
                Rect labelRect = new Rect(Vector2.zero, Vector2.one);
                GUIContent content = new GUIContent();
                float xOffset = rect.xMin - _ScrollPosition.x;
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

                int frame = EditorUtils.CalculatePositionToFrame(e.mousePosition.x+this._ScrollPosition.x);
                SelectFrame(frame);
            }

            
        }

        void SelectFrame(int frame)
        {
            if (frame < 0)
            {
                frame = 0;
            }

            if (frame > this._MaxFrameLength)
            {
                frame = this._MaxFrameLength;
            }
            
            Debug.Log("========frame=======" + frame);
            this._IsPlaying = false;
            this._CurrentFrame = frame;
            
            
        }
    
    }

}

