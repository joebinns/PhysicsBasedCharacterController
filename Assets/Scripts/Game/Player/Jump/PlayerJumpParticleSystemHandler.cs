using JoeBinns.Spacetime.Platform;
using UnityEngine;

namespace JoeBinns.Spacetime.Player
{
    public class PlayerJumpParticleSystemHandler : DynamicParticleSystemHandler
	{
		[Header("Jump")]
		[SerializeField, Min(0)] private int _burstCount = 3;

		// INITIALIZATION

		protected override void OnEnable()
		{
			base.OnEnable();
			PlayerController.Instance.Jump.OnJump += Jump;
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			PlayerController.Instance.Jump.OnJump -= Jump;
		}

		// JUMP

		private void Jump()
		{
			_particleSystem.Emit(_burstCount);
		}
	}
}
