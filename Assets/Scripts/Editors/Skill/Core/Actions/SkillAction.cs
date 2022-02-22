using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using UnityEngine;

namespace Skill
{
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
    
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class SkillAction
    {
        [JsonProperty]
        public TimelineData timelineData = new TimelineData();

        public virtual void LoadResourceEditor(Action loadFinish)
        {
            loadFinish?.Invoke();
        }
        public virtual void LoadResource(Action loadFinish)
        {
            loadFinish?.Invoke();
        }
    }

    // 特效
    public class SkillEffectAction : SkillAction
    {
        [JsonProperty]
        public string prefabPath;

        // 特效资源
        public GameObject mainObject;
        [JsonProperty]
        public string guid;
        
        public SkillEffectAction(int start, int length)
        {
            this.timelineData.start = start;
            this.timelineData.length = length;
        }
    }
    
    // 事件
    public class SkillEventAction : SkillAction
    {
        // 添加事件的AniamtionClip的名称
        [JsonProperty]
        public string clipName;
        
        // 组件列表
        // timescale
        [JsonProperty]
        public TimescaleComponent timescaleComponent;

        public SkillEventAction(int start)
        {
            this.timelineData.start = start;
            this.timelineData.length = 0;
        }

        /// <summary>
        /// 执行事件
        /// </summary>
        /// <param name="gameObject"></param>
        public void Execute(GameObject gameObject)
        {
            if (timescaleComponent != null)
            {
                timescaleComponent.Execute(gameObject);
            }
        }

    }
    
    // 自定义事件
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

    }
    

}


