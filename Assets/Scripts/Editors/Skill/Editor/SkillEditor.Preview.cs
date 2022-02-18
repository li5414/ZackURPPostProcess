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

        // 烘焙要播放的动画
        void bakeAnimation(string stateName)
        {
            if (this._Animator != null)
            {
                AnimationClip clip = getAnimationClip(stateName);

                if (clip)
                {
                    this._Animator.Rebind();
                    this._Animator.StopPlayback();
                    this._Animator.recorderStartTime =  0;

                    // 开始记录指定的帧数
                    int frameCount = getAnimationStateFrames(clip);
                    this._Animator.StartRecording(frameCount);
                    this._Animator.Play(stateName);
                
                    for (var i =  0; i < frameCount -  1; i++)
                    {
                        // 记录每一帧
                        this._Animator.Update( 1.0f / clip.frameRate);
                    }
                    // 完成记录
                    this._Animator.StopRecording();
                    Debug.Log($"Animator bake compelete: {stateName}!");
                
                    this._Animator.StartPlayback();
                }
            }
        }

        // 刷新动画显示
        void updateAnimation()
        {
            if (this._Animator!=null && this._SkillConfig!=null)
            {
                string stateName = this._SkillConfig.animatorState.ToString();
                AnimationClip clip = getAnimationClip(stateName);
                float escapTime = this._CurrentFrame / clip.frameRate;
                
                this._Animator.playbackTime = escapTime;
                this._Animator.Update(0);
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

