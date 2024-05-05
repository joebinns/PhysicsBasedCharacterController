using UnityEngine;

namespace JoeBinns.Spacetime.Player.Follower
{
	public class PlayerScaleFollower : MonoBehaviour
	{
		private void FixedUpdate()
		{
			transform.localScale = PlayerController.Instance.transform.localScale;
		}
	}
}
