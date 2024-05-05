using UnityEngine;
using JoeBinns.Inputs;
using Utilities.GizmosExtensions;

namespace JoeBinns.Spacetime.Player
{
	public class PlayerMovement : MonoBehaviour
	{
		public enum MovementState
		{
			Default,
			Sprint
		}

		[SerializeField, Min(0f)] private float _defaultSpeed;
		[SerializeField, Min(0f)] private float _sprintSpeed;
		[SerializeField, Min(0f)] private float _acceleration;
		[SerializeField, Min(0f)] private float _maximumAcceleration;
		[Tooltip("Curve of acceleration factor over the dot product of current velocity and goal velocity.")]
		[SerializeField] private AnimationCurve _accelerationCurve;
		[SerializeField] private float _lean;

		private Vector3 _lastNonZeroInputVelocityDirection = Vector3.forward;
		private Vector3 _goalVelocity;
		private Vector3 _forward;
		private Vector3 _right;
		private Vector2 _input;
		private MovementState _state;
		private float _speed;
		private bool _isSprintEngaged;

		// PROPERTIES

		public Vector3 LastNonZeroInputVelocityDirection => _lastNonZeroInputVelocityDirection;

		public MovementState State
		{
			get => _state;
			private set
			{
				switch (value)
				{
					case MovementState.Default:
						_speed = _defaultSpeed;
						break;
					case MovementState.Sprint:
						_speed = _sprintSpeed;
						break;
				}

				_state = value;
			}
		}

		public bool IsSprintEngaged
		{
			get => _isSprintEngaged;
			private set
			{
				_isSprintEngaged = value;

				// Toggle movement state
				if (value && _state == MovementState.Default)
				{
					State = MovementState.Sprint;
				}
				else if (!value && _state == MovementState.Sprint)
				{
					State = MovementState.Default;
				}
			}
		}

		// INITIALIZATION

		private void Awake()
		{
			State = MovementState.Default;
			IsSprintEngaged = false;
		}

		private void OnEnable()
		{
			ResetMoveInput();
			InputManager.Instance.Move.OnPerform += OnMoveInput;
			InputManager.Instance.Move.OnCancel += OnMoveInput;
			InputManager.Instance.Move.OnBind += ResetMoveInput;
			InputManager.Instance.Move.OnUnbind += ResetMoveInput;

			ResetSprintEngagement();
			InputManager.Instance.Sprint.OnPress += OnSprintEngagement;
			InputManager.Instance.Sprint.OnBind += ResetSprintEngagement;
			InputManager.Instance.Sprint.OnUnbind += ResetSprintEngagement;
		}

		private void OnDisable()
		{
			InputManager.Instance.Move.OnPerform -= OnMoveInput;
			InputManager.Instance.Move.OnCancel -= OnMoveInput;
			InputManager.Instance.Move.OnBind -= ResetMoveInput;
			InputManager.Instance.Move.OnUnbind -= ResetMoveInput;

			InputManager.Instance.Sprint.OnPress -= OnSprintEngagement;
			InputManager.Instance.Sprint.OnBind -= ResetSprintEngagement;
			InputManager.Instance.Sprint.OnUnbind -= ResetSprintEngagement;

			_input = Vector2.zero;
		}

		// INPUT

		private void OnMoveInput(Vector2 input) => _input = input;
		private void OnSprintEngagement() => IsSprintEngaged = !_isSprintEngaged;

		private void ResetMoveInput() => _input = InputManager.Instance.Move.Value;
		private void ResetSprintEngagement() => IsSprintEngaged = InputManager.Instance.Sprint.IsInProgress;

		// FIXED UPDATE

		private Vector3 _curvatureIgnorantAcceleration = Vector3.zero;
		private Vector3 _curvatureCompensatedAcceleration = Vector3.zero;

		private void FixedUpdate()
		{
			// Update the forward and right directions
			UpdateOrientation();

			var horizontalRigidbodyVelocity = Vector3.ProjectOnPlane(PlayerController.Instance.Rigidbody.velocity, Vector3.up);
			var worldInput = InputToWorld(_input);
			if (_input.sqrMagnitude != 0f)
			{
				_lastNonZeroInputVelocityDirection = worldInput;
			}

			// Calculate the acceleration magnitude
			var velocityAlignment = Vector3.Dot(worldInput, horizontalRigidbodyVelocity.normalized);
			var accelerationFactor = _accelerationCurve.Evaluate(velocityAlignment);
			var accelerationMagnitude = accelerationFactor * _acceleration;

			// Calculate the goal velocity
			var goalVelocity = worldInput * _speed;
			var maxDeltaVelocity = accelerationMagnitude * Time.fixedDeltaTime;
			_goalVelocity = Vector3.MoveTowards(_goalVelocity, goalVelocity, maxDeltaVelocity);
			var horizontalGoalVelocity = Vector3.ProjectOnPlane(_goalVelocity, Vector3.up);

			// Calculate acceleration to apply
			var horizontalAcceleration = (horizontalGoalVelocity - horizontalRigidbodyVelocity) / Time.fixedDeltaTime;
			_curvatureIgnorantAcceleration = horizontalAcceleration;

			// Clamp acceleration
			var maximumAccelerationMagnitude = accelerationFactor * _maximumAcceleration;
			horizontalAcceleration = Vector3.ClampMagnitude(horizontalAcceleration, maximumAccelerationMagnitude);

			// Rotate acceleration to account for the gravity field's curvature
			var velocity = horizontalAcceleration * Time.fixedDeltaTime;
			var displacement = velocity * Time.fixedDeltaTime;
			var direction = displacement.normalized;
			var acceleration = horizontalAcceleration.magnitude * direction;
			_curvatureCompensatedAcceleration = acceleration;
		
			// Apply force
			var force = PlayerController.Instance.Rigidbody.mass * acceleration;
			var position = PlayerController.Instance.Rigidbody.position + Vector3.up * _lean;	
			PlayerController.Instance.Rigidbody.AddForceAtPosition(force, position, ForceMode.Force);
		}

		private void UpdateOrientation()
		{
			var playerCameraHandler = PlayerController.Instance.CameraHandler;

			// Calculate look rotation
			var orbitalTransposer = playerCameraHandler.OrbitalTransposer;
			var xAxisLookDegrees = orbitalTransposer.m_XAxis.Value;
			var xAxislookRotation = Quaternion.Euler(0f, xAxisLookDegrees, 0f);
			var lookRotation = xAxislookRotation;

			// Calculate look directions
			var lookForward = lookRotation * Vector3.forward;
			var lookRight = lookRotation * Vector3.right;

			_forward = lookForward;
			_right = lookRight;
		}

		private Vector3 InputToWorld(Vector2 input) => (input.x * _right) + (input.y * _forward);

#if !SHIPPING
		// GIZMOS

		private void OnDrawGizmosSelected()
		{
			if (_forward.sqrMagnitude == 0f || _right.sqrMagnitude == 0f) return;

			var position = transform.position;

			// Draw forward arrow
			Gizmos.color = Color.green;
			GizmosExtensions.DrawArrow(position, position + _forward);

			// Draw right arrow
			Gizmos.color = Color.red;
			GizmosExtensions.DrawArrow(position, position + _right);

			// Draw acceleration
			var epsilon = 0.1f;
			if (_curvatureIgnorantAcceleration.magnitude > epsilon && _curvatureCompensatedAcceleration.sqrMagnitude != epsilon)
			{
				Gizmos.color = Color.grey;
				GizmosExtensions.DrawArrow(position, position + _curvatureIgnorantAcceleration.normalized);
				Gizmos.color = Color.white;
				GizmosExtensions.DrawArrow(position, position + _curvatureCompensatedAcceleration.normalized);
			}
		}
#endif
	}
}
