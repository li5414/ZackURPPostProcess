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

        static Parameters()
        {
            k_SkillConfigFilePath = Path.Combine(Application.dataPath, "_Resources/Configs/Skill");
        }
    }
}
