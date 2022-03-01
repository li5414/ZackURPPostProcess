using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using UnityEngine;

namespace Skill
{
    // 技能组件基类
    [JsonObject(MemberSerialization.OptIn)]
    public abstract partial class SkillComponent
    {
        // 资源列表 (用于加载和卸载资源)
        protected List<SkillAsset> _Assets;
        
        // 检测技能资源
        protected virtual void CheckSkillAsset() {}
        // 加载资源
        public virtual void LoadResource(Action callback)
        {
            // 检测技能资源
            CheckSkillAsset();
            
            // 加载
            if (this._Assets != null && this._Assets.Count > 0)
            {
                int totalCount = this._Assets.Count;
                int finishCount = 0;
                for (int i = 0; i < totalCount; ++i)
                {
                    SkillAsset asset = this._Assets[i];
                    SkillObjectManager.GetInstance().LoadResource(asset.bundleName, asset.assetName, () =>
                    {
                        finishCount++;
#if UNITY_EDITOR
                        Debug.Log($" {this.GetType().Name} SkillAsset Loading {finishCount}/{totalCount}.");
#endif
                        if (finishCount >= totalCount)
                        {
                            callback?.Invoke();
                        }
                    }, asset.guid);
                }
            }
            else
            {
                callback?.Invoke();
            }
            
        }
        // 卸载资源
        public virtual void UnLoadResource()
        {
            // 卸载
            if (this._Assets != null && this._Assets.Count > 0)
            {
                int totalCount = this._Assets.Count;
                for (int i = 0; i < totalCount; ++i)
                {
                    SkillAsset asset = this._Assets[i];
                    SkillObjectManager.GetInstance().UnLoadResource(asset.bundleName, asset.assetName);
                }
            }
        }
        
        
        // 开始 (SkillEventAction只需调用此接口)
        public virtual void OnStart(SkillActionArguments args)
        {
            args.runningSkillComponents.Add(this);
        }
        // 结束
        public virtual void OnEnd(SkillActionArguments args)
        {
            args.runningSkillComponents.Remove(this);
        }
        // 打断
        public virtual void OnStop(SkillActionArguments args) {}
    }
}
