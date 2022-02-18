using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Skill
{
    public class Parameters
    {
        // 技能配置文件目录
        public static readonly string k_SkillConfigFilePath;
        // 角色预制体目录
        public static readonly string k_CharacterPrefabFilePath;
        public static readonly string k_CharacterPrefabAssetPath;
        
        static Parameters()
        {
            k_SkillConfigFilePath = Path.Combine(Application.dataPath, "_Resources/Configs/Skill");
            k_CharacterPrefabFilePath = Path.Combine(Application.dataPath, "_Resources/Prefabs/Character");
            k_CharacterPrefabAssetPath = Path.Combine("Assets", "_Resources/Prefabs/Character");
        }
    }
}
