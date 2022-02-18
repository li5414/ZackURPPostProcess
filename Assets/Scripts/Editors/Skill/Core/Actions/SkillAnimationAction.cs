using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace  Skill
{
    /// <summary>
    /// 动画类型 (注意和AnimatorController同名)
    /// </summary>
    public enum SkillAnimatorState
    {
        Attack1 = 0,
        Attack2,
        Skill,
    };
    
    
    // 动画
    [JsonObject(MemberSerialization.OptIn)]
    public class SkillAnimationAction : SkillAction
    {
        [JsonProperty]
        public string stateName;

        public SkillAnimationAction(int start, int length)
        {
            this.timelineData.start = start;
            this.timelineData.length = length;
        }
    }

}


