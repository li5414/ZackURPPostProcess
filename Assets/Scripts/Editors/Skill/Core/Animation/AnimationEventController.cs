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
        // 动画回调处理
        public AnimationEventHandler _eventHandler = new AnimationEventHandler();


        public void Awake()
        {
            this._animator = this.gameObject.GetComponent<Animator>();
            this._controller = new AnimatorOverrideController();
            this._controller.runtimeAnimatorController = this._animator.runtimeAnimatorController;
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
                AnimationClip clip = this._controller[clipName];    // 注意：这里的name是animation的名称(即State中的Motion名称)，不是State的名字
                if (clip != null)
                {
                    return this._eventHandler.AddEvent(clip, clip.length, completeCallback);
                }
            }

            return -1;
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
                AnimationClip clip = this._controller[clipName];    // 注意：这里的name是animation的名称(即State中的Motion名称)，不是State的名字
                if (clip != null)
                {
                    return this._eventHandler.AddEvent(clip, frame/clip.frameRate, callback);
                }
            }

            return -1;
        }
        /// <summary>
        /// 添加动画事件
        /// </summary>
        /// <param name="clipName"></param>
        /// <param name="normalizedTime"></param>
        /// <param name="callback"></param>
        public int AddAnimationEventByNormalized(string clipName, float normalizedTime, Action callback)
        {
            // 添加完成回调事件
            if (callback != null)
            {
                AnimationClip clip = this._controller[clipName];    // 注意：这里的name是animation的名称(即State中的Motion名称)，不是State的名字
                if (clip != null)
                {
                    return this._eventHandler.AddEvent(clip, clip.length*normalizedTime, callback);
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

