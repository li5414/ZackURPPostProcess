using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Skill;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using RangeAttribute = UnityEngine.RangeAttribute;

namespace Zack.Editor
{
    public static class EditorUtils
    {
        public static string GetDescription(this Enum e)
        {
            var fieldInfo = e.GetType().GetField(e.ToString());
            var descriptions = fieldInfo.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
            if (descriptions.Length == 0)
            {
                return null;
            }

            return (descriptions[0] as System.ComponentModel.DescriptionAttribute).Description;
        }

        public static string GetDescripthion<T>(this T component) where T : class
        {
            var descriptions = typeof(T).GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
            if (descriptions.Length == 0)
            {
                return null;
            }

            return (descriptions[0] as System.ComponentModel.DescriptionAttribute).Description;
        }
        
        public static string GetDescripthion(this FieldInfo field)
        {
            var descriptions = field.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
            if (descriptions.Length == 0)
            {
                return null;
            }

            return (descriptions[0] as System.ComponentModel.DescriptionAttribute).Description;
        }

        public static T GetFieldAttribute<T>(FieldInfo field) where T : Attribute
        {
            var attributes = field.GetCustomAttributes(typeof(T), false);
            if (attributes.Length == 0)
            {
                return null;
            }

            return (attributes[0] as T);
        }

        public static SkillAssetType GetSkillAssetType(FieldInfo field)
        {
            var attributes = field.GetCustomAttributes(typeof(SkillAssetAttribute), false);
            if (attributes.Length == 0)
            {
                return SkillAssetType.GameObject;
            }
            return (attributes[0] as SkillAssetAttribute).type;
        }

        public static int GetAnimatorLayer(Enum e)
        {
            var fieldInfo = e.GetType().GetField(e.ToString());
            var descriptions = fieldInfo.GetCustomAttributes(typeof(AnimatorLayerAttribute), false);
            if (descriptions.Length == 0)
            {
                return 0;
            }

            return (descriptions[0] as AnimatorLayerAttribute).layer;
        }
        
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

        public static void CreateButton(GUIContent content, GUIStyle style, System.Action callback, params GUILayoutOption[] options)
        {
            if (GUILayout.Button(content, style, options))
            {
                callback?.Invoke();
            }
        }

        public static void CreateTextField(string title, ref string text, params GUILayoutOption[] options)
        {
            text = EditorGUILayout.TextField(title, text, options);
        }

        public static string CreateTextField(string title, string text, params GUILayoutOption[] options)
        {
            return EditorGUILayout.TextField(title, text, options);
        }

        public static void CreateTextFieldDisable(string title, string text, params GUILayoutOption[] options)
        {
            GUI.enabled = false;
            EditorGUILayout.TextField(title, text, options);
            GUI.enabled = true;
        }
        
        public static void CreateIntField(string title, ref int value, params GUILayoutOption[] options)
        {
            value = EditorGUILayout.IntField(title, value, options);
        }
        public static int CreateIntField(string title, int value, params GUILayoutOption[] options)
        {
            return EditorGUILayout.IntField(title, value, options);
        }
        public static int CreateIntField(string title, int value, int min, int max, params GUILayoutOption[] options)
        {
            value = EditorGUILayout.IntField(title, value, options);
            if (value < min)
            {
                value = min;
            }
            else if (value > max)
            {
                value = max;
            }

            return value;
        }
        public static void CreateIntField(ref int value, int min, int max, params GUILayoutOption[] options)
        {
            value = EditorGUILayout.IntField(value, options);
            if (value < min)
            {
                value = min;
            }
            else if (value > max)
            {
                value = max;
            }
        }
        public static void CreateIntField(string title, ref int value, int min, int max, params GUILayoutOption[] options)
        {
            value = EditorGUILayout.IntField(title, value, options);
            if (value < min)
            {
                value = min;
            }
            else if (value > max)
            {
                value = max;
            }
        }
        public static void CreateIntFieldDisable(string title, int value, params GUILayoutOption[] options)
        {
            GUI.enabled = false;
            EditorGUILayout.IntField(title, value, options);
            GUI.enabled = true;
        }
        
        public static void CreateFloatField(string title, ref float value, params GUILayoutOption[] options)
        {
            value = EditorGUILayout.FloatField(title, value, options);
        }
        public static float CreateFloatField(string title, float value, params GUILayoutOption[] options)
        {
            return EditorGUILayout.FloatField(title, value, options);
        }
        
        public static Vector2 CreateVector2Field(string title, Vector2 value, params GUILayoutOption[] options)
        {
            using (new GUILayoutHorizontal())
            {
                CreateLabel(title);
                GUILayout.FlexibleSpace();
                value = EditorGUILayout.Vector2Field(GUIContent.none, value, options);        
            }
            return value;
        }
        
        public static Vector3 CreateVector3Field(string title, Vector3 value, params GUILayoutOption[] options)
        {
            using (new GUILayoutHorizontal())
            {
                CreateLabel(title);
                GUILayout.FlexibleSpace();
                value = EditorGUILayout.Vector3Field(GUIContent.none, value, options);     
            }
            return value;
        }
        
        public static Vector4 CreateVector4Field(string title, Vector4 value, params GUILayoutOption[] options)
        {
            using (new GUILayoutHorizontal())
            {
                CreateLabel(title);
                GUILayout.FlexibleSpace();
                value = EditorGUILayout.Vector4Field(GUIContent.none, value, options);     
            }
            return value;
        }

        public static void CreateBoolField(string title, ref bool value, params GUILayoutOption[] options)
        {
            value = EditorGUILayout.Toggle(title, value, options);
        }
        public static bool CreateBoolField(string title, bool value, params GUILayoutOption[] options)
        {
            return EditorGUILayout.Toggle(title, value, options);
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
        public static void CreateText(string text, GUIStyle style, params GUILayoutOption[] options)
        {
            GUILayout.TextField(text, style, options);
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
        public static void CreateToggle(bool value, string text, GUIStyle style, params GUILayoutOption[] options)
        {
            GUILayout.Toggle(value, text, style, options);
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
        
        // 通过枚举创建菜单
        public static void CreateMenu<T>(int selectedIndex, System.Action<int> callback) where T : Enum
        {
            List<string> texts = new List<string>();
            foreach (var e in Enum.GetValues(typeof(T)))
            {
                texts.Add(((T)e).GetDescription());
            }

            GenericMenu menu = new GenericMenu();
            for (int i = 0; i < texts.Count; ++i)
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
        public static void CreateMenu<T>(T eSelectedIndex, System.Action<int> callback) where T : Enum
        {
            int selectedIndex = -1;

            Type type = eSelectedIndex.GetType();
            List<string> texts = new List<string>();
            int idx = 0;
            foreach (var e in Enum.GetValues(type))
            {
                texts.Add(((T)e).GetDescription());
                if (Enum.GetName(type, e) == Enum.GetName(type, eSelectedIndex))
                {
                    selectedIndex = idx;
                }
                idx++;
            }

            
            GenericMenu menu = new GenericMenu();
            for (int i = 0; i < texts.Count; ++i)
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

        /// <summary>
        /// 获取UnityEngine.Object的GUID
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetGameObjectGUID(UnityEngine.Object obj)
        {
            return AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(obj));
        }

        /// <summary>
        /// 更新SkillAsset的ab信息数据
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="obj"></param>
        public static void UpdateSkillAssetBundleInfo(SkillAsset asset)
        {
            if (asset.mainObject == null)
            {
                return;
            }
            
            string path = AssetDatabase.GetAssetPath(asset.mainObject);
            AssetImporter assetImporter = AssetImporter.GetAtPath(path);  //得到AssetImporter
            if (assetImporter)
            {
                // 设置ab包名
                asset.bundleName = assetImporter.assetBundleName;
                // 设置资源名
                asset.assetName = getObjectName(path);
            }
        }
        
        public static string getObjectName(string path)
        {
            string objectName = path;
            objectName = objectName.Substring(0, objectName.LastIndexOf("."));
            objectName = objectName.Substring(objectName.LastIndexOf("/") + 1);
            return objectName;
        }
        
        
        /// <summary>
        /// 计算timeline的矩形Rect
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="elementHeight"></param>
        /// <returns></returns>
        public static Rect CalculateTimeRect(int begin, int end, int elementHeight = 20)
        {
            float xMin = CalculateFrameToPosition(begin);
            float xMax = CalculateFrameToPosition(end);
            return new Rect(xMin, 0, xMax-xMin, elementHeight);        
        }
        /// <summary>
        /// 计算timeline帧位置
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        public static float CalculateFrameToPosition(int frame)
        {
            return EditorParameters.k_RulerOffsetX + frame * EditorParameters.k_TickGap;
        }

        public static int CalculatePositionToFrame(float position)
        {
            return (int) ((position - EditorParameters.k_RulerOffsetX) / EditorParameters.k_TickGap);
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
        public GUILayoutScrollView(Vector2 scrollPosition)
        {
//            GUILayout.BeginScrollView(scrollPosition);
            GUILayout.BeginScrollView(scrollPosition, GUIStyle.none, GUIStyle.none);
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

    /// <summary>
    /// 临时替换Handles.Color
    /// </summary>
    public class GUIHandlesColor : IDisposable
    {
        private Color srcColor;

        public GUIHandlesColor(Color color)
        {
            srcColor = Handles.color;
            Handles.color = color;
        }

        public void Dispose()
        {
            Handles.color = srcColor;
        }
    }

    public class GUIColor : IDisposable
    {
        private Color srcColor;

        public GUIColor(Color color)
        {
            srcColor = GUI.color;
            GUI.color = color;
        }

        public void Dispose()
        {
            GUI.color = srcColor;
        }
    }

}

