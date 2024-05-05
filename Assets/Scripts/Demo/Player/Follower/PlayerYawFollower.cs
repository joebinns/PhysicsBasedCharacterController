using UnityEngine;

namespace JoeBinns.Spacetime.Player.Follower
{
	public class PlayerYawFollower : MonoBehaviour
	{
		private void FixedUpdate()
		{
			var rotation = Quaternion.LookRotation(PlayerController.Instance.transform.forward, Vector3.up);;
			transform.rotation = rotation;
		}
	}
}
