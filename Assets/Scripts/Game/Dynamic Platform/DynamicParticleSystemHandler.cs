using UnityEngine;

namespace JoeBinns.Spacetime.Platform
{
	public class DynamicParticleSystemHandler : MonoBehaviour
	{
		[SerializeField] protected ParticleSystem _particleSystem;
		[SerializeField] private DynamicPlatformFollower _dynamicPlatformFollower;

		private ParticleSystem.MainModule _mainModule;

		// INITIALIZATION

		protected virtual void Awake()
		{
			_mainModule = _particleSystem.main;
		}

		protected virtual void OnEnable()
		{
			_dynamicPlatformFollower.OnDynamicPlatformChanged += SetSimulationSpace;
		}

		protected virtual void OnDisable()
		{
			_dynamicPlatformFollower.OnDynamicPlatformChanged -= SetSimulationSpace;
		}

		// SIMULATION SPACE

		private void SetSimulationSpace(DynamicPlatform dynamicPlatform)
		{
			var useCustomSimulationSpace = dynamicPlatform != null;
			if (useCustomSimulationSpace)
			{
				_mainModule.simulationSpace = ParticleSystemSimulationSpace.Custom;
				_mainModule.customSimulationSpace = dynamicPlatform.transform;
			}
			else
			{
				_mainModule.simulationSpace = ParticleSystemSimulationSpace.World;
			}
		}
	}
}
