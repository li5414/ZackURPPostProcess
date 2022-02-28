using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using UnityEngine;

namespace Skill
{
    // 绑定节点类型
    public enum BindNodeType
    {
        // 根节点
        [Description("根节点")]
        Root = 0,
        
    }
    
    // 技能的PrefabComponent组件
    [Description("预制体组件")]
    [JsonObject(MemberSerialization.OptIn)]
    public class PrefabComponent : SkillComponent
    {
        // 特效物体
        [JsonProperty]
        [Description("预制体"), SkillAssetAttribute(SkillAssetType.GameObject)]
        public SkillAsset prefabObject;
        // 绑定节点
        [JsonProperty]
        [Description("绑定节点")]
        public BindNodeType bindNodeType;
        // 位置
        [JsonProperty, JsonConverter(typeof(Vector3Converter))]
        [Description("位置")]
        public Vector3 position;
        // 旋转
        [JsonProperty, JsonConverter(typeof(Vector3Converter))]
        [Description("旋转")]
        public Vector3 eulerAngles;
        // 缩放
        [JsonProperty, JsonConverter(typeof(Vector3Converter))]
        [Description("缩放")]
        public Vector3 scale;


        protected override void CheckSkillAsset()
        {
            if (this._Assets == null)
            {
                this._Assets = new List<SkillAsset>();
                this._Assets.Add(prefabObject);
            }
        }
        
        // 运行时绑定GameObject并设置Transform
        protected void BindGameObject(GameObject characterObject)
        {
            // 创建object
            GameObject gameObject = SkillObjectManager.GetInstance().PopObject(this.prefabObject.bundleName, this.prefabObject.assetName);
            this.BindGameObject(characterObject, gameObject);
        }
        public void BindGameObject(GameObject characterObject, GameObject gameObject)
        {
            Transform transform = gameObject.transform;
            // 设置父节点
            switch (this.bindNodeType)
            {
            case BindNodeType.Root:
                {
                    // 根节点
                    transform.parent = characterObject.transform;
                }
                break;
            }
            // 设置Transform
            transform.localPosition = this.position;
            transform.localEulerAngles = this.eulerAngles;
            transform.localScale = this.scale;
            // 激活
            gameObject.SetActive(true);

            this.prefabObject.runtimeObject = gameObject;
        }
        // 运行时解绑GameObject
        protected void UnBindGameObject()
        {
            if (this.prefabObject.runtimeObject != null)
            {
                SkillObjectManager.GetInstance().PushObject(this.prefabObject.bundleName, this.prefabObject.assetName, this.prefabObject.runtimeObject as GameObject);
                this.prefabObject.runtimeObject = null;
            }
        }

        
    } 
}
