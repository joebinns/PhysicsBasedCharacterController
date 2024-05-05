using System;
using UnityEngine;

namespace JoeBinns.Spacetime.Feet
{
    public class FootstepHandler : MonoBehaviour
    {
		[SerializeField] private Rigidbody _speedometer;

		[Header("Footstep")]
		[SerializeField, Min(0f)] private float _interval = 2f;
		[SerializeField, Range(0f, 1f)] private float _normalisedDelay = 0f;

		private float _timer;

		// EVENTS

		public event Action OnFootstep;

		// INITIALIZATION

		private void OnEnable()
		{
			_timer = _normalisedDelay * _interval;
		}

		// UPDATE

		private void Update()
		{
			var horizontalVelocity = Vector3.ProjectOnPlane(_speedometer.velocity, transform.up);
			var horizontalSpeed = horizontalVelocity.magnitude;
			var deltaTimer = Time.deltaTime * horizontalSpeed;
			_timer += deltaTimer;

			if (_timer >= _interval)
			{
				OnFootstep?.Invoke();
				_timer = 0f;
			}
		}
	}
}
