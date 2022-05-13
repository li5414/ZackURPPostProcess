using System;
using UnityEngine;

public struct PredictionCoordinate
{
	public Vector3 Position { get; private set; }
	public Quaternion Rotation { get; private set; }

	public PredictionCoordinate(Vector3 position, Quaternion rotation)
	{
		this.Position = position;
		this.Rotation = rotation;
	}

	public static PredictionCoordinate From(Transform transform) => From(transform, Space.World);
	public static PredictionCoordinate From(Transform transform, Space space)
	{
        switch (space)
        {
            case Space.World:
				return new PredictionCoordinate(transform.position, transform.rotation);

            case Space.Self:
				return new PredictionCoordinate(transform.localPosition, transform.localRotation);
        }

		throw new NotImplementedException();
    }
}
