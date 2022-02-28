using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Utils;
using UnityEngine;

namespace Skill
{
    /// <summary>
    /// 技能管理
    /// 注意：技能使用之前需要确保技能资源加载完成(即LoadSkill)!!!
    /// </summary>
    public class SkillManager : Singleton<SkillManager>
    {
        class SkillConfigAsset : IDisposable
        {
            /// <summary>
            /// ab包名
            /// </summary>
            private string _BundleName;
            /// <summary>
            /// 资源名
            /// </summary>
            private string _AssetName;
            /// <summary>
            /// 文本内容
            /// </summary>
            private TextAsset _SkillTextAsset;

            /// <summary>
            /// 只读的SkillConfig
            /// </summary>
            private SkillConfig _SkillConfig;

            public SkillConfigAsset(string bundleName, string assetName, TextAsset asset)
            {
                this._BundleName = bundleName;
                this._AssetName = assetName;
                this._SkillTextAsset = asset;
            }

            public void Dispose()
            {
                TextAsset.Destroy(this._SkillTextAsset);
                // 卸载ab包
                ABLoader.GetInstance().UnloadAssetBundle(this._BundleName);
            }

            /// <summary>
            /// 获取SkillConfig，这个SkillConfig是只读的
            /// </summary>
            /// <returns></returns>
            public SkillConfig GetSkillConfig()
            {
                if (this._SkillConfig == null)
                {
                    this._SkillConfig = GenerateSkillConfig();
                }
                return this._SkillConfig;
            }

            /// <summary>
            /// 生成一个新的SkillConfig对象，用于释放技能
            /// </summary>
            /// <returns></returns>
            public SkillConfig GenerateSkillConfig()
            {
                return JsonUtils.DeserializeObject<SkillConfig>(this._SkillTextAsset.text);
            }
        }
        
        // 技能配置列表 <bundleName, <assetName, ConfigAssetObject>>
        private Dictionary<string, SkillConfigAsset> _SkillConfigAssets = new Dictionary<string, SkillConfigAsset>(); 
        // 技能事件列表
        private Dictionary<int, List<int>> _SkillEventIds = new Dictionary<int, List<int>>();
        // 当前运行技能标识生成器
        IdentityGenerator _SkillIDGenerator = new IdentityGenerator();

        /// <summary>
        /// 使用技能
        /// </summary>
        /// <param name="characterGO"></param>
        /// <param name="skillId"></param>
        /// <param name="completeCallback"></param>
        /// <returns></returns>
        public int UseSkill(GameObject characterGO, string skillId, Action completeCallback = null)
        {
            SkillConfigAsset skillConfigAsset;
            if (!this._SkillConfigAssets.TryGetValue(skillId, out skillConfigAsset))
            {
                Debug.LogError($"技能配置未提前加载即使用: {skillId}");
                return -1;
            }

            return UseSkill(characterGO, skillConfigAsset.GenerateSkillConfig(), completeCallback);
        }
        /// <summary>
        /// 使用技能
        /// 注意：这里的SkillConfig里面包含不同的运行时实例对象（如GameObject），所以每次释放技能都需要new一个
        /// </summary>
        /// <param name="characterGO"></param>
        /// <param name="skillConfig"></param>
        public int UseSkill(GameObject characterGO, SkillConfig skillConfig, Action completeCallback = null)
        {
            // 事件id列表
            List<int> eventIds = new List<int>();
            // 记录eventIds
            int id = this._SkillIDGenerator.GenerateId();
            this._SkillEventIds.Add(id, eventIds);
            // 技能参数
            SkillActionArguments args = new SkillActionArguments(id, characterGO);
            
            AnimationEventController eventController = args.eventController;
            // 添加onCompelete回调
            // 注意：最先调用是因为AnimationEventList是按照从Count-1到0的顺序执行事件回调数组的，所以onCompete应当是最后一个被调用的。不然技能会被stop，就打断了剩下的最后一帧的事件
            {
                List<SkillAnimationAction> actions = skillConfig.animations;
                if (actions.Count > 0)
                {
                    SkillAnimationAction action = actions[actions.Count - 1];
                    eventController.AddAnimationCompleteEvent(action.clipName, () =>
                    {
                        Debug.Log("=============onCompeleteEvent===============");
                        
                        // 干掉技能
                        StopSkill(characterGO, id);
                        
                        // 技能执行完成回调
                        completeCallback?.Invoke();
                    });
                }

            }
            // 处理特效
            {
                List<SkillEffectAction> actions = skillConfig.effects;
                for (int i = 0; i < actions.Count; ++i)
                {
                    SkillEffectAction action = actions[i];
                    action.RegisterAnimationEvent(args, eventIds);
                    Debug.Log($"在{action.startClipName}第{action.timelineData.start}帧添加Effect动画事件{i}");
                    Debug.Log($"在{action.endClipName}第{action.timelineData.end}帧添加Effect动画事件{i}");
                }
            }
            // 处理事件
            {
                List<SkillEventAction> actions = skillConfig.events;
                for (int i = 0; i < actions.Count; ++i)
                {
                    SkillEventAction action = actions[i];
                    action.RegisterAnimationEvent(args, eventIds);
                    Debug.Log($"在{action.clipName}第{action.timelineData.start}帧添加动画事件{i}");
                }
            }
            // 自定义事件
            {
                List<SkillCustomEventAction> actions = skillConfig.customEvents;
                for (int i = 0; i < actions.Count; ++i)
                {
                    SkillCustomEventAction action = actions[i];
                    action.RegisterAnimationEvent(args, eventIds);
                    Debug.Log($"在{action.clipName}第{action.timelineData.start}帧注册动画事件回调方法{action.functionName}");
                }
            }

            return id;
        }

        /// <summary>
        /// 打断已释放的技能
        /// </summary>
        /// <param name="eventController"></param>
        /// <param name="id"></param>
        public void StopSkill(GameObject characterGO, int id)
        {
            Debug.Log($"StopSkill = {id}");
            List<int> eventIds;
            if (this._SkillEventIds.TryGetValue(id, out eventIds))
            {
                AnimationEventController eventController = characterGO.GetComponent<AnimationEventController>();
                
                // 移除事件
                for (int i = 0; i < eventIds.Count; ++i)
                {
                    eventController.RemoveAnimationEvent(eventIds[i]);
                }
                // TODO:打断正在执行的事件...
                
                
                this._SkillEventIds.Remove(id);
            }
        }

        /// <summary>
        /// 加载技能资源
        /// </summary>
        /// <param name="skillId">技能id</param>
        /// <param name="callback">加载完成回调</param>
        public void LoadSkill(string skillId, Action callback)
        {
            string cfgBundleName = Parameters.k_SkillConfigBundleName;

            SkillConfigAsset skillConfigAsset;
            if (this._SkillConfigAssets.TryGetValue(skillId, out skillConfigAsset))
            {
                // 配置已加载
                LoadSkill(skillConfigAsset.GetSkillConfig(), callback);
            }
            else
            {
                // 配置未加载
                ABLoader.GetInstance().LoadAssetFromBundle(cfgBundleName, skillId, (loadedObject) =>
                {
                    TextAsset configTextAsset = loadedObject as TextAsset;
                    skillConfigAsset = new SkillConfigAsset(cfgBundleName, skillId, configTextAsset);
                    this._SkillConfigAssets.Add(skillId, skillConfigAsset);
                    // 加载资源
                    LoadSkill(skillConfigAsset.GetSkillConfig(), callback);
                });
            }
        }

        /// <summary>
        /// 加载技能资源
        /// </summary>
        /// <param name="skillConfig"></param>
        /// <param name="callback"></param>
        public void LoadSkill(SkillConfig skillConfig, Action callback)
        {
            // 需要加载资源的action数量
            int totalCount = skillConfig.effects.Count + skillConfig.events.Count;
            // 当前已加载的数量
            int finishCount = 0;
            // 处理特效
            {
                List<SkillEffectAction> actions = skillConfig.effects;
                for (int i = 0; i < actions.Count; ++i)
                {
                    SkillEffectAction action = actions[i];
                    action.LoadResource(() =>
                    {
                        finishCount++;
                        Debug.Log($"Load Skill Resource ({finishCount}/{totalCount}).");
                        if (finishCount >= totalCount)
                        {
                            callback?.Invoke();
                        }
                    });
                }
            }
            // 处理事件
            {
                List<SkillEventAction> actions = skillConfig.events;
                for (int i = 0; i < actions.Count; ++i)
                {
                    SkillEventAction action = actions[i];
                    action.LoadResource(() =>
                    {
                        finishCount++;
                        Debug.Log($"Load Skill Resource ({finishCount}/{totalCount}).");
                        if (finishCount >= totalCount)
                        {
                            callback?.Invoke();
                        }
                    });
                }
            }
        }

        /// <summary>
        /// 卸载技能资源
        /// </summary>
        /// <param name="skillId"></param>
        public void UnLoadSkill(string skillId)
        {
            SkillConfigAsset skillConfigAsset;
            if (this._SkillConfigAssets.TryGetValue(skillId, out skillConfigAsset))
            {
                UnLoadSkill(skillConfigAsset.GetSkillConfig());
            }
        }

        /// <summary>
        /// 卸载技能资源
        /// </summary>
        /// <param name="skillConfig"></param>
        public void UnLoadSkill(SkillConfig skillConfig)
        {
            // 处理特效
            {
                List<SkillEffectAction> actions = skillConfig.effects;
                for (int i = 0; i < actions.Count; ++i)
                {
                    SkillEffectAction action = actions[i];
                    action.UnLoadResource();
                }
            }
            // 处理事件
            {
                List<SkillEventAction> actions = skillConfig.events;
                for (int i = 0; i < actions.Count; ++i)
                {
                    SkillEventAction action = actions[i];
                    action.UnLoadResource();
                }
            }
        }
        
    };
    

}


