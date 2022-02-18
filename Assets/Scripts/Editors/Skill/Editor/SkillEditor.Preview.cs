using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEditor.SceneManagement;

namespace Skill.Editor
{
    public partial class SkillEditor
    {
        /// <summary>
        /// 加载主角模型
        /// </summary>
        void loadMainCharacter(string path)
        {
            string guid = AssetDatabase.AssetPathToGUID(path);
            
            // 删除旧的
            if (this._MainCharacter && guid!=this._MainCharacterResourceGuid)
            {
                GameObject.DestroyImmediate(this._MainCharacter);
                this._MainCharacterResourceGuid = string.Empty;
                this._MainCharacterResource = null;
                this._MainCharacter = null;
                this._Animator = null;
            }

            // 替换新的
            if (guid!=null && guid!=String.Empty)
            {
                Debug.Log("==创建模型==");
                this._MainCharacterResourceGuid = guid;
                this._MainCharacterResource = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));
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
        
        // 获取动画状态时长
        int getAnimationStateFrames(string stateName)
        {
            if (this._MainCharacter && this._Animator)
            {
                Debug.Log("=====getAnimationStateFrames=11===");
                AnimatorController controller = this._Animator.runtimeAnimatorController as AnimatorController;

                for (int layerIdx = 0; layerIdx < controller.layers.Length; ++layerIdx)
                {
                    ChildAnimatorState[] states = controller.layers[layerIdx].stateMachine.states;
                    for (int i = 0; i < states.Length; ++i)
                    {
                        if (states[i].state.name == stateName)
                        {
                            Debug.Log(states[i].state.name);
                            return Mathf.CeilToInt(states[i].state.motion.averageDuration * 30);
                        }
                    }
                }
                    
            }
            Debug.Log("=====getAnimationStateFrames=22===");

            return 0;
        }
        
    }

}

