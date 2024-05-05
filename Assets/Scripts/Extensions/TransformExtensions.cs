using UnityEngine;

namespace JoeBinns.Extensions.Transform
{
	public static class TransformExtensions
	{
		public static Vector3 TransformPointUnscaled(this UnityEngine.Transform transform, Vector3 position)
		{
			var localToWorldMatrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
			return localToWorldMatrix.MultiplyPoint3x4(position);
		}

		public static Vector3 InverseTransformPointUnscaled(this UnityEngine.Transform transform, Vector3 position)
		{
			var worldToLocalMatrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one).inverse;
			return worldToLocalMatrix.MultiplyPoint3x4(position);
		}
	}
}
