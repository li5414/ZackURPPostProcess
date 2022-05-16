using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Skill
{
    public class AnimationEventController : MonoBehaviour
    {
        // Animator组件
        private Animator _animator;
        // controller： 用于获取AnimationClip
        private AnimatorOverrideController _controller;
        // override动画健值对列表
        private List<KeyValuePair<AnimationClip, AnimationClip>> _overrides;
        // 动画回调处理
        public AnimationEventHandler _eventHandler = new AnimationEventHandler();


        public void Awake()
        {
            this._animator = this.gameObject.GetComponent<Animator>();
            this._controller = new AnimatorOverrideController();
            AnimatorOverrideController animatorOverrideController = this._animator.runtimeAnimatorController as AnimatorOverrideController;
            if (!animatorOverrideController)
            {
                // 使用原始AnimatorController
                this._controller.runtimeAnimatorController = this._animator.runtimeAnimatorController;
            }
            else
            {
                // 使用AniamtorOverrideController
                this._controller.runtimeAnimatorController = animatorOverrideController.runtimeAnimatorController;  // 原始AnimatorController
                // 获取overrides列表
                this._overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();
                animatorOverrideController.GetOverrides(this._overrides);
            }
        }

        /// <summary>
        /// 通过clipName获取AnimationClip
        /// </summary>
        /// <param name="clipName">动画名</param>
        /// <returns></returns>
        public AnimationClip GetAnimationClip(string clipName)
        {
            AnimationClip clip = this._controller[clipName];    // 注意：这里的name是animation的名称(即State中的Motion名称)，不是State的名字

            // 检测是否使用AnimatorOverrideController的AnimationClip
            AnimationClip overrideClip = getOverrideAnimationClip(clipName);
            if (overrideClip)
            {
                clip = overrideClip;
            }

            return clip;
        }

        /// <summary>
        /// 从AnimatorOverrideController中获取override的AnimationClip
        /// </summary>
        /// <param name="clipName"></param>
        /// <returns></returns>
        private AnimationClip getOverrideAnimationClip(string clipName)
        {
            if (this._overrides != null && this._overrides.Count > 0)
            {
                for (int i = 0; i < this._overrides.Count; ++i)
                {
                    KeyValuePair<AnimationClip, AnimationClip> pair = this._overrides[i];
                    if (pair.Value != null && pair.Value.name == clipName)
                    {
                        return pair.Value;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 添加动画播完回调事件
        /// </summary>
        /// <param name="clipName"></param>
        /// <param name="completeCallback"></param>
        public int AddAnimationCompleteEvent(string clipName, Action completeCallback)
        {
            // 添加完成回调事件
            if (completeCallback != null)
            {
                AnimationClip clip = GetAnimationClip(clipName);    // 注意：这里的name是animation的名称(即State中的Motion名称)，不是State的名字
                if (clip != null)
                {
                    return this._eventHandler.AddEvent(clip, clip.length, completeCallback);
                }
            }

            return -1;
        }

        /// <summary>
        /// 注册回调
        /// </summary>
        /// <param name="clipName"></param>
        /// <param name="frame"></param>
        /// <param name="functionName"></param>
        public void RegisiterAnimationEvent(string clipName, int frame, string functionName)
        {
            AnimationClip clip = GetAnimationClip(clipName);    // 注意：这里的name是animation的名称(即State中的Motion名称)，不是State的名字
            if (clip != null)
            {
                float time = frame / clip.frameRate;
                if (time > clip.length)
                {
                    Debug.LogWarning("Frame index is out of clip animation's range, please check your programe at RegisiterAnimationEvent");
                    time = clip.length;
                }
                else if (clip.length - time < 0.01f)
                {
                    time = clip.length;
                }
                AnimationEventManager.GetInstance().AddAnimationEvent(clip, time, functionName);
            }
        }
        
        /// <summary>
        /// 添加动画事件
        /// </summary>
        /// <param name="clipName"></param>
        /// <param name="frame"></param>
        /// <param name="callback"></param>
        public int AddAnimationEvent(string clipName, int frame, Action callback)
        {
            // 添加完成回调事件
            if (callback != null)
            {
                AnimationClip clip = GetAnimationClip(clipName);    // 注意：这里的name是animation的名称(即State中的Motion名称)，不是State的名字
                if (clip != null)
                {
                    float time = frame / clip.frameRate;
                    if (time > clip.length)
                    {
                        Debug.LogWarning("Frame index is out of clip animation's range, please check your programe at RegisiterAnimationEvent");
                        time = clip.length;
                    }
                    else if (clip.length - time < 0.01f)
                    {
                        time = clip.length;
                    }
                    return this._eventHandler.AddEvent(clip, time, callback);
                }
            }

            return -1;
        }
    
        public void RemoveAnimationEvent(int id)
        {
            if (id != -1)
            {
                this._eventHandler.RemoveEvent(id);
            }
        }

        public void ClearAllAnimationEvents()
        {
            this._eventHandler.ClearAllEvents();
        }
        
        /// <summary>
        /// 指定动画层上，是否正在播放该动画
        /// </summary>
        /// <param name="stateName"></param>
        /// <param name="layer"></param>
        /// <returns></returns>
        public bool IsAnimationPlaying(string stateName, int layer)
        {
            AnimatorStateInfo animationStateInfo = this._animator.GetCurrentAnimatorStateInfo(layer);
            return animationStateInfo.IsName(stateName);
        }
        
        /// <summary>
        /// 设置Controller的Parameters
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void SetFloat(string name, float value)
        {
            this._animator.SetFloat(name, value);
        }
        public float GetFloat(string name)
        {
            return this._animator.GetFloat(name);
        }

        /// <summary>
        /// 设置是否开启ApplyRootMotion
        /// </summary>
        /// <param name="applyRootMotion"></param>
        public void SetApplyRootMotion(bool applyRootMotion)
        {
            this._animator.applyRootMotion = applyRootMotion;
        }

        void Update()
        {
            // 清除没用的事件
//            this._eventHandler.ClearUnusedEvents();

        }

        /// <summary>
        /// 用于动画事件的回调
        /// </summary>
        void onAnimationEvent(AnimationEvent evt)
        {
            Debug.Log($"======onAnimationEvent===={(evt.objectReferenceParameter as AnimationClip).name}={evt.floatParameter}==");
            
            this._eventHandler.ExecuteEvent(evt.objectReferenceParameter as AnimationClip, evt.floatParameter);
        }
        
    }
}

