using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using Unity.Mathematics;
using UnityEngine;

namespace Skill
{
    // 技能特效的PrefabEffect组件
    [Description("特效物体")]
    [JsonObject(MemberSerialization.OptIn)]
    public class PrefabEffect : PrefabComponent
    {
       // 播放速度
        [JsonProperty]
        [Description("播放速度")]
        public float speed;

        
        public PrefabEffect()
        {
        }

        public override void OnStart(GameObject characterObject)
        {
            Debug.Log($"显示特效:{this.prefabObject.bundleName}  -  {this.prefabObject.assetName}");
            BindGameObject(characterObject);
            // 设置speed
            SkillUtils.SetEffectSpeed(this.prefabObject.mainObject as GameObject, this.speed);
        }

        public override void OnEnd(GameObject characterObject)
        {
            Debug.Log($"隐藏特效:{this.prefabObject.bundleName}  -  {this.prefabObject.assetName}");
            UnBindGameObject();
        }
        
    } 
}

