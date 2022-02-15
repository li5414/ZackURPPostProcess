using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class JsonUtils
{
    /// <summary>
    /// 反序列化Json文件
    /// </summary>
    /// <param name="filepath"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T DeserializeObjectFromFile<T>(string filepath) where T : class
    {
        string jsonStr = File.ReadAllText(filepath);
        return DeserializeObject<T>(jsonStr);
    }

    /// <summary>
    /// 反序列化Json内容
    /// </summary>
    /// <param name="jsonStr"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T DeserializeObject<T>(string jsonStr) where T : class
    {
        return JsonConvert.DeserializeObject<T>(jsonStr);
    }

    /// <summary>
    /// 序列化Json文件
    /// </summary>
    /// <param name="data"></param>
    /// <param name="filepath"></param>
    /// <typeparam name="T"></typeparam>
    public static void SerializeObjectToFile<T>(T data, string filepath) where T : class
    {
//        JsonSerializerSettings jsetting = new JsonSerializerSettings();
//        jsetting.NullValueHandling = NullValueHandling.Ignore;
//        string jsonStr = JsonConvert.SerializeObject(data, Formatting.Indented, jsetting);
        string jsonStr = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(filepath, jsonStr);
    }
}
