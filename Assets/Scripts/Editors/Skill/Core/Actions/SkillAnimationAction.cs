using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using UnityEngine;

namespace  Skill
{
    /// <summary>
    /// 动画类型 (注意和AnimatorController同名)
    /// </summary>
    public enum SkillAnimatorState
    {
        [Description("Attack1")]
        Attack1 = 0,
        [Description("Attack2")]
        Attack2,
        [Description("Skill")]
        Skill,
    };
    
    
    // 动画
    [JsonObject(MemberSerialization.OptIn)]
    public class SkillAnimationAction : SkillAction
    {
        [JsonProperty]
        public SkillAnimatorState state;

        public SkillAnimationAction(int start, int length)
        {
            this.timelineData.start = start;
            this.timelineData.length = length;
        }
    }

}


