using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Plastic.Antlr3.Runtime.Misc;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using Zack.Editor;

namespace Skill.Editor
{
   public partial class SkillEditor
   {
       /// <summary>
       /// 遍历人物
       /// </summary>
       /// <returns></returns>
       string[] browserCharacters()
       {
           string folderPath = Parameters.k_CharacterPrefabFilePath;
           
           List<string> folders = new List<string>();
           foreach (string path in Directory.GetDirectories(folderPath))
           {
               folders.Add(Path.GetFileName(path));
           }
           return folders.ToArray();
       }
       
       /// <summary>
       /// 遍历节能
       /// </summary>
       /// <returns></returns>
       string[] browserSkills()
       {
           string folderPath = Path.Combine(Parameters.k_SkillConfigFilePath, $"{this._SelectedCharacterID}");
           
           List<string> skillFiles = new List<string>();
           DirectoryInfo direction = new DirectoryInfo(folderPath);
           //文件夹下一层的所有子文件
           //SearchOption.TopDirectoryOnly：这个选项只取下一层的子文件
           //SearchOption.AllDirectories：这个选项会取其下所有的子文件
           FileInfo[] files = direction.GetFiles("*", SearchOption.TopDirectoryOnly);
           for(int i = 0; i < files.Length; i++)
           {
               if(files[i].Name.EndsWith(".json"))
               {
                   skillFiles.Add(files[i].Name);
               }
           }

           return skillFiles.ToArray();
       }
       
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
                this._Groups.Add(group);
                
                UpdateAnimationActions();
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

        // 更新动画帧数
        void UpdateAnimationActions()
        {
            string stateName = this._SkillConfig.animatorState.ToString();
            int frames = getAnimationStateFrames(stateName);
            
            // 更新Group
            Group group = this._Groups[(int) SkillActionType.Animation];
            group.actions.Clear();
            SkillAnimationAction action = new SkillAnimationAction(0, frames);
            action.stateName = stateName;
            group.actions.Add(action);
            // 更新Config
            this._SkillConfig.totalFrames = frames;

            // 播放动画
            bakeAnimation(stateName);
        }

        /// <summary>
        /// 存储配置
        /// </summary>
        void SaveConfig(string filepath)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filepath));
            // Animation
            {
                this._SkillConfig.animations = new List<SkillAnimationAction>();
                List<SkillAction> actions = this._Groups[(int) SkillActionType.Animation].actions;
                for (int i = 0; i < actions.Count; ++i)
                {
                    this._SkillConfig.animations.Add(actions[i] as SkillAnimationAction);
                }
            }
            // Effect
            {
                this._SkillConfig.effects = new List<SkillEffectAction>();
                List<SkillAction> actions = this._Groups[(int) SkillActionType.Effect].actions;
                for (int i = 0; i < actions.Count; ++i)
                {
                    this._SkillConfig.effects.Add(actions[i] as SkillEffectAction);
                }
            }
            // Event
            {
                this._SkillConfig.events = new List<SkillEventAction>();
                List<SkillAction> actions = this._Groups[(int) SkillActionType.Event].actions;
                for (int i = 0; i < actions.Count; ++i)
                {
                    this._SkillConfig.events.Add(actions[i] as SkillEventAction);
                }
            }
            
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

