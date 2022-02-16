using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Utils
{
    // 单例 class类型
    public class Singleton<T> where T : class, new()
    {
        private static T m_oInstance;

        public static T getInstance()
        {
            if (m_oInstance == null)
            {
                m_oInstance = new T();
            }
            
            return m_oInstance;
        }
    }

    // 单例 MonoBehaviour类型(会在场景中创建GameObject)
    public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T m_oInstance;

        public static T getInstance()
        {
            if (m_oInstance == null)
            {
                GameObject gameObject = new GameObject(typeof(T).Name);
                m_oInstance = gameObject.AddComponent<T>();

                DontDestroyOnLoad(gameObject);
            }

            return m_oInstance;
        }
    }

}

