using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using UnityEngine;

namespace Skill
{
    // 技能组件基类
    [JsonObject(MemberSerialization.OptIn)]
    public abstract partial class SkillComponent
    {
        // 是否有资源须要加载
        [JsonProperty]
        protected bool needLoadRes = false;
    
        // 加载资源
        public virtual void LoadResource(Action callback) {}
        // 执行
        public virtual void Execute(GameObject gameObject) {}
    }
}
