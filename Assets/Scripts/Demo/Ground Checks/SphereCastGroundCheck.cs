using UnityEngine;
using Utilities.GizmosExtensions;

namespace JoeBinns.Spacetime.GroundChecks
{ 
	public class SphereCastGroundCheck : GroundCheck
	{
		[Header("Sphere Cast")]
		[SerializeField] protected float _radius = 0.25f;

		// PROPERTIES

		public Vector3 RadiusIndependentRayHitPoint => IsGrounded ? RayHit.point - _radius * Direction : _ray.GetPoint(RadiusIndependentRange);
		public float RadiusIndependentRange => Mathf.Max(0f, Range - _radius);
		public float RadiusIndependentRayHitDistance => IsGrounded ? Mathf.Max(0f, RayHit.distance - _radius) : RadiusIndependentRange;

		// GROUND CHECK

		protected override (bool, RaycastHit) Check(Ray ray)
		{
			var isGrounded = Physics.SphereCast(ray, _radius, out var rayHit, RadiusIndependentRange, _layerMask, _queryTriggerInteraction);
			if (!isGrounded)
			{
				// Making up for SphereCast not detecting colliders for which the original sphere overlaps
				isGrounded = Physics.Raycast(ray, out rayHit, _radius, _layerMask, _queryTriggerInteraction);
			}
			return (isGrounded, rayHit);
		}

#if !SHIPPING
		// GIZMOS

		protected override void OnDrawGizmosSelected()
		{
			base.OnDrawGizmosSelected();

			var hitPosition = HitPosition;

			// Draw ground check hit
			var circleRotation = Quaternion.FromToRotation(Vector3.up, _ray.direction);
			GizmosExtensions.DrawWireCircle(hitPosition, _radius, 20, circleRotation);
		}
#endif
	}
}
