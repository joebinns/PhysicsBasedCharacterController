using System;
using UnityEngine;
using JoeBinns.Inputs;

namespace JoeBinns.Spacetime.Player
{
	public class PlayerJump : MonoBehaviour
	{
		[SerializeField, Min(0f)] private float _jumpHeight = 2f;
		[SerializeField, Min(0f)] private float _failedJumpCooldown = 0.5f;

		private Rigidbody _platformRigidbody = null;
		private float _timeSinceJump = 0f;
		private bool _isJumping = false;
		private bool _isGrounded = true;

		public bool CanJump => !_isJumping && _isGrounded;

		// EVENTS

		public event Action OnJump;

		// INITIALIZATION

		private void Awake()
		{
			// TODO: Should probably unsubscribe from these somewhere...
			PlayerController.Instance.GroundCheck.OnGrounded += OnGrounded;
			PlayerController.Instance.GroundCheck.OnUngrounded += OnUngrounded;
		}

		// TOGGLING

		private void OnEnable()
		{
			InputManager.Instance.Jump.OnStart += OnJumpInput;
		}

		private void OnDisable()
		{
			InputManager.Instance.Jump.OnStart -= OnJumpInput;
		}

		// GROUNDED

		private void OnGrounded(RaycastHit rayHit)
		{
			_isGrounded = true;
			_isJumping = false;
			_platformRigidbody = rayHit.transform.GetComponent<Rigidbody>();
		}

		private void OnUngrounded()
		{
			_isGrounded = false;
			_platformRigidbody = null;
		}

		// INPUT

		private void OnJumpInput(float _)
		{
			if (!CanJump) return;

			Jump();
		}

		// JUMPING

		private void Jump()
		{
			var initialJumpSpeed = GetInitialJumpSpeed(_jumpHeight, PlayerController.Instance.Gravity.ForceMagnitude);
			var initialJumpVelocity = Vector3.up * initialJumpSpeed;
			var rigidbody = PlayerController.Instance.Rigidbody;
			var deltaVelocity = initialJumpVelocity - rigidbody.velocity;
			var impulse = rigidbody.mass * deltaVelocity;
			rigidbody.AddForce(impulse, ForceMode.Impulse);
			_isJumping = true;
			_timeSinceJump = 0f;
			OnJump?.Invoke();

			if (_platformRigidbody)
			{
				_platformRigidbody.AddForce(- impulse, ForceMode.Impulse);
			}
		}

		// FIXED UPDATE

		private void FixedUpdate()
		{
			if (!_isJumping) return;

			FailedJumpCooldown();
			_timeSinceJump += Time.fixedDeltaTime;
		}

		/// <summary>
		/// Incase a jump never makes the player ungrounded, reset is jumping after a cooldown.
		/// </summary>
		private void FailedJumpCooldown()
		{
			if (!_isGrounded || _timeSinceJump <= _failedJumpCooldown) return;

			_isJumping = false;
		}

		// UTILITIES

		/// <summary>
		/// Calculate the initial vertical speed required to achieve a jump with a desired height and gravitational force magnitude.
		/// </summary>
		private float GetInitialJumpSpeed(float jumpHeight, float jumpGravity)
		{
			return Mathf.Sqrt(2f * jumpHeight * jumpGravity);
		}
	}
}
