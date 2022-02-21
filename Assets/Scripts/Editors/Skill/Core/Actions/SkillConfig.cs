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
        // id
        [JsonProperty]
        public string id;
        // 总时长
        [JsonProperty] 
        public int totalFrames;
        // 动画action列表
        [JsonProperty] 
        public List<SkillAnimationAction> animations;
        // 特效action列表
        [JsonProperty]
        public List<SkillEffectAction> effects;
        // 事件action列表
        [JsonProperty]
        public List<SkillEventAction> events;
    }

}


