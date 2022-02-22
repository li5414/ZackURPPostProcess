using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEditor.SceneManagement;
using Zack.Editor;

namespace Skill.Editor
{
    public partial class SkillEditor
    {
        /// <summary>
        /// 加载主角模型
        /// </summary>
        void LoadMainCharacter(string path)
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

        // 刷新动画显示
        void updateAnimation()
        {
            if (this._Animator!=null && this._SkillConfig!=null)
            {
                string stateName = getAnimationStateName(this._CurrentFrame);
                AnimationClip clip = getAnimationClip(stateName);
                
                float percent = this._CurrentFrame / (float)this._SkillConfig.totalFrames;
                this._Animator.Play(stateName, -1, percent);
                if (!EditorApplication.isPlaying)
                {
                    this._Animator.Update(0);
                }
            }
        }
        
        void PreviewSkill()
        {
            this._IsPlaying = false;
            SaveConfig();
            
            if (this._Animator)
            {
                AnimationEventController controller = this._Animator.GetComponent<AnimationEventController>();
                // 清除所有动画事件回调
                controller.ClearAllAnimationEvents();
                // 使用技能
                SkillManager.GetInstance().UseSkill(controller, this._SkillConfig);
            }

            // 播放
            if (this._Animator!=null && this._SkillConfig!=null)
            {
                string stateName = this._SkillConfig.animations[0].state.GetDescription();
                this._Animator.Play(stateName, -1, 0);
            }
        }
        

        /// <summary>
        /// 创建新场景
        /// </summary>
        void createNewScene()
        {
            EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);
        }
        
        
        // 获取动画状态时长
        int getAnimationStateFrames(string stateName)
        {
            AnimationClip clip = getAnimationClip(stateName);
            if (clip)
            {
                return Mathf.CeilToInt(clip.length * clip.frameRate);
            }
            return 0;
        }

        int getAnimationStateFrames(AnimationClip clip)
        {
            if (clip)
            {
                return Mathf.CeilToInt(clip.length * clip.frameRate);
            }
            return 0;
        }

        
        
        // 获取动画状态机上的AnimationClip
        AnimationClip getAnimationClip(string stateName)
        {
            if (this._MainCharacter && this._Animator)
            {
                AnimatorController controller = this._Animator.runtimeAnimatorController as AnimatorController;

                for (int layerIdx = 0; layerIdx < controller.layers.Length; ++layerIdx)
                {
                    ChildAnimatorState[] states = controller.layers[layerIdx].stateMachine.states;
                    for (int i = 0; i < states.Length; ++i)
                    {
                        if (states[i].state.name == stateName)
                        {
                            AnimationClip clip = states[i].state.motion as AnimationClip;    // 将motion转为AnimationClip
                            return clip;
                        }
                    }
                }
            }

            return null;
        }
        
    }

}

