using System.Collections;
using System.Collections.Generic;
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
            EditorWindow.GetWindow<SkillEditor>().Show();
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
                        Debug.Log("=====打开====="+tmpPath);
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
                EditorUtils.CreateToggle(ref _IsClickFirstFrame, EditorParameters.k_FirstFrameContent, EditorStyles.toolbarButton);
                EditorUtils.CreateToggle(ref _IsClickPreviousFrame, EditorParameters.k_PreviousFrameContent, EditorStyles.toolbarButton);
                EditorUtils.CreateToggle(ref _IsPlaying, EditorParameters.k_PlayFramesContent, EditorStyles.toolbarButton);
                EditorUtils.CreateToggle(ref _IsClickNextFrame, EditorParameters.k_NextFrameContent, EditorStyles.toolbarButton);
                EditorUtils.CreateToggle(ref _IsClickLastFrame, EditorParameters.k_LastFrameContent, EditorStyles.toolbarButton);
                
                // 当前帧数
                EditorUtils.CreateText(_CurrentFrame.ToString(), EditorStyles.textField, GUILayout.Width(70));
                
                // timeline刻度尺
                using (new GUILayoutArea(new Rect(k_HierarchyPanelWidth, EditorParameters.k_ToolbarHeight, k_TimelinePanelWidth, EditorParameters.k_ToolbarHeight)))
                {
                    Rect rulerRect = new Rect(0, 0, k_TimelinePanelWidth, EditorParameters.k_ToolbarHeight);
                    drawTimelineRuler(rulerRect);
                }
                
                GUILayout.FlexibleSpace();
            }
        }

        /// <summary>
        /// 数据编辑
        /// </summary>
        void drawHierarchy()
        {
            using (new GUILayoutHorizontal(EditorParameters.k_WindowBackground))
            {
                // hierarchy面板
                using (new GUILayoutVertical(GUILayout.Width(k_HierarchyPanelWidth)))
                {
                    using (new GUILayoutScrollView(_ScrollPosition))
                    {
                        for (int i = 0; i < 100; ++i)
                        {
                            using (new GUILayoutHorizontal(GUILayout.Height(k_ElementHeight)))
                            {
                                GUILayout.Label($"{i}");
                                // 不知道为啥两个ScrollView滚动进度没办法对齐，所以创建一个box用来将每个元素大小调整一致
                                GUILayout.Box(GUIContent.none, EditorParameters.k_TimelineBlockLeft, GUILayout.Width(0f), GUILayout.Height(k_ElementHeight));
                            }
                        }

                    }
                }

                // timeline数据
                using (new GUILayoutVertical(GUILayout.Width(k_TimelinePanelWidth)))
                {
                    drawTimelineArea();
                    // 绘制timeline线
                    drawTimeFrameGUI();
                }
                
                // inspector数据
                using (new GUILayoutVertical(GUILayout.Width(k_InspectorPanelWidth)))
                {
                    GUILayout.Label("属性面板");
                }
                
   

            }
            
        }

        void drawTimeFrameGUI()
        {
            float x = k_HierarchyPanelWidth + calculateFramePosition(this._CurrentFrame) - this._ScrollPosition.x;

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
        /// timeline内容
        /// </summary>
        /// <param name="rect"></param>
        void drawTimelineArea()
        {
//            using (new GUILayoutVertical(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
            using (new GUILayoutVertical())
            {
                using (new GUILayoutScrollView(ref _ScrollPosition))
                {
                    for (int i = 0; i < 100; ++i)
                    {
//                        GUILayout.Space(k_ElementHeight);

                        {
                            bool isSelected = i == 1;
                            GUIStyle backgroundStyle = isSelected
                                ? EditorParameters.k_BackgroundSelected
                                : EditorParameters.k_BackgroundEven;
                            using (new GUILayoutHorizontal(GUILayout.Height(k_ElementHeight)))
                            {
                                // TODO: 绘制timeline数据元素
                                _drawTestElement();
                            }
                        }

                        
                    }
                }
            }
        }

        void _drawTestElement(  )
        {
            int length = 500 - 10;
            Rect rect = calculateTimeRect(10, 500);

            float halfWidth = rect.width / 2;
            GUILayout.Space(rect.x);
            Rect lastRect = GUILayoutUtility.GetLastRect();

            Color color = GUI.color;
            GUI.color = length > 0 ? color : Color.red;
            GUILayout.Box(GUIContent.none, EditorParameters.k_TimelineBlockLeft, GUILayout.Width(halfWidth), GUILayout.Height(k_ElementHeight));
            GUILayout.Box(GUIContent.none, EditorParameters.k_TimelineBlockRight, GUILayout.Width(halfWidth), GUILayout.Height(k_ElementHeight));
            GUI.color = color;
            lastRect.x += lastRect.width;
            lastRect.width = rect.width;
            lastRect.height = rect.height;

//            return lastRect;
        }

        Rect calculateTimeRect(int begin, int end)
        {
            float xMin = calculateFramePosition(begin);
            float xMax = calculateFramePosition(end);
            return new Rect(xMin, 0, xMax-xMin, k_ElementHeight);            
        }
        float calculateFramePosition(int frame)
        {
            return EditorParameters.k_RulerOffsetX + frame * EditorParameters.k_TickGap;
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
                Color color = Handles.color;
                Color c1 = Color.white;
                Color c2 = new Color(c1.r, c1.g, c1.b, c1.a*0.5f);
                Rect labelRect = new Rect(Vector2.zero, Vector2.one);
                GUIContent content = new GUIContent();
                float xOffset = rect.xMin - _ScrollPosition.x;
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
                        Handles.color = c1;
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
                        Handles.color = c2;
                    }
                    var p2 = new Vector2(x, y3);
                    // 绘制刻度
                    Handles.DrawLine(p1, p2);
                }
                Handles.color = color;
            }
            
        }
    
    }

}

