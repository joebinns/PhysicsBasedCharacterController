using UnityEngine;

namespace JoeBinns.Spacetime.Player.Follower
{
	public class PlayerPositionFollower : MonoBehaviour
	{
		private void FixedUpdate()
		{
			var position = PlayerController.Instance.transform.position;
			transform.position = position;
		}
	}
}
