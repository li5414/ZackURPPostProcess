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
            drawTopToolbar();

            drawHierarchyToolbar();
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

        private bool isPlay = false;
        private Vector2 _ScrollPosition;
        void drawHierarchyToolbar()
        {
            // position中存储的是当前窗口的位置和大小
            using (new GUILayoutArea(new Rect(0, EditorParameters.k_ToolbarHeight, position.width, position.height - EditorParameters.k_ToolbarHeight)))
            {
                    using (new GUILayoutArea(new Rect(0, 0, k_HierarchyPanelWidth, EditorParameters.k_ToolbarHeight)))
                    {
                        using (new GUILayoutHorizontal(EditorStyles.toolbar, GUILayout.Width(k_HierarchyPanelWidth)))
                        {
                            EditorUtils.CreateToggle(ref isPlay, EditorParameters.k_PlayFramesContent, EditorStyles.toolbarButton);
                            EditorUtils.CreateToggle(ref isPlay, EditorParameters.k_FirstFrameContent, EditorStyles.toolbarButton);
                            EditorUtils.CreateToggle(ref isPlay, EditorParameters.k_PreviousFrameContent, EditorStyles.toolbarButton);
                        }
                    }

                    using (new GUILayoutArea(new Rect(k_HierarchyPanelWidth, 0, position.width - k_HierarchyPanelWidth - k_InspectorPanelWidth, EditorParameters.k_ToolbarHeight)))
                    {
                        Rect rulerRect = new Rect(0, 0, position.width - k_HierarchyPanelWidth - k_InspectorPanelWidth, EditorParameters.k_ToolbarHeight);
                        drawTimelineRuler(rulerRect);
                    }

                    using (new GUILayoutArea(new Rect(position.width-k_InspectorPanelWidth, 0, k_HierarchyPanelWidth, EditorParameters.k_ToolbarHeight)))
                    {
                        using (new GUILayoutHorizontal(EditorStyles.toolbar, GUILayout.Width(k_HierarchyPanelWidth)))
                        {
                            EditorUtils.CreateToggle(ref isPlay, EditorParameters.k_NextFrameContent, EditorStyles.toolbarButton);
                            EditorUtils.CreateToggle(ref isPlay, EditorParameters.k_LastFrameContent, EditorStyles.toolbarButton);
                        }
                    }

                
                    

                }
                
        }

        /// <summary>
        /// timeline内容
        /// </summary>
        /// <param name="rect"></param>
        void drawTimelineArea(Rect rect)
        {
            using (new GUILayoutVertical(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
            {
                float timeAreaWidth = rect.width - k_InspectorPanelWidth;
                
                using (new GUILayoutScrollView(ref _ScrollPosition))
                {
                    for (int i = 0; i < 10; ++i)
                    {
                        GUILayout.Space(k_ElementHeight);
                        // TODO: 绘制timeline数据元素
                        
                        GUILayout.Space(2);
                    }
                }
            }
        }

        void _drawTestElement( bool isActive)
        {
            
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
                        labelRect.size = EditorParameters.k_TimeBlockLeft.CalcSize(content);
                        GUI.Label(labelRect, content, EditorParameters.k_TimeBlockLeft);
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

