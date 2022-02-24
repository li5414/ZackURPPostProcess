using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Framework.Utils;
using Newtonsoft.Json;
using UnityEngine;
using Object = System.Object;

namespace Skill
{
    // 技能对象池管理
    public class SkillObjectManager : MonoBehaviourSingleton<SkillObjectManager>
    {
        // 对象池
        private Dictionary<string, Dictionary<string, ObjectPool>> _ObjectPools = new Dictionary<string, Dictionary<string, ObjectPool>>();
        

        void Awake()
        {
            
        }
        
        /// <summary>
        /// 加载对象
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="assetName"></param>
        /// <param name="callback"></param>
        public void LoadResource(string bundleName, string assetName, Action callback)
        {
            Dictionary<string, ObjectPool> bundlePools;
            if (!this._ObjectPools.TryGetValue(bundleName, out bundlePools))
            {
                bundlePools = new Dictionary<string, ObjectPool>();
                this._ObjectPools.Add(bundleName, bundlePools);
            }

            ObjectPool pool;
            if (bundlePools.TryGetValue(assetName, out pool))
            {
                // 已加载
                pool.Retain();
                callback?.Invoke();
                return;
            }

            // 未加载
            ABLoader.GetInstance().LoadAssetFromBundle(bundleName, assetName, (obj) =>
            {
                pool = new ObjectPool(createPoolNode(bundleName, assetName), bundleName, assetName, obj as GameObject);
                pool.Retain();
                bundlePools.Add(assetName, pool);
//                callback?.Invoke();
                this.StartCoroutine(this.tttttt(callback));
            });

        }

        private int time = 1;
        IEnumerator tttttt(Action callback)
        {
            yield return new WaitForSeconds(time++);
            
            callback?.Invoke();
        }

        /// <summary>
        /// 卸载资源
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="assetName"></param>
        public void UnLoadResource(string bundleName, string assetName)
        {
            Dictionary<string, ObjectPool> bundlePools;
            if (!this._ObjectPools.TryGetValue(bundleName, out bundlePools))
            {
                Debug.LogWarning($"技能资源未加载: {bundleName}--{assetName}");
                return;
            }
            ObjectPool pool;
            if (!bundlePools.TryGetValue(assetName, out pool))
            {
                Debug.LogWarning($"技能资源未加载: {bundleName}--{assetName}");
                return;
            }
            
            pool.Release();
            if (pool.ReferenceCount <= 0)
            {
                bundlePools.Remove(assetName);
                GameObject.Destroy(pool.Transform.gameObject);
            }
        }

        /// <summary>
        /// 弹出空闲对象
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public GameObject PopObject(string bundleName, string assetName)
        {
            Dictionary<string, ObjectPool> bundlePools;
            if (!this._ObjectPools.TryGetValue(bundleName, out bundlePools))
            {
                Debug.LogError($"技能资源未提前加载即使用: {bundleName}--{assetName}");
                return null;
            }
            ObjectPool pool;
            if (!bundlePools.TryGetValue(assetName, out pool))
            {
                Debug.LogError($"技能资源未提前加载即使用: {bundleName}--{assetName}");
                return null;
            }

            return pool.Pop();
        }

        /// <summary>
        /// 回收对象
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="assetName"></param>
        /// <param name="gameObject"></param>
        public void PushObject(string bundleName, string assetName, GameObject gameObject)
        {
            Dictionary<string, ObjectPool> bundlePools;
            if (!this._ObjectPools.TryGetValue(bundleName, out bundlePools))
            {
                Debug.LogError($"技能资源未提前加载即使用: {bundleName}--{assetName}");
                return;
            }
            ObjectPool pool;
            if (!bundlePools.TryGetValue(assetName, out pool))
            {
                Debug.LogError($"技能资源未提前加载即使用: {bundleName}--{assetName}");
                return;
            }
            
            pool.Push(gameObject);
        }
        

        // 创建对象池节点
        private Transform createPoolNode(string bundleName, string assetName)
        {
            GameObject nodeObject = new GameObject();
            Transform nodeTransform = nodeObject.transform;
            nodeTransform.parent = this.transform;
            return nodeTransform;
        }
        
        
    }
    

}

public class ABLoader : Singleton<ABLoader>
{
    public void LoadAssetFromBundle(string bundleName, string assetName, Action<object> act)
    {
        act?.Invoke(new GameObject(assetName));
    }
    
    public static void AddTracker(GameObject go, string bundleName) {}
}


