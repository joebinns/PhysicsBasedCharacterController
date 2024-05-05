using UnityEngine;
using JoeBinns.Utilities.Maths;

namespace JoeBinns.Spacetime.Player
{ 
	public class PlayerErector : MonoBehaviour
	{
		[SerializeField, Min(0f)] private float _stiffness = 50f;
		[SerializeField, Min(0f)] private float _damper = 5f;

		private Vector3 _facingGoal;
		private Vector3 _erectGoal = Vector3.up;

		// FIXED UPDATE

		private void FixedUpdate()
		{
			UpdateFacingGoal();
			Erect();
		}

		private void UpdateFacingGoal()
		{
			_facingGoal = PlayerController.Instance.Movement.LastNonZeroInputVelocityDirection;
		}

		private void Erect()
		{
			var rotation = PlayerController.Instance.Rigidbody.rotation;
			var goalRotation = Quaternion.LookRotation(_facingGoal, _erectGoal);
			var rotationError = MathsUtilities.ShortestRotation(goalRotation, rotation);

			// Calculate restoring torque
			rotationError.ToAngleAxis(out var degreesOfError, out var axisOfError);  // TODO: Isn't how I'm using this equivalent to just converting to euler angles?
			var radiansOfError = degreesOfError * Mathf.Deg2Rad;
			var angularVelocity = PlayerController.Instance.Rigidbody.angularVelocity;
			var torque = axisOfError * radiansOfError * _stiffness - angularVelocity * _damper;

			// Apply restoring torque
			PlayerController.Instance.Rigidbody.AddTorque(torque, ForceMode.Force);
		}
	}
}
