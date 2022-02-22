using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using UnityEngine;

namespace Skill
{
    public enum SkillAssetType
    {
        // GameObject
        [Description("GameObject")]
        GameObject = 0,
        // AudioClip
        [Description("AudioClip")]
        AudioClip,
    };

    public class SkillAssetAttribute : Attribute
    {
        public SkillAssetType type
        {
            get; 
            private set;
        }

        public SkillAssetAttribute(SkillAssetType type)
        {
            this.type = type;
        }
    };
    
    // 技能资源数据
    [JsonObject(MemberSerialization.OptIn)]
    public class SkillAsset
    {
        // 资源类型
        [JsonProperty]
        public SkillAssetType type;
        // GUID
        [JsonProperty]
        public string guid;
        // ab包
        public string bundleName;
        // 资源名
        public string assetName;
        // 资源
        public UnityEngine.Object mainObject;
    }
    
    

}


