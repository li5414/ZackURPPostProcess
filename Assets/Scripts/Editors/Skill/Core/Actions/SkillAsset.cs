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
        // ab包
        [JsonProperty]
        public string bundleName;
        // 资源名
        [JsonProperty]
        public string assetName;
        
        // GUID (用于技能编辑时指定资源)
        [JsonProperty]
        public string guid;
        // 资源 (用于技能编辑时指定资源)
        public UnityEngine.Object mainObject;
        // 运行时资源
        public UnityEngine.Object runtimeObject;
    }
    
    

}


