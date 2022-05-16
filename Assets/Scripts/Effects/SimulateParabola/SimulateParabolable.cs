using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulateParabolable : MonoBehaviour
{
    [SerializeField]
    float _Duration;
    private float _Timer;

    private Vector3[] _Points;
    
    void Awake()
    {
        _Timer = 0;
    }

    public void SetPoints(Vector3[] points)
    {
        _Points = points;
    }
    

    void Update()
    {
        _Timer += Time.deltaTime;
        float ratio = _Timer / _Duration;
        if (ratio > 1)
        {
            ratio = 1;
        }
        if (_Points != null)
        {
            Vector3 pos1 = this._Points[0];
            Vector3 pos2 = this._Points[0];
            float ratio_min = 0;
            float interval = 1f / (this._Points.Length - 1);
            for (int i = 0; i < this._Points.Length-1; ++i)
            {
                if ((interval * i <= ratio && ratio <= interval * (i + 1)) || (i == this._Points.Length - 2))
                {
                    pos1 = this._Points[i];
                    pos2 = this._Points[i + 1];
                    ratio_min = interval * i;
                    break;
                }
            }

            transform.position = Vector3.Lerp(pos1, pos2, (ratio - ratio_min)/interval);
        }
        
        
    }
    
}
