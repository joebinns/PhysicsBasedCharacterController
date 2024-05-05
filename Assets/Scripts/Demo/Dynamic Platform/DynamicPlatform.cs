using UnityEngine;

namespace JoeBinns.Spacetime.Platform
{
	public class DynamicPlatform : MonoBehaviour
	{
		private Matrix4x4 _transformMatrix;
		private Matrix4x4 _deltaTransformMatrix;

		// FIXED UPDATE

		private void FixedUpdate()
		{
			var transformMatrix = transform.worldToLocalMatrix;
			_deltaTransformMatrix = Matrix4x4.Inverse(transformMatrix) * _transformMatrix;
			_transformMatrix = transformMatrix;
		}

		public void Follow(Transform transform)
		{
			transform.position = _deltaTransformMatrix.MultiplyPoint(transform.position);
			transform.rotation = _deltaTransformMatrix.rotation * transform.rotation;
		}
	}
}
