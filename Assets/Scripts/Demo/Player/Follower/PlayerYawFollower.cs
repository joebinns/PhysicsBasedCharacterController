using UnityEngine;

namespace JoeBinns.Spacetime.Player.Follower
{
	public class PlayerYawFollower : MonoBehaviour
	{
		private void FixedUpdate()
		{
			var horizontalLookDirection = Vector3.ProjectOnPlane(PlayerController.Instance.transform.forward, Vector3.up);
			var rotation = Quaternion.LookRotation(horizontalLookDirection, Vector3.up);
			transform.rotation = rotation;
		}
	}
}
