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
            // 解析SkillCustomEventAction
            {
                var group = new SkillCustomEventGroup();;
                if (config.customEvents != null && config.customEvents.Count > 0)
                {
                    for (int i = 0; i < config.customEvents.Count; ++i)
                    {
                        group.actions.Add(config.customEvents[i]);
                    }
                }
                this._Groups.Add(group);
            }
            // 更新动画帧
            UpdateAnimationActions();
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
                AnimationClip clip = getAnimationClip(stateName);
                int frames = getAnimationStateFrames(clip);
                action.timelineData.start = totalFrames;
                action.timelineData.length = frames;
                action.clipName = clip.name;
                
                totalFrames += frames;
            }
            
            // 更新Config
            this._SkillConfig.totalFrames = totalFrames;

        }
        
        // 根据帧数获取动画状态
        string getAnimationStateName(int frame)
        {
            List<SkillAction> actions = this._Groups[(int) SkillActionType.Animation].actions;
            for (int i = actions.Count - 1; i >= 0; --i)
            {
                SkillAnimationAction action = actions[i] as SkillAnimationAction;
                if (action.timelineData.start <= frame && frame <= action.timelineData.end)
                {
                    return action.state.GetDescription();
                }
            }

            return ((SkillAnimatorState) 0).GetDescription();
        }

        // 根据帧数获取动画layer
        int getAnimationStateLayer(int frame)
        {
            List<SkillAction> actions = this._Groups[(int) SkillActionType.Animation].actions;
            for (int i = actions.Count - 1; i >= 0; --i)
            {
                SkillAnimationAction action = actions[i] as SkillAnimationAction;
                if (action.timelineData.start <= frame && frame <= action.timelineData.end)
                {
                    return EditorUtils.GetAnimatorLayer(action.state);
                }
            }

            return 0;
        }

        /// <summary>
        /// 存储配置
        /// </summary>
        void SaveConfig()
        {
            // 更新动画帧
            UpdateAnimationActions();
            
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
                    SkillEffectAction action = actions[i] as SkillEffectAction;
                    action.startClipName = getAnimationClip(getAnimationStateName(action.timelineData.start)).name;
                    action.endClipName = getAnimationClip(getAnimationStateName(action.timelineData.end)).name;
                    this._SkillConfig.effects.Add(action);
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
            // CustomEvent
            {
                this._SkillConfig.customEvents = new List<SkillCustomEventAction>();
                List<SkillAction> actions = this._Groups[(int) SkillActionType.CustomEvent].actions;
                for (int i = 0; i < actions.Count; ++i)
                {
                    SkillCustomEventAction action = actions[i] as SkillCustomEventAction;
                    action.clipName = getAnimationClip(getAnimationStateName(action.timelineData.start)).name;
                    this._SkillConfig.customEvents.Add(action);
                }
            }

            // 写入文件
            Directory.CreateDirectory(Path.GetDirectoryName(filepath));
            JsonUtils.SerializeObjectToFile(this._SkillConfig, filepath);

            Debug.Log($"保存配置完成: {filepath}");
            AssetDatabase.Refresh();
        }
        
        /// <summary>
        /// 新建配置
        /// </summary>
        /// <param name="skillId"></param>
        public void NewConfig(string skillId)
        {
            string filepath = Path.Combine(Parameters.k_SkillConfigFilePath, $"{this._SelectedCharacterID}/{skillId}.json");
            
            // 新建技能
            SkillConfig skillConfig = new SkillConfig();
            skillConfig.id = skillId;
            // Animation
            skillConfig.animations = new List<SkillAnimationAction>();
            // Effect
            skillConfig.effects = new List<SkillEffectAction>();
            // Event
            skillConfig.events = new List<SkillEventAction>();
            // CustomEvent
            skillConfig.customEvents = new List<SkillCustomEventAction>();

            // 写入文件
            Directory.CreateDirectory(Path.GetDirectoryName(filepath));
            JsonUtils.SerializeObjectToFile(skillConfig, filepath);

            Debug.Log($"保存配置完成: {filepath}");
            AssetDatabase.Refresh();
            
            // 更新技能列表
            this._SkillIDs = browserSkills();
        }
        
        // 删除配置
        void DeleteConfig()
        {
            string filepath = Path.Combine(Parameters.k_SkillConfigFilePath, $"{this._SelectedCharacterID}/{this._SelectedSkillID}.json");
            File.Delete(filepath);
            AssetDatabase.Refresh();

            Debug.Log($"删除配置完成: {filepath}");
            // 更新技能列表
            this._SkillIDs = browserSkills();
            // 重置
            this._SkillConfig = null;
            this._SelectedSkillID = null;
            this._Groups.Clear();
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
                    
                    // 更新动画帧
                    UpdateAnimationActions();
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
            case SkillActionType.CustomEvent:
            {
                SkillCustomEventAction action = new SkillCustomEventAction(0);
                this._Groups[(int)actionType].actions.Add(action);    
            }
                break;
            }
        }
      
   }
   

}

