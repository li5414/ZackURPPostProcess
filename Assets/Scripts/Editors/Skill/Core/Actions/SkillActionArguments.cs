using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace  Skill
{
    /// <summary>
    /// 技能Action参数
    /// </summary>
    public class SkillActionArguments
    {
        /// <summary>
        /// SkillManager使用技能的唯一id标识 (readonly)
        /// </summary>
        public int id { get; }
        
        /// <summary>
        /// 事件列表
        /// </summary>
        public List<int> eventIds { get; }

        /// <summary>
        /// 运行中的技能组件
        /// </summary>
        public HashSet<SkillComponent> runningSkillComponents;

        /// <summary>
        /// 角色GameObject
        /// </summary>
        public GameObject characterGameObject;

        /// <summary>
        /// 事件控制器
        /// </summary>
        public AnimationEventController eventController;

        public SkillActionArguments(int id, GameObject characterGO)
        {
            this.id = id;
            this.characterGameObject = characterGO;
            this.eventController = characterGO.GetComponent<AnimationEventController>();
            
            this.eventIds = new List<int>();
            this.runningSkillComponents = new HashSet<SkillComponent>();
        }
        
    }
}


