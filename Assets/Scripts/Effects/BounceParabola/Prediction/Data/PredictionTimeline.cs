using System.Collections.Generic;
using UnityEngine;

public class PredictionTimeline : List<PredictionCoordinate>
{
    public virtual void Add(Vector3 position, Quaternion rotation)
    {
        var coordinate = new PredictionCoordinate(position, rotation);

        Add(coordinate);
    }
}
