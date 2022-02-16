using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Utils;
using Newtonsoft.Json;
using UnityEngine;

namespace  Skill
{
    public class SkillManager : MonoBehaviourSingleton<SkillManager>
    {
        // 当前正在执行的技能
        private List<SkillExecutor> _Executors;
        
        
    };
    

}


