using UnityEngine;

namespace JoeBinns.Spacetime.Player
{
	public class PlayerCameraTarget : MonoBehaviour
	{
		// UPDATE

		private void Update()
		{
			UpdatePoint();
		}

		// POINT

		private void UpdatePoint()
		{
			transform.position = PlayerController.Instance.LongGroundCheck.RadiusIndependentRayHitPoint;
		}
	}
}
