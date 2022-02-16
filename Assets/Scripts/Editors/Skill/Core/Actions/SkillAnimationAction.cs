using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace  Skill
{
    public enum AnimatorParameterType
    {
        Trigger = 0,
        Bool = 1,
        Int = 2,
        Float = 3,
    }
    
    /// <summary>
    /// Animator参数
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class AnimatorParameter
    {
        [JsonProperty]
        public AnimatorParameterType type;
        [JsonProperty]
        public string name;
        [JsonProperty] 
        public bool bvalue;
        [JsonProperty] 
        public int ivalue;
        [JsonProperty] 
        public float fvalue;

        public void Execute(Animator animator)
        {
            switch(type)
            {
            case AnimatorParameterType.Trigger:
                {
                    animator.SetTrigger(name);
                }
                break;
                     
            case AnimatorParameterType.Bool:
                {
                    animator.SetBool(name, bvalue);
                }
                break;
                     
            case AnimatorParameterType.Int:
                {
                    animator.SetInteger(name, ivalue);
                }
                break;
                     
            case AnimatorParameterType.Float:
                {
                    animator.SetFloat(name, fvalue);
                }
                break;
            }
        }
    }
    
    // 动画
    [JsonObject(MemberSerialization.OptIn)]
    public class SkillAnimationAction : SkillAction
    {
        [JsonProperty]
        public string prefabPath;
        [JsonProperty]
        public string clipName;

        // Animator参数
        [JsonProperty] 
        public List<AnimatorParameter> parameters = new List<AnimatorParameter>();

        public SkillAnimationAction(int start, int length)
        {
            this.timelineData.start = start;
            this.timelineData.length = length;
        }
    }

}


