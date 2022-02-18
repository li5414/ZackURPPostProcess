using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Rouge.Animation
{
    public class AnimationController : MonoBehaviour
    {
        // Animator组件
        private Animator _animator;
        // controller： 用于获取AnimationClip
        private AnimatorOverrideController _controller;
        // 全身动作Layer
        private const int _baseLayerIndex = 0;
        // 上身动作Layer
        private const int _upperBodyLayerIndex = 1;
        // 动画回调处理
        public AnimationEventHandler _eventHandler = new AnimationEventHandler();


        public void Awake()
        {
            this._animator = this.gameObject.GetComponent<Animator>();
            this._controller = new AnimatorOverrideController();
            this._controller.runtimeAnimatorController = this._animator.runtimeAnimatorController;
        }


        /// <summary>
        /// 全身播放同一个动画
        /// </summary>
        /// <param name="stateName">全身状态机名称BaseLayer (理论上需要添加Complete回调的stateName和AnimationClip应该保持相同名称)</param>
        /// <param name="blendDuration"></param>
        public void PlayAnimation(string stateName, float blendDuration)
        {
            this._animator.CrossFadeInFixedTime(stateName, blendDuration, _baseLayerIndex);
            this._animator.CrossFadeInFixedTime(stateName, blendDuration, _upperBodyLayerIndex);
        }

        /// <summary>
        /// 上身和下身播放不同动画
        /// </summary>
        /// <param name="stateName">全身状态机名称BaseLayer (理论上需要添加Complete回调的stateName和AnimationClip应该保持相同名称)</param>
        /// <param name="upperStatename">上身状态机名称BaseLayer (理论上需要添加Complete回调的stateName和AnimationClip应该保持相同名称)</param>
        /// <param name="blendDuration"></param>
        public void PlayAnimationsTogether(string stateName, string upperStatename, float blendDuration)
        {
            this._animator.CrossFadeInFixedTime(stateName, blendDuration, _baseLayerIndex);
            this._animator.CrossFadeInFixedTime(upperStatename, blendDuration, _upperBodyLayerIndex);
        }

        public void PlayAnimationImmediate(string stateName)
        {
            this._animator.Play(stateName, _baseLayerIndex);
            this._animator.Play(stateName, _upperBodyLayerIndex);
            // Update一下用来处理切换AnimatorController时，由于默认状态机会造成模型闪一下的问题
            this._animator.Update(0);
        }
        
        public void PlayAnimationsTogetherImmediate(string stateName, string upperStatename)
        {
            this._animator.Play(stateName, _baseLayerIndex);
            this._animator.Play(upperStatename, _upperBodyLayerIndex);
            // Update一下用来处理切换AnimatorController时，由于默认状态机会造成模型闪一下的问题
            this._animator.Update(0);
        }
       
        /// <summary>
        /// 添加动画播完回调事件
        /// </summary>
        /// <param name="stateName"></param>
        /// <param name="completeCallback"></param>
        public int AddAnimationCompleteEvent(string stateName, Action completeCallback, float blendDuration)
        {
            // 添加完成回调事件
            if (completeCallback != null)
            {
                AnimationClip clip = this._controller[stateName];    // 注意：这里的name是animation的名称(即State中的Motion名称)，不是State的名字
                if (clip != null)
                {
                    return this._eventHandler.AddEvent(clip, (clip.length>blendDuration ? clip.length-blendDuration : clip.length), completeCallback);
                }
            }

            return -1;
        }

        /// <summary>
        /// 添加动画事件
        /// </summary>
        /// <param name="stateName"></param>
        /// <param name="time"></param>
        /// <param name="callback"></param>
        public int AddAnimationEvent(string stateName, float time, Action callback)
        {
            // 添加完成回调事件
            if (callback != null)
            {
                AnimationClip clip = this._controller[stateName];    // 注意：这里的name是animation的名称(即State中的Motion名称)，不是State的名字
                if (clip != null)
                {
                    return this._eventHandler.AddEvent(clip, time, callback);
                }
            }

            return -1;
        }
        /// <summary>
        /// 添加动画事件
        /// </summary>
        /// <param name="stateName"></param>
        /// <param name="percent"></param>
        /// <param name="callback"></param>
        public int AddAnimationEventByPercent(string stateName, float percent, Action callback)
        {
            // 添加完成回调事件
            if (callback != null)
            {
                AnimationClip clip = this._controller[stateName];    // 注意：这里的name是animation的名称(即State中的Motion名称)，不是State的名字
                if (clip != null)
                {
                    return this._eventHandler.AddEvent(clip, clip.length*percent, callback);
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

