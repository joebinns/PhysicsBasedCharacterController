using UnityEngine;
using JoeBinns.Spacetime.GroundChecks;

namespace JoeBinns.Spacetime.Platform
{
	public class DynamicPlatformFollowerHandler : MonoBehaviour
	{
		[SerializeField] private GroundCheck _groundCheck;
		[SerializeField] private DynamicPlatformFollower _platformFollower;

		// INITIALIZATION

		private void OnEnable()
		{
			_groundCheck.OnGrounded += OnGrounded; // TODO: We should also TryEnablePlatformFollower if the grounded transform has changed...
		}

		private void OnDisable()
		{
			_groundCheck.OnGrounded -= OnGrounded;
		}

		// GROUNDED

		private void OnGrounded(RaycastHit raycastHit)
		{
			var dynamicPlatform = raycastHit.collider.GetComponentInParent<DynamicPlatform>();
			_platformFollower.DynamicPlatform = dynamicPlatform;
		}
	}
}
