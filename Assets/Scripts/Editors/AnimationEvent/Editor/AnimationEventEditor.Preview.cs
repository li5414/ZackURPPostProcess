using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEditor.SceneManagement;
using Zack.Editor;

namespace AnimationEventEditor
{
    public partial class AnimationEventEditor
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
            if (this._Animator!=null && this._TotalFrames!=0)
            {
                string stateName = getAnimationStateName(this._CurrentFrame);
                this._AnimatorLayer = getAnimationStateLayer(this._CurrentFrame);
                AnimationClip clip = getAnimationClip(stateName);
                
                float percent = this._CurrentFrame / (float)this._TotalFrames;
                this._Animator.applyRootMotion = this._ApplyRootMotion;
                this._Animator.Play(stateName, this._AnimatorLayer, percent);
                if (!EditorApplication.isPlaying)
                {
                    this._Animator.Update(0);
                }
            }
        }
        
        void PreviewSkill()
        {
            // 保存配置
            this._IsPlaying = false;
            
            if (this._Animator)
            {
                this._Animator.enabled = false;
                
                // 播放
                string stateName = this._SelectedState.GetDescription();
                this._AnimatorLayer = EditorUtils.GetAnimatorLayer(this._SelectedState);
                this._Animator.enabled = true;
                this._Animator.applyRootMotion = _ApplyRootMotion;
                this._Animator.Play(stateName, this._AnimatorLayer, 0);
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
                return Mathf.FloorToInt(clip.length * clip.frameRate);
            }
            return 0;
        }

        int convertTime2Frame(AnimationClip clip, float time)
        {
            if (clip)
            {
                return Mathf.FloorToInt(time * clip.frameRate);
            }
            return 0;
        }

        
        
        // 获取动画状态机上的AnimationClip
        AnimationClip getAnimationClip(string stateName)
        {
            if (this._MainCharacter && this._Animator)
            {
                AnimatorController controller = this._Animator.runtimeAnimatorController as AnimatorController;
                AnimatorOverrideController overrideController = null;
                bool hasOverride = false;

                if (controller)
                {
                    // 使用原始的AnimatorController
                    hasOverride = false;
                }
                else
                {
                    // 使用AnimatorOverrideController
                    hasOverride = true;
                    
                    overrideController = this._Animator.runtimeAnimatorController as AnimatorOverrideController;
                    controller = overrideController.runtimeAnimatorController as AnimatorController;
                }

                for (int layerIdx = 0; layerIdx < controller.layers.Length; ++layerIdx)
                {
                    ChildAnimatorState[] states = controller.layers[layerIdx].stateMachine.states;
                    for (int i = 0; i < states.Length; ++i)
                    {
                        if (states[i].state.name == stateName)
                        {
                            AnimationClip clip = states[i].state.motion as AnimationClip;    // 将motion转为AnimationClip
                            
                            // 如果使用了AnimatorOverrideController，并且overrides中有值，则用overrides中的值代替
                            if (hasOverride && overrideController[clip])
                            {
                                clip = overrideController[clip];
                            }
                            
                            return clip;
                        }
                    }
                }
            }

            return null;
        }
        
    }

}

