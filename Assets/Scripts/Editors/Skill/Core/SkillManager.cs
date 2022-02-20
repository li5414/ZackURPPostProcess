using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Utils;
using Newtonsoft.Json;
using UnityEngine;

namespace Skill
{
    public class SkillManager : Singleton<SkillManager>
    {
        /// <summary>
        /// 使用技能
        /// </summary>
        /// <param name="eventController"></param>
        /// <param name="skillConfig"></param>
        public void UseSkill(AnimationEventController eventController, SkillConfig skillConfig)
        {
            // 处理事件
            {
                List<SkillEventAction> actions = skillConfig.events;
                for (int i = 0; i < actions.Count; ++i)
                {
                    SkillEventAction action = actions[i];
                    eventController.AddAnimationEvent(action.clipName, action.timelineData.start, ()=>{
                        Debug.Log($"============SkillEventAction callback===========frame: {action.timelineData.start}====");
                    });
                    Debug.Log($"在{action.clipName}第{action.timelineData.start}帧添加动画事件{i}");
                }
            }
            
        }
        
    };
    

}


