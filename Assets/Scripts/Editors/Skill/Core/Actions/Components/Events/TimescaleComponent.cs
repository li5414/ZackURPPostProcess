using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using UnityEngine;

namespace Skill
{
    // 技能事件的timescale组件
    [Description("Timescale缩放")]
    [JsonObject(MemberSerialization.OptIn)]
    public partial class TimescaleComponent : SkillComponent
    {
        // 时间缩放
        [JsonProperty]
        [Description("timescale")]
        public float timescale = 1f;

        [JsonProperty]
        [Description("animate")]
        public SkillAnimatorState state;

        TimescaleComponent()
        {
            this.needLoadRes = false;
        }

        public override void Execute(GameObject gameObject)
        {
            Debug.Log($"设置timescale:{timescale}");
            Time.timeScale = timescale;
        }
    } 
}

