using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Plastic.Antlr3.Runtime.Misc;
using UnityEditor;
using UnityEngine;
using Zack.Editor;

namespace Skill.Editor
{
   public partial class SkillEditor
   {
        /// <summary>
        /// 读取配置
        /// </summary>
        /// <param name="filepath"></param>
        void ReadConfig(string filepath)
        {
            SkillConfig config = JsonUtils.DeserializeObjectFromFile<SkillConfig>(filepath);
            this._SkillConfig = config;

            this._Groups.Clear();
            // 解析SkillAnimationAction
            {
                var group = new SkillAnimationGroup();
                if (config.animations != null && config.animations.Count > 0)
                {
                    for (int i = 0; i < config.animations.Count; ++i)
                    {
                        group.actions.Add(config.animations[i]);

//                        if (i == 0)
//                        {
//                            {
//                                AnimatorTriggerParameter param = new AnimatorTriggerParameter();
//                                param.name = "attack1";
//                                config.animations[i].parameters.Add(param);
//                            }
//                            {
//                                AnimatorBoolParameter param = new AnimatorBoolParameter();
//                                param.name = "attack2";
//                                param.value = false;
//                                config.animations[i].parameters.Add(param);
//                            }
//                            {
//                                AnimatorIntParameter param = new AnimatorIntParameter();
//                                param.name = "skill";
//                                param.value = 1;
//                                config.animations[i].parameters.Add(param);
//                            }
//                        }
                        

                    }
                }
                this._Groups.Add(group);
            }
            // 解析SkillEffectAction
            {
                var group = new SkillEffectGroup();
                if (config.effects != null && config.effects.Count > 0)
                {
                    for (int i = 0; i < config.effects.Count; ++i)
                    {
                        group.actions.Add(config.effects[i]);
                    }
                }
                this._Groups.Add(group);
            }
            // 解析SkillEventAction
            {
                var group = new SkillEventGroup();
                if (config.events != null && config.events.Count > 0)
                {
                    for (int i = 0; i < config.events.Count; ++i)
                    {
                        group.actions.Add(config.events[i]);
                    }
                }
                this._Groups.Add(group);
            }
        }

        /// <summary>
        /// 存储配置
        /// </summary>
        void SaveConfig(string filepath)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filepath));
            JsonUtils.SerializeObjectToFile(this._SkillConfig, filepath);
            
            AssetDatabase.Refresh();
        }

        // 添加Action
        public void AddSkillAction(SkillActionType actionType)
        {
            switch (actionType)
            {
            case SkillActionType.Animation:
                {
                    SkillAnimationAction action = new SkillAnimationAction(0, 0);
                    this._Groups[(int)actionType].actions.Add(action);    
                }
                break;
            case SkillActionType.Effect:
                {
                    SkillEffectAction action = new SkillEffectAction(0, 0);
                    this._Groups[(int)actionType].actions.Add(action);    
                }
                break;
            case SkillActionType.Event:
                {
                    SkillEventAction action = new SkillEventAction(0);
                    this._Groups[(int)actionType].actions.Add(action);    
                }
                break;
            }
        }
      
   }
   

}

