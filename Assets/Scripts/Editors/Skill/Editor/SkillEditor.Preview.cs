using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

namespace Skill.Editor
{
    public partial class SkillEditor
    {
        /// <summary>
        /// 加载主角模型
        /// </summary>
        void loadMainCharacter()
        {
            // 删除旧的
            if (this._MainCharacter && this._SkillConfig.rootObjectGuid!=this._MainCharacterResourceGuid)
            {
                GameObject.DestroyImmediate(this._MainCharacter);
                this._MainCharacterResourceGuid = string.Empty;
                this._MainCharacterResource = null;
                this._MainCharacter = null;
                this._Animator = null;
            }

            // 替换新的
            if (this._SkillConfig.rootObjectGuid != null)
            {
                Debug.Log("==创建模型==");
                this._MainCharacterResourceGuid = this._SkillConfig.rootObjectGuid;
                this._MainCharacterResource = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(this._SkillConfig.rootObjectGuid));
                if (this._MainCharacterResource)
                {
                    this._MainCharacter = GameObject.Instantiate(this._MainCharacterResource, Vector3.zero, Quaternion.identity);
                    this._Animator = this._MainCharacter.GetComponent<Animator>();
                }
                
            }
        }


        // 创建新场景
        void createNewScene()
        {
            EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);
        }
    }

}

