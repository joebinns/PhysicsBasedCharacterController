using JoeBinns.Spacetime.Platform;
using UnityEngine;

namespace JoeBinns.Spacetime.Feet
{
	public class FootstepParticleSystemHandler : DynamicParticleSystemHandler
	{
		[Header("Footstep")]
		[SerializeField] private FootstepHandler _footstepHandler;

		// INITIALIZATION

		protected override void OnEnable()
		{
			base.OnEnable();
			_footstepHandler.OnFootstep += Footstep;
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			_footstepHandler.OnFootstep -= Footstep;
		}

		// FOOTSTEP

		private void Footstep()
		{
			_particleSystem.Emit(1);
		}
	}
}
