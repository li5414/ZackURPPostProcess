using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using Unity.Mathematics;
using UnityEngine;

namespace Skill
{
    // 技能事件的timescale组件
    [Description("Timescale缩放")]
    [JsonObject(MemberSerialization.OptIn)]
    public partial class TimescaleEvent : SkillComponent
    {
        // 时间缩放
        [JsonProperty]
        [Description("timescale")]
        [Range(0,2)]
        public float timescale = 1f;

        [JsonProperty]
        [Description("animation")]
        public SkillAnimatorState state;
        
        [JsonProperty, JsonConverter(typeof(Vector4Converter))]
        [Description("vector4")]
        public Vector4 vec4;
        
        [JsonProperty, JsonConverter(typeof(Vector3Converter))]
        [Description("vector3")]
        public Vector3 vec3;
        
        [JsonProperty, JsonConverter(typeof(Vector2Converter))]
        [Description("vector2")]
        public Vector2 vec2;

        [JsonProperty]
        [Description("functionName")]
        public string functionName;

        
        [JsonProperty]
        [Description("asset"), SkillAssetAttribute(SkillAssetType.AudioClip)]
        public SkillAsset asset;
        

        public TimescaleEvent()
        {
        }

        public override void OnStart(GameObject gameObject)
        {
            Debug.Log($"设置timescale:{timescale}");
//            Time.timeScale = timescale;
            Animator animator = gameObject.GetComponent<Animator>();
            animator.speed = timescale;
        }
    } 
}

