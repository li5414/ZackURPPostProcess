using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimelineData
{
    public string name;
    public int start;
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

public abstract class SkillAction
{
    public TimelineData timelineData = new TimelineData();
    public bool isMute = false;
}

public class SkillAnimationAction : SkillAction
{
    public string prefabPath;
    public string clipName;

    public SkillAnimationAction(int start, int length)
    {
        this.timelineData.start = start;
        this.timelineData.length = length;
    }
}

