using UnityEngine;

namespace JoeBinns.Spacetime.Player.Shadow
{
	public class PlayerShadowHandler : MonoBehaviour
	{
		[SerializeField] private PlayerShadow _shadow;

		// PROPERTIES

		public PlayerShadow PlayerShadow => _shadow;

		// INITIALIZATION

		private void OnEnable()
		{
			PlayerController.Instance.LongGroundCheck.OnGrounded += EnableShadow;
			PlayerController.Instance.LongGroundCheck.OnUngrounded += DisableShadow;

			_shadow.enabled = PlayerController.Instance.LongGroundCheck.IsGrounded;
		}

		private void OnDisable()
		{
			PlayerController.Instance.LongGroundCheck.OnGrounded -= EnableShadow;
			PlayerController.Instance.LongGroundCheck.OnUngrounded -= DisableShadow;

			_shadow.enabled = false;
		}

		// SHADOW

		private void EnableShadow(RaycastHit _) => _shadow.enabled = true;
		private void DisableShadow() => _shadow.enabled = false;
	}
}
