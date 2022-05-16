using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using UnityEngine;

namespace Skill
{
    public class AnimatorLayerAttribute : Attribute
    {
        public int layer;

        public AnimatorLayerAttribute(int animLayer = 0)
        {
            this.layer = animLayer;
        }
    }
    
    /// <summary>
    /// 动画类型 (注意和AnimatorController同名)
    /// </summary>
    public enum SkillAnimatorState
    {
        [Description("Attack1"), AnimatorLayerAttribute(0)]
        Attack1 = 0,
        [Description("Attack2"), AnimatorLayerAttribute(0)]
        Attack2,
        [Description("Skill"), AnimatorLayerAttribute(0)]
        Skill,
    };
    
    public class Parameters
    {
        // 技能配置文件目录
        public static readonly string k_SkillConfigFilePath;
        // 角色预制体目录
        public static readonly string k_CharacterPrefabFilePath;
        public static readonly string k_CharacterPrefabAssetPath;
        // 技能配置ab包名
        public static readonly string k_SkillConfigBundleName = "skillConfig";
        
        static Parameters()
        {
            k_SkillConfigFilePath = Path.Combine(Application.dataPath, "_Resources/Configs/Skill");
            k_CharacterPrefabFilePath = Path.Combine(Application.dataPath, "_Resources/Prefabs/Character");
            k_CharacterPrefabAssetPath = Path.Combine("Assets", "_Resources/Prefabs/Character");
        }
    }
}
