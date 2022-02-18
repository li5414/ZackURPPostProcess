using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace  Skill
{
    [JsonObject(MemberSerialization.OptIn)]
    public class SkillConfig
    {
        // 总时长
        [JsonProperty] 
        public int totalFrames;
        // 动画状态
        [JsonProperty]
        public SkillAnimatorState animatorState;
        // 动画action列表 (仅用作editor显示用)
        public List<SkillAnimationAction> animations;
        // 特效action列表
        [JsonProperty]
        public List<SkillEffectAction> effects;
        // 事件action列表
        [JsonProperty]
        public List<SkillEventAction> events;
    }

}


