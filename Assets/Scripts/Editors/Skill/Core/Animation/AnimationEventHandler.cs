using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Framework.Utils;

namespace Skill
{
    public class AnimationEventManager : Singleton<AnimationEventManager>
    {
        private Dictionary<AnimationClip, Dictionary<float, List<AnimationEvent>>> _AnimationEventsDict;

        public AnimationEventManager()
        {
            this._AnimationEventsDict = new Dictionary<AnimationClip, Dictionary<float, List<AnimationEvent>>>();
        }

        /// <summary>
        /// 给AnimationClip添加事件 (没有才会添加)
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="time"></param>
        /// <param name="functionName"></param>
        public void AddAnimationEvent(AnimationClip clip, float time, string functionName)
        {
            Dictionary<float, List<AnimationEvent>> eventDicts;
            if (!this._AnimationEventsDict.TryGetValue(clip, out eventDicts))
            {
                eventDicts = new Dictionary<float, List<AnimationEvent>>();
                this._AnimationEventsDict.Add(clip, eventDicts);
            }
            List<AnimationEvent> events;
            if (!eventDicts.TryGetValue(time, out events))
            {
                events = new List<AnimationEvent>();
                eventDicts.Add(time, events);
            }

            if (!containsFunc(events, functionName))
            {
                // AnimationEvent
                AnimationEvent evt = new AnimationEvent();
                // 回调参数
                evt.objectReferenceParameter = clip;
                evt.floatParameter = time;
                //事件挂载的时间位置。
                evt.time = time;
                //事件调用的函数名
                evt.functionName = functionName;
                //加入到clip中。有效期：播放结束
                clip.AddEvent(evt);
                
                Debug.Log($"==============AddAnimationEvent====={clip.name}==={time}=====");
                events.Add(evt);
            }
            else
            {
                Debug.Log($"====已存在==========AddAnimationEvent====={clip.name}==={time}=====");
            }
           
        }

        public void ClearAnimationEvents(AnimationClip clip)
        {
            Debug.Log($"==============ClearAnimationEvents================");
            clip.events = new AnimationEvent[0];   
//            UnityEditor.AnimationUtility.SetAnimationEvents(clip, new AnimationEvent[0]);
        }

        private bool containsFunc(List<AnimationEvent> events, string functionName)
        {
            for (int i = 0; i < events.Count; ++i)
            {
                AnimationEvent evt = events[i];
                if (evt.functionName == functionName)
                {
                    return true;
                }
            }

            return false;
        }

    }
        
    public class AnimationEventData
    {
        // 事件id
        public int eventID;
        // AnimationClip
        public AnimationClip clip;
        // 执行时间
        public float time;
        // 生成时间戳
        public float createTimestamp;
        // 事件回调
        public Action callback;

        public AnimationEventData(int id, AnimationClip animclip, float exetime, float stamp, Action func)
        {
            this.eventID = id;
            this.clip = animclip;
            this.time = exetime;
            this.createTimestamp = stamp;
            this.callback = func;
        }
    }

    /// <summary>
    /// 单个AnimationClip中所有事件回调列表
    /// </summary>
    public class AnimationEventList
    {
        Dictionary<float, List<AnimationEventData>> _eventsDict = new Dictionary<float, List<AnimationEventData>>();

        // 添加事件回调
        public void AddEvent(float time, AnimationEventData data)
        {
            List<AnimationEventData> events;
            if (!this._eventsDict.TryGetValue(time, out events))
            {
                events = new List<AnimationEventData>();
                this._eventsDict.Add(time, events);
            }
            events.Add(data);
        }
        // 执行
        public void ExecuteEvents(float time, Action<AnimationEventData> callback)
        {
            List<AnimationEventData> events;
            if (this._eventsDict.TryGetValue(time, out events))
            {
                // 注意callback可能会移除events中的元素，所以这里判断条件加一下每次判断元素数量
                for (int i = events.Count-1; events.Count>0 && i>=0 ; --i)
                {
                    AnimationEventData data = events[i];
                    events.RemoveAt(i);
                    callback(data);
                }
            }
        }
        // 移除事件回调
        public void RemoveEvent(AnimationEventData data)
        {
            List<AnimationEventData> events;
            if (this._eventsDict.TryGetValue(data.time, out events))
            {
                events.Remove(data);
            }
        }

        // 移除所有事件回调
        public void ClearAllEvents()
        {
            foreach (List<AnimationEventData> events in this._eventsDict.Values)
            {
                events.Clear();
            }
            this._eventsDict.Clear();
        }
    }

    public class AnimationEventHandler
    {
        // 动画事件回调
        private const string k_AnimationEventFunctionName = "onAnimationEvent";
        
        // 事件列表
        private Dictionary<int, AnimationEventData> _quickEvents = new Dictionary<int, AnimationEventData>();
        private Dictionary<AnimationClip, AnimationEventList> _allEvents = new Dictionary<AnimationClip, AnimationEventList>();
        // 事件ID
        IdentityGenerator _eventIDGenerator = new IdentityGenerator();
        // 待清除列表
        private List<int> _unusedEventIDs = new List<int>();

        /// <summary>
        /// 添加事件
        /// </summary>
        /// <param name="clip">动画片段</param>
        /// <param name="time">回调执行时间</param>
        /// <param name="action">回调事件</param>
        /// <param name="functionName">clip回调方法名称</param>
        public int AddEvent(AnimationClip clip, float time, Action action)
        {
            // 在clip中注册事件
            AnimationEventManager.GetInstance().AddAnimationEvent(clip, time, k_AnimationEventFunctionName);
            
            int id = _eventIDGenerator.GenerateId();
            // AnimationEventData
            AnimationEventData data = new AnimationEventData(id, clip, time, Time.realtimeSinceStartup, action);
            // _quickEvents
            this._quickEvents.Add(id, data);
            // _allEvents
            AnimationEventList eventList;
            if (!this._allEvents.TryGetValue(clip, out eventList))
            {
                eventList = new AnimationEventList();
                this._allEvents.Add(clip, eventList);
            }
            eventList.AddEvent(time, data);
            
            return id;
        }

        /// <summary>
        /// 执行事件
        /// </summary>
        /// <param name="id"></param>
        public void ExecuteEvent(AnimationClip clip, float time)
        {
            AnimationEventList eventList;
            if (this._allEvents.TryGetValue(clip, out eventList))
            {
                eventList.ExecuteEvents(time, (data) =>
                {
                    data.callback?.Invoke();
                    
                    // 移除
                    this._quickEvents.Remove(data.eventID);
                });
            }
        }

        /// <summary>
        /// 移除指定事件
        /// </summary>
        /// <param name="id"></param>
        public void RemoveEvent(int id)
        {
            AnimationEventData data;
            if (this._quickEvents.TryGetValue(id, out data))
            {
                AnimationEventList eventList;
                if (this._allEvents.TryGetValue(data.clip, out eventList))
                {
                    eventList.RemoveEvent(data);
                }
                
                this._quickEvents.Remove(id);
            }
        }

        /// <summary>
        /// 移除没用的事件 (根据时间戳来移除的，不能保证安全性)
        /// </summary>
        public void ClearUnusedEvents()
        {
            // 待清除列表
            this._unusedEventIDs.Clear();
            
            // 找出很久没有调用的回调事件id
            var itor = this._quickEvents.GetEnumerator();
            while (itor.MoveNext())
            {
                AnimationEventData data = itor.Current.Value;
                if (data.createTimestamp + data.clip.length * 5 <= Time.realtimeSinceStartup)
                {
                    this._unusedEventIDs.Add(data.eventID);
                }
            }
            
            // 清除
            if (this._unusedEventIDs.Count > 0)
            {
                Debug.Log("====ClearUnusedEvents=====");
                for (int i = 0; i < this._unusedEventIDs.Count; ++i)
                {
                    RemoveEvent(this._unusedEventIDs[i]);
                }
                this._unusedEventIDs.Clear();
            }

        }

        /// <summary>
        /// 移除所有事件
        /// </summary>
        public void ClearAllEvents()
        {
            foreach (var eventList in _allEvents.Values)
            {
                eventList.ClearAllEvents();   
            }
            this._allEvents.Clear();
            this._quickEvents.Clear();
        }

    }
}

