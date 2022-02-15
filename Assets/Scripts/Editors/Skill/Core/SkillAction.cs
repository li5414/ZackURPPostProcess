using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[JsonObject(MemberSerialization.OptIn)]
public class TimelineData
{
    [JsonProperty]
    public int start;
    [JsonProperty]
    public int length;
    public int end
    {
        get { return this.start + this.length; }
    }

    public TimelineData() {}
    public TimelineData(int start, int length)
    {
        this.start = start;
        this.length = length;
    }

    public static bool operator ==(TimelineData v1, TimelineData v2)
    {
        bool isNull = v1==null && v2==null;
        if (isNull)
        {
            return true;
        }
        else
        {
            return v1.start == v2.start && v1.length == v2.length;
        }
    }

    public override bool Equals(object obj)
    {
        return this == (obj as TimelineData);
    }
    
    public static bool operator !=(TimelineData v1, TimelineData v2)
    {
        return !(v1 == v2);
    }
}

[JsonObject(MemberSerialization.OptIn)]
public class SkillConfig
{
    [JsonProperty]
    public List<SkillAnimationAction> animations;
}

[JsonObject(MemberSerialization.OptIn)]
public abstract class SkillAction
{
    [JsonProperty]
    public TimelineData timelineData = new TimelineData();
    public bool isMute = false;
}

[JsonObject(MemberSerialization.OptIn)]
public class SkillAnimationAction : SkillAction
{
    [JsonProperty]
    public string prefabPath;
    [JsonProperty]
    public string clipName;

    public SkillAnimationAction(int start, int length)
    {
        this.timelineData.start = start;
        this.timelineData.length = length;
    }
}
