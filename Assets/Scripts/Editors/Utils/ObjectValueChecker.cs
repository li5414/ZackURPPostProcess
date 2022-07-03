using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class ObjectValueChecker
{
    /// <summary>
    /// 遍历某个object去找指定的类型的值 (用于检查指定类型值是否合法)
    /// </summary>
    /// <param name="o"></param>
    /// <param name="callback"></param>
    /// <typeparam name="T"></typeparam>
    public static void RecurseObjectToCheckValue<T>(object o, System.Action<T> callback) where T : class
    {
        // 
        if (o == null)
        {
            return;
        }
     
        System.Type type = o.GetType();
        
        // 检测自己
        if (type == typeof(T))
        {
            callback?.Invoke(o as T);
            return;
        }
        
        // List类型
        if (type.IsGenericType     //判断是否是泛型
            && Array.IndexOf(type.GetInterfaces(), typeof(IEnumerable)) > -1)    //想判断这个list的类型 并遍历其中所有元素
        {
//            Type[] genericTypes = type.GetGenericArguments();
//            Console.WriteLine("泛型参数有:");
//            foreach (Type t in genericTypes)
//            {
//                Console.WriteLine(t.Name);
//            }

            IEnumerable enumerable = o as IEnumerable;
            foreach (object obj in enumerable)
            {
                RecurseObjectToCheckValue(obj, callback);                    
            }
        }
        else
        {
            // 遍历自身所有属性，如果不是指定类型，则继续向下递归遍历
            FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
            for (int i = 0; i < fieldInfos.Length; ++i)
            {
                var fieldInfo = fieldInfos[i];
                object value = fieldInfo.GetValue(o);
                T t = value as T;
                if (t != null)
                {
                    callback?.Invoke(t);
                }
                else if (o != null)
                {
                    RecurseObjectToCheckValue<T>(value, callback);
                }
            }
        }
        
        
    }
}
