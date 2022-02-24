using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using Unity.Mathematics;
using UnityEngine;

namespace Skill
{
    // 绑定节点类型
    public enum BindNodeType
    {
        // 自己
        [Description("Root")]
        Root = 0,
        
    }
    
    // 技能特效的PrefabEffect组件
    [Description("特效物体")]
    [JsonObject(MemberSerialization.OptIn)]
    public partial class PrefabEffect : SkillComponent
    {
        // 特效物体
        [JsonProperty]
        [Description("特效"), SkillAssetAttribute(SkillAssetType.GameObject)]
        public SkillAsset effectObject;
        
        [JsonProperty]
        [Description("特效"), SkillAssetAttribute(SkillAssetType.GameObject)]
        public SkillAsset effectObject2;
        // 绑定节点
        [JsonProperty]
        [Description("绑定节点")]
        public BindNodeType bindNodeType;
        // 位置
        [JsonProperty, JsonConverter(typeof(Vector3Converter))]
        [Description("位置")]
        public Vector3 position;
        // 旋转
        [JsonProperty, JsonConverter(typeof(Vector3Converter))]
        [Description("旋转")]
        public Vector3 eulerAngles;
        // 缩放
        [JsonProperty, JsonConverter(typeof(Vector3Converter))]
        [Description("缩放")]
        public Vector3 scale;
        // 播放速度
        [JsonProperty]
        [Description("播放速度")]
        public float speed;

        
        public PrefabEffect()
        {
        }


        protected override void CheckSkillAsset()
        {
            if (this._Assets == null)
            {
                this._Assets = new List<SkillAsset>();
                this._Assets.Add(effectObject);
                this._Assets.Add(effectObject2);
            }
        }

        public override void OnStart(GameObject gameObject)
        {
            Debug.Log($"显示特效:{effectObject.bundleName}  -  {effectObject.assetName}");
        }

        public override void OnEnd(GameObject gameObject)
        {
            Debug.Log($"隐藏特效:{effectObject.bundleName}  -  {effectObject.assetName}");
        }
        
    } 
}

