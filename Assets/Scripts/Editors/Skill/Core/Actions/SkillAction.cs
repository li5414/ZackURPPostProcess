using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using UnityEngine;

namespace Skill
{
    /// <summary>
    /// SkillAction类型枚举
    /// </summary>
    public enum SkillActionType
    {
        // 动画
        [Description("动画")]
        Animation = 0,
        // 特效
        [Description("特效")]
        Effect = 1,
        // 事件
        [Description("事件")]
        Event = 2,
        // 自定义回调
        [Description("自定义回调")]
        CustomEvent = 3,
    };
    
    /// <summary>
    /// Timeline数据
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class TimelineData
    {
        [JsonProperty]
        public int start;
        [JsonProperty]
        public int length;
        public int end
        {
            get { return this.start + this.length; }
        }

        public TimelineData() {}
        public TimelineData(int start, int length)
        {
            this.start = start;
            this.length = length;
        }

        public static bool operator ==(TimelineData v1, TimelineData v2)
        {
            bool isNull = v1==null && v2==null;
            if (isNull)
            {
                return true;
            }
            else
            {
                return v1.start == v2.start && v1.length == v2.length;
            }
        }

        public override bool Equals(object obj)
        {
            return this == (obj as TimelineData);
        }
    
        public static bool operator !=(TimelineData v1, TimelineData v2)
        {
            return !(v1 == v2);
        }
    }
    
    /// <summary>
    /// 技能数据
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class SkillAction
    {
        [JsonProperty]
        public TimelineData timelineData = new TimelineData();

        // 资源加载 (component的资源)
        public virtual void LoadResource(Action loadFinish)
        {
            loadFinish?.Invoke();
        }
        // 注册事件
        public virtual void RegisterAnimationEvent(AnimationEventController eventController, List<int> eventIds) {}
    }
    
    // 动画
    [JsonObject(MemberSerialization.OptIn)]
    public class SkillAnimationAction : SkillAction
    {
        [JsonProperty]
        public SkillAnimatorState state;
        [JsonProperty] 
        public string clipName;

        public SkillAnimationAction(int start, int length)
        {
            this.timelineData.start = start;
            this.timelineData.length = length;
        }
    }

    /// <summary>
    /// 特效数据
    /// </summary>
    public class SkillEffectAction : SkillAction
    {
        // 添加事件的AniamtionClip的名称 (注意这种有时长的可能会跨越两段动画)
        [JsonProperty]
        public string startClipName;
        [JsonProperty]
        public string endClipName;

        // 组件列表
        // 特效物体
        [JsonProperty]
        public PrefabEffect prefabEffect;
        
        public SkillEffectAction(int start, int length)
        {
            this.timelineData.start = start;
            this.timelineData.length = length;
        }
        
        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="eventController"></param>
        public override void RegisterAnimationEvent(AnimationEventController eventController, List<int> eventIds)
        {
            int eventId;
            // OnStart事件
            {
                eventId = eventController.AddAnimationEvent(this.startClipName, this.timelineData.start, ()=>{
                    Debug.Log($"============SkillEffectAction OnStart===========frame: {this.timelineData.start}====");
                    this.OnStart(eventController.gameObject);
                });
                eventIds.Add(eventId);
            }
            // OnEnd事件
            {
                eventId = eventController.AddAnimationEvent(this.endClipName, this.timelineData.end, ()=>{
                    Debug.Log($"============SkillEffectAction OnEnd===========frame: {this.timelineData.end}====");
                    this.OnEnd(eventController.gameObject);
                });
                eventIds.Add(eventId);
            }
        }

        protected void OnStart(GameObject gameObject)
        {
            if (this.prefabEffect != null)
            {
                this.prefabEffect.OnStart(gameObject);
            }
        }

        protected void OnEnd(GameObject gameObject)
        {
            if (this.prefabEffect != null)
            {
                this.prefabEffect.OnEnd(gameObject);
            }
        }
    }
    
    /// <summary>
    /// 事件数据
    /// </summary>
    public class SkillEventAction : SkillAction
    {
        // 添加事件的AniamtionClip的名称
        [JsonProperty]
        public string clipName;
        
        // 组件列表
        // timescale
        [JsonProperty]
        public TimescaleEvent timescaleEvent;

        public SkillEventAction(int start)
        {
            this.timelineData.start = start;
            this.timelineData.length = 0;
        }
        
        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="eventController"></param>
        public override void RegisterAnimationEvent(AnimationEventController eventController, List<int> eventIds)
        {
            int eventId = eventController.AddAnimationEvent(this.clipName, this.timelineData.start, ()=>{
                Debug.Log($"============SkillEventAction callback===========frame: {this.timelineData.start}====");
                this.Execute(eventController.gameObject);
            });
            eventIds.Add(eventId);
        }

        /// <summary>
        /// 执行事件
        /// </summary>
        /// <param name="gameObject"></param>
        public void Execute(GameObject gameObject)
        {
            // timescale event
            if (timescaleEvent != null)
            {
                timescaleEvent.OnStart(gameObject);
            }
        }

    }
    
    /// <summary>
    /// 自定义事件数据
    /// </summary>
    public class SkillCustomEventAction : SkillAction
    {
        // 添加事件的AniamtionClip的名称
        [JsonProperty]
        public string clipName;
        
        // 回调方法名
        [JsonProperty]
        public string functionName;
        
        public SkillCustomEventAction(int start)
        {
            this.timelineData.start = start;
            this.timelineData.length = 0;
        }
        
        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="eventController"></param>
        public override void RegisterAnimationEvent(AnimationEventController eventController, List<int> eventIds)
        {
            eventController.RegisiterAnimationEvent(this.clipName, this.timelineData.start, this.functionName);
        }

    }
    

}


