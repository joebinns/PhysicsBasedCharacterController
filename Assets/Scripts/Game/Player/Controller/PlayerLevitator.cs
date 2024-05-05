using UnityEngine;

namespace JoeBinns.Spacetime.Player
{
	public class PlayerLevitator : MonoBehaviour
	{
		[SerializeField, Min(0f)] private float _rideHeight = 0.8f;
		[SerializeField, Min(0f)] private float _stiffness = 50f;
		[SerializeField, Min(0f)] private float _damper = 10f;

		private Rigidbody _platformRigidbody = null;

		// PROPERTIES

		public float RideHeight => _rideHeight;

		// TOGGLING

		private void OnEnable()
		{
			PlayerController.Instance.GroundCheck.OnUngrounded += Disable;
			PlayerController.Instance.Jump.OnJump += Disable;

			PlayerController.Instance.GroundCheck.OnGrounded -= Enable;
		}

		private void OnDisable()
		{
			PlayerController.Instance.GroundCheck.OnGrounded += Enable;

			PlayerController.Instance.GroundCheck.OnUngrounded -= Disable;
			PlayerController.Instance.Jump.OnJump -= Disable;
		}

		private void Enable(RaycastHit rayHit)
		{
			enabled = true;
			_platformRigidbody = rayHit.transform.GetComponent<Rigidbody>();
		}

		private void Disable()
		{
			enabled = false;
			_platformRigidbody = null;
		}

		// FIXED UPDATE

		private void FixedUpdate()
		{
			Levitate();
		}

		private void Levitate()
		{
			var relativeVerticalVelocity = GetRelativeVerticalVelocity();
			var rideHeightError = GetRideHeightError();

			// Calculate restoring force
			var restoringForceMagnitude = (rideHeightError * _stiffness - relativeVerticalVelocity * _damper);
			var restoringForce = restoringForceMagnitude * Vector3.down;
			var gravityCorrectedRestoringForce = restoringForce - PlayerController.Instance.Gravity.Force;

			// Apply restoring force
			PlayerController.Instance.Rigidbody.AddForce(gravityCorrectedRestoringForce, ForceMode.Force);

			// If platform has a rigidbody, then apply the opposite force to the platform
			if (_platformRigidbody)
			{
				_platformRigidbody.AddForceAtPosition(- restoringForce, PlayerController.Instance.GroundCheck.RayHit.point);
			}	
		}

		// UTILITIES

		private float GetRelativeVerticalVelocity()
		{
			// TODO: Account for platform's velocity (if it's a rigidbody)
			return -PlayerController.Instance.Rigidbody.velocity.y;
		}

		private float GetRideHeightError()
		{
			float rideHeightError = PlayerController.Instance.GroundCheck.RadiusIndependentRayHitDistance - _rideHeight;
			return rideHeightError;
		}

#if !SHIPPING
		// GIZMOS

		private void OnDrawGizmosSelected()
		{
			if (!PlayerController.Instance) return;
			if (!PlayerController.Instance.GroundCheck) return;
			if (!PlayerController.Instance.GroundCheck.IsGrounded) return;

			var equillibrium = PlayerController.Instance.GroundCheck.RadiusIndependentRayHitPoint + _rideHeight * Vector3.up;
			Gizmos.color = Color.white;
			Gizmos.DrawWireSphere(equillibrium, 0.1f);
		}
#endif
	}
}
