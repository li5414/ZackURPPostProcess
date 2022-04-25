using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parabola : MonoBehaviour
{
    private LineRenderer _LineRenderer;

    private List<Vector3> _Points = new List<Vector3>();
    
    void Awake()
    {
        this._LineRenderer = this.GetComponent<LineRenderer>();

    }

    // Update is called once per frame
    void Update()
    {
        if (_Points.Count > 0)
        {
            _LineRenderer.SetVertexCount(_Points.Count);
            _LineRenderer.SetPositions(_Points.ToArray());
        }
        else
        {
            _LineRenderer.SetVertexCount(0);
        }
    }

    public void SetPoints(Vector3[] points)
    {
        _Points.Clear();
        _Points.AddRange(points);

    }
    
}
