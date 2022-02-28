using System.Collections;
using System.Collections.Generic;
using System.IO;
using Codice.Client.Commands;
using UnityEditor;
using UnityEngine;
using Zack.Editor;

namespace Skill.Editor
{
    public class SkillEditorNewConfig : EditorWindow
    {
        void Awake()
        {
            Vector2 size = new Vector2(300, 60);
            this.minSize = size;
            this.maxSize = size;
            this.title = "新建配置";
        }

        private string _SkillId;
        
        void OnGUI()
        {
            using (new GUILayoutVertical())
            {
                using (new GUILayoutHorizontal())
                {
                    EditorUtils.CreateLabel("技能id:", GUILayout.Width(60));
                    EditorUtils.CreateText(ref this._SkillId, EditorStyles.textField, true);
                }
                EditorUtils.CreateButton("新建", EditorParameters.k_ACButton, () =>
                {
                    var skillEditor = EditorWindow.GetWindow<SkillEditor>();
                    skillEditor.NewConfig(this._SkillId);
                    this.Close();

                }, GUILayout.Height(20));
            }
        }
    }

}

