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
        // 技能事件列表
        private Dictionary<int, List<int>> _SkillEventIds = new Dictionary<int, List<int>>();
        // 当前运行技能标识生成器
        IdentityGenerator _SkillIDGenerator = new IdentityGenerator();
        
        /// <summary>
        /// 使用技能
        /// </summary>
        /// <param name="eventController"></param>
        /// <param name="skillConfig"></param>
        public void UseSkill(AnimationEventController eventController, SkillConfig skillConfig, Action completeCallback = null)
        {
            // 事件id列表
            List<int> eventIds = new List<int>();
            // 记录eventIds
            int id = this._SkillIDGenerator.GenerateId();
            this._SkillEventIds.Add(id, eventIds);
            
            // 添加onCompelete回调
            // 注意：最先调用是因为AnimationEventList是按照从Count-1到0的顺序执行事件回调数组的，所以onCompete应当是最后一个被调用的。不然技能会被stop，就打断了剩下的最后一帧的事件
            {
                List<SkillAnimationAction> actions = skillConfig.animations;
                if (actions.Count > 0)
                {
                    SkillAnimationAction action = actions[actions.Count - 1];
                    eventController.AddAnimationCompleteEvent(action.clipName, () =>
                    {
                        Debug.Log("=============onCompeleteEvent===============");
                        
                        // 干掉技能
                        StopSkill(eventController, id);
                        
                        // 技能执行完成回调
                        completeCallback?.Invoke();
                    });
                }

            }
            // 处理特效
            {
                List<SkillEffectAction> actions = skillConfig.effects;
                for (int i = 0; i < actions.Count; ++i)
                {
                    SkillEffectAction action = actions[i];
                    action.RegisterAnimationEvent(eventController, eventIds);
                    Debug.Log($"在{action.startClipName}第{action.timelineData.start}帧添加Effect动画事件{i}");
                    Debug.Log($"在{action.endClipName}第{action.timelineData.end}帧添加Effect动画事件{i}");
                }
            }
            // 处理事件
            {
                List<SkillEventAction> actions = skillConfig.events;
                for (int i = 0; i < actions.Count; ++i)
                {
                    SkillEventAction action = actions[i];
                    action.RegisterAnimationEvent(eventController, eventIds);
                    Debug.Log($"在{action.clipName}第{action.timelineData.start}帧添加动画事件{i}");
                }
            }
            // 自定义事件
            {
                List<SkillCustomEventAction> actions = skillConfig.customEvents;
                for (int i = 0; i < actions.Count; ++i)
                {
                    SkillCustomEventAction action = actions[i];
                    action.RegisterAnimationEvent(eventController, eventIds);
                    Debug.Log($"在{action.clipName}第{action.timelineData.start}帧注册动画事件回调方法{action.functionName}");
                }
            }
        }

        /// <summary>
        /// 打断已释放的技能
        /// </summary>
        /// <param name="eventController"></param>
        /// <param name="id"></param>
        public void StopSkill(AnimationEventController eventController, int id)
        {
            Debug.Log($"StopSkill = {id}");
            List<int> eventIds;
            if (this._SkillEventIds.TryGetValue(id, out eventIds))
            {
                // 移除事件
                for (int i = 0; i < eventIds.Count; ++i)
                {
                    eventController.RemoveAnimationEvent(eventIds[i]);
                }
                
                this._SkillEventIds.Remove(id);
            }
        }
        
        
    };
    

}


