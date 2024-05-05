using JoeBinns.Spacetime.Platform;
using UnityEngine;

namespace JoeBinns.Spacetime.Player
{
    public class PlayerLandParticleSystemHandler : DynamicParticleSystemHandler
    {
		[Header("Land")]
		[SerializeField, Min(0)] private int _burstCount = 7;

		// INITIALIZATION

		protected override void OnEnable()
		{
			base.OnEnable();
			PlayerController.Instance.GroundCheck.OnGrounded += Land;
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			PlayerController.Instance.GroundCheck.OnGrounded -= Land;
		}

		// LAND

		private void Land(RaycastHit _) => Land();

		private void Land()
		{
			_particleSystem.Emit(_burstCount);
		}
	}
}
