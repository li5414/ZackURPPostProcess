using System;
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
        JsonSerializerSettings settings = new JsonSerializerSettings();
//        settings.NullValueHandling = NullValueHandling.Ignore;    // 忽略null值
        string jsonStr = JsonConvert.SerializeObject(data, Formatting.Indented, settings);
//        string jsonStr = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(filepath, jsonStr);
    }
}

/// <summary>
/// Newtonsoft.Json序列化扩展特性
/// </summary>
public class Vector4Converter : JsonConverter
{
    struct SampleVector4
    {
        public float x;
        public float y;
        public float z;
        public float w;
    }
    
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(Vector4);
    }
 
    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        return serializer.Deserialize<Vector4>(reader);;
    }
 
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        SampleVector4 cvalue;
        cvalue.x = ((Vector4) value).x;
        cvalue.y = ((Vector4) value).y;
        cvalue.z = ((Vector4) value).z;
        cvalue.w = ((Vector4) value).w;
        serializer.Serialize(writer, cvalue);
    }
}
public class Vector3Converter : JsonConverter
{
    struct SampleVector3
    {
        public float x;
        public float y;
        public float z;
    }
    
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(Vector3);
    }
 
    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        return serializer.Deserialize<Vector3>(reader);;
    }
 
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        SampleVector3 cvalue;
        cvalue.x = ((Vector3) value).x;
        cvalue.y = ((Vector3) value).y;
        cvalue.z = ((Vector3) value).z;
        serializer.Serialize(writer, cvalue);
    }
}
public class Vector2Converter : JsonConverter
{
    struct SampleVector2
    {
        public float x;
        public float y;
    }
    
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(Vector2);
    }
 
    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        return serializer.Deserialize<Vector2>(reader);;
    }
 
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        SampleVector2 cvalue;
        cvalue.x = ((Vector2) value).x;
        cvalue.y = ((Vector2) value).y;
        serializer.Serialize(writer, cvalue);
    }
}
