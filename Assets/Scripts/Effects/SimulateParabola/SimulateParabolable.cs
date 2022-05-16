using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulateParabolable : MonoBehaviour
{
    private float _Timer;

    private float _Interval;
    private Vector3[] _Points;

    void Awake()
    {
        _Timer = 0;
    }

    public void SetPoints(float interval, Vector3[] points, float speed = 1f)
    {
        _Interval = interval / speed;
        _Points = points;
    }


    void Update()
    {
        _Timer += Time.deltaTime;
        
        int startIdx = (int) (_Timer / _Interval);
        startIdx = Math.Min(startIdx, _Points.Length - 1);
        int endIdx = startIdx + 1;
        endIdx = Math.Min(endIdx, _Points.Length - 1);
        float percent = (_Timer - _Interval * startIdx) / _Interval;
        percent = Math.Min(percent, 1);
        Vector3 pos1 = this._Points[startIdx];
        Vector3 pos2 = this._Points[endIdx];
        transform.position = Vector3.Lerp(pos1, pos2, percent);

        // float ratio = _Timer / _Duration;
        // if (ratio > 1)
        // {
        //     ratio = 1;
        // }
        // if (_Points != null)
        // {
        //     Vector3 pos1 = this._Points[0];
        //     Vector3 pos2 = this._Points[0];
        //     float ratio_min = 0;
        //     float interval = 1f / (this._Points.Length - 1);
        //     for (int i = 0; i < this._Points.Length-1; ++i)
        //     {
        //         if ((interval * i <= ratio && ratio <= interval * (i + 1)) || (i == this._Points.Length - 2))
        //         {
        //             pos1 = this._Points[i];
        //             pos2 = this._Points[i + 1];
        //             ratio_min = interval * i;
        //             break;
        //         }
        //     }
        //
        //     transform.position = Vector3.Lerp(pos1, pos2, (ratio - ratio_min)/interval);
        // }


    }
    
}
