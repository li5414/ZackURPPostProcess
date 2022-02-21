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
                   skillFiles.Add(files[i].Name.Replace(".json", ""));
               }
           }

           return skillFiles.ToArray();
       }
       
        /// <summary>
        /// 读取配置
        /// </summary>
        /// <param name="filepath"></param>
        void ReadConfig()
        {
            string filepath = Path.Combine(Parameters.k_SkillConfigFilePath, $"{this._SelectedCharacterID}/{this._SelectedSkillID}.json");
            
            SkillConfig config = JsonUtils.DeserializeObjectFromFile<SkillConfig>(filepath);
            this._SkillConfig = config;
            if (this._SkillConfig.id == null || this._SkillConfig.id == String.Empty)
            {
                this._SkillConfig.id = this._SelectedSkillID;
            }

            this._Groups.Clear();
            // 解析SkillAnimationAction
            {
                var group = new SkillAnimationGroup();
                if (config.animations != null && config.animations.Count > 0)
                {
                    for (int i = 0; i < config.animations.Count; ++i)
                    {
                        group.actions.Add(config.animations[i]);
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
            
            Debug.Log($"读取配置完成: {filepath}");
        }

        // 更新动画帧数
        void UpdateAnimationActions()
        {
            List<SkillAction> actions = this._Groups[(int) SkillActionType.Animation].actions;
            int totalFrames = 0;
            for (int i = 0; i < actions.Count; ++i)
            {
                SkillAnimationAction action = actions[i] as SkillAnimationAction;
                string stateName = action.state.GetDescription();
                int frames = getAnimationStateFrames(stateName);
                action.timelineData.start = totalFrames;
                action.timelineData.length = frames;
                
                totalFrames += frames;
            }
            
            // 更新Config
            this._SkillConfig.totalFrames = totalFrames;

        }
        
        // 根据帧数获取动画状态
        string getAnimationStateName(int frame)
        {
            List<SkillAction> actions = this._Groups[(int) SkillActionType.Animation].actions;
            for (int i = actions.Count-1; i>=0; --i)
            {
                SkillAnimationAction action = actions[i] as SkillAnimationAction;
                if (action.timelineData.start <= frame && frame <= action.timelineData.end)
                {
                    return action.state.GetDescription();
                }
            }

            return ((SkillAnimatorState) 0).GetDescription();
        }
        
        /// <summary>
        /// 存储配置
        /// </summary>
        void SaveConfig()
        {
            string filepath = Path.Combine(Parameters.k_SkillConfigFilePath, $"{this._SelectedCharacterID}/{this._SelectedSkillID}.json");
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
                    SkillEventAction action = actions[i] as SkillEventAction;
                    action.clipName = getAnimationClip(getAnimationStateName(action.timelineData.start)).name;
                    this._SkillConfig.events.Add(action);
                }
            }

            // 写入文件
            Directory.CreateDirectory(Path.GetDirectoryName(filepath));
            JsonUtils.SerializeObjectToFile(this._SkillConfig, filepath);

            Debug.Log($"保存配置完成: {filepath}");
            AssetDatabase.Refresh();
        }

        // 添加Action
        public void AddSkillAction(SkillActionType actionType)
        {
            switch (actionType)
            {
            case SkillActionType.Animation:
                {
                    SkillAnimatorState state = SkillAnimatorState.Attack1;
                    AnimationClip clip = getAnimationClip(state.GetDescription());
                    int frames = getAnimationStateFrames(clip);
                    SkillAnimationAction action = new SkillAnimationAction(0, frames);
                    action.state = state;
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

