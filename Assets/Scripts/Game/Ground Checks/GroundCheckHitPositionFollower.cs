using UnityEngine;

namespace JoeBinns.Spacetime.GroundChecks
{ 
	public class GroundCheckHitPositionFollower : MonoBehaviour
	{
		[SerializeField] private GroundCheck _groundCheck;

		// UPDATE

		private void Update()
		{
			transform.position = _groundCheck.HitPosition;
		}

#if !SHIPPING
		private const float GIZMO_SPHERE_RADIUS = 0.25f;

		// GIZMOS

		private void OnDrawGizmosSelected()
		{
			Gizmos.DrawWireSphere(transform.position, GIZMO_SPHERE_RADIUS);
		}
	}
#endif
}
