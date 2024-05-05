using UnityEngine;

namespace JoeBinns.Utilities.Structs
{
	public struct OrientedPoint
	{
		public Vector3 Position;
		public Quaternion Rotation;

		public OrientedPoint(Vector3 position, Quaternion rotation)
		{
			Position = position;
			Rotation = rotation;
		}
	}
}
