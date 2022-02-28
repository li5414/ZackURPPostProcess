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
        }
        
    }
}


