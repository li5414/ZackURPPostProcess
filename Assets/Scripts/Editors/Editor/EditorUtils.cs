using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Zack.Editor
{
    public class EditorUtils
    {
        /// <summary>
        /// 创建按钮
        /// </summary>
        /// <param name="text"></param>
        /// <param name="style"></param>
        /// <param name="callback"></param>
        public static void CreateButton(string text, GUIStyle style, System.Action callback, params GUILayoutOption[] options)
        {
            if (GUILayout.Button(text, style, options))
            {
                callback?.Invoke();
            }
        }

        public static void CreateLabel(string text, params GUILayoutOption[] options)
        {
            GUILayout.Label(text, EditorStyles.label, options);
        }
        
        /// <summary>
        /// 创建文本
        /// </summary>
        /// <param name="text"></param>
        /// <param name="style"></param>
        /// <param name="writeEnabled"></param>
        /// <param name="options"></param>
        public static void CreateText(ref string text, GUIStyle style, bool writeEnabled = false, params GUILayoutOption[] options)
        {
            if (writeEnabled)
            {
                text = GUILayout.TextField(text, style, options);
            }
            else
            {
                GUILayout.Label(text, style, options);
            }
        }
        public static void CreateText(ref string text, GUIStyle style, bool writeEnabled, int maxLength, params GUILayoutOption[] options)
        {
            if (writeEnabled)
            {
                text = GUILayout.TextField(text, maxLength, style, options);
            }
            else
            {
                GUILayout.Label(text, style, options);
            }
        }

        /// <summary>
        /// 创建Toggle
        /// </summary>
        /// <param name="value"></param>
        /// <param name="content"></param>
        /// <param name="style"></param>
        /// <param name="options"></param>
        public static void CreateToggle(ref bool value, GUIContent content, GUIStyle style, params GUILayoutOption[] options)
        {
            value = GUILayout.Toggle(value, content, style, options);
        }
        public static void CreateToggle(ref bool value, string text, GUIStyle style, params GUILayoutOption[] options)
        {
            value = GUILayout.Toggle(value, text, style, options);
        }

        /// <summary>
        /// 下拉菜单
        /// </summary>
        /// <param name="texts"></param>
        /// <param name="selectedIndex"></param>
        /// <param name="callback"></param>
        public static void CreateMenu(string[] texts, int selectedIndex, System.Action<int> callback)
        {
            GenericMenu menu = new GenericMenu();
            for (int i = 0; i < texts.Length; ++i)
            {
                menu.AddItem(new GUIContent(texts[i]), i==selectedIndex, (userdata) =>
                {
                    int index = (int)userdata;
                    if (index >= 0)
                    {
                        callback?.Invoke(index);
                    }
                }, i);
            }
            menu.ShowAsContext();
        }
    }
    
    /// <summary>
    /// 水平排版
    /// </summary>
    public class GUILayoutHorizontal : IDisposable
    {
        public GUILayoutHorizontal(params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal(options);
        }
        public GUILayoutHorizontal(GUIStyle style, params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal(style, options);
        }
        public GUILayoutHorizontal(string text, GUIStyle style, params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal(text, style, options);
        }
        public void Dispose()
        {
            GUILayout.EndHorizontal();
        }
    }
    
    /// <summary>
    /// 垂直排版
    /// </summary>
    public class GUILayoutVertical : IDisposable
    {
        public GUILayoutVertical(params GUILayoutOption[] options)
        {
            GUILayout.BeginVertical(options);
        }
        public GUILayoutVertical(GUIStyle style, params GUILayoutOption[] options)
        {
            GUILayout.BeginVertical(style, options);
        }
        public GUILayoutVertical(string text, GUIStyle style, params GUILayoutOption[] options)
        {
            GUILayout.BeginVertical(text, style, options);
        }
        public void Dispose()
        {
            GUILayout.EndVertical();
        }
    }
    
    /// <summary>
    /// 区域排版
    /// </summary>
    public class GUILayoutArea : IDisposable
    {
        public GUILayoutArea(Rect rect)
        {
            GUILayout.BeginArea(rect);
        }
        public GUILayoutArea(Rect rect, string text)
        {
            GUILayout.BeginArea(rect, text);
        }
        public GUILayoutArea(Rect rect, Texture image)
        {
            GUILayout.BeginArea(rect, image);
        }
        public GUILayoutArea(Rect rect, GUIStyle style)
        {
            GUILayout.BeginArea(rect, style);
        }
        public void Dispose()
        {
            GUILayout.EndArea();
        }
    }
    
    /// <summary>
    /// 滚动排版
    /// </summary>
    public class GUILayoutScrollView : IDisposable
    {
        public GUILayoutScrollView(ref Vector2 scrollPosition)
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        }
        public GUILayoutScrollView(ref Vector2 scrollPosition, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, params GUILayoutOption[] options)
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, horizontalScrollbar, verticalScrollbar, options);
        }
        
        public void Dispose()
        {
            GUILayout.EndScrollView();
        }
    }

    /// <summary>
    /// 检测ui数据是否发生改变
    /// </summary>
    public class GUIChangeCheck : IDisposable
    {
        private System.Action _callback;
        
        public GUIChangeCheck(System.Action callback)
        {
            EditorGUI.BeginChangeCheck();
            this._callback = callback;
        }

        public void Dispose()
        {
            if (EditorGUI.EndChangeCheck())
            {
                this._callback?.Invoke();
            }
        }
    }

}

