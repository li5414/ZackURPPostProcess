using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Utils;
using Newtonsoft.Json;
using UnityEngine;

namespace Skill
{
    // 对象池
    public class ObjectPool
    {
        // ab包名
        private string _BundleName;
        // 资源名
        private string _AssetName;
        // 原始对象
        private GameObject _MainObject;
        // 对象池父节点
        public Transform Transform
        {
            get => this._Transform;
        }
        private Transform _Transform;
        // 对象池
        private List<GameObject> _Pool;
        // 引用计数
        public int ReferenceCount
        {
            get => this._ReferenceCount;
        }
        private int _ReferenceCount;

        public ObjectPool(Transform node, string bundleName, string assetName, GameObject mainObject)
        {
            this._Pool = new List<GameObject>();
            this._ReferenceCount = 0;
            
            this._Transform = node;
            this._BundleName = bundleName;
            this._AssetName = assetName;
            this._MainObject = mainObject;
            updateNodeName();
            
            // 注意：ab包的卸载和对象池GameObject绑定
            ABLoader.AddTracker(node.gameObject, this._BundleName);
        }

        public void Retain()
        {
            this._ReferenceCount++;
            updateNodeName();
        }

        public void Release()
        {
            this._ReferenceCount--;
            updateNodeName();
        }
        

        /// <summary>
        /// 弹出对象
        /// </summary>
        /// <returns></returns>
        public GameObject Pop()
        {
            GameObject gameObject = null;
            
            if (this._Pool.Count > 0)
            {
                int index = this._Pool.Count - 1;
                gameObject = this._Pool[index];
                this._Pool.RemoveAt(index);
                return gameObject;
            }
            if (this._MainObject != null)
            {
                gameObject = GameObject.Instantiate(this._MainObject, Vector3.zero, Quaternion.identity);
            }

            return gameObject;
        }

        /// <summary>
        /// 弹出对象
        /// </summary>
        /// <param name="gameObject"></param>
        public void Push(GameObject gameObject)
        {
            gameObject.transform.parent = this._Transform;
            gameObject.SetActive(false);
            this._Pool.Add(gameObject);
        }
        
        // 更新节点名称
        private void updateNodeName()
        {
#if UNITY_EDITOR
            this._Transform.name = $"{this._BundleName}--{this._AssetName}: {this._ReferenceCount}";
#endif
        }
        
    }
    

}


