using System;
using UnityEngine;

namespace JoeBinns.Spacetime.GroundChecks
{ 
	public abstract class GroundCheck : MonoBehaviour
	{
		[Header("Ground Check")]
		[Min(0f)] public float Range = 1f;
		[SerializeField] protected LayerMask _layerMask;
		[SerializeField] protected QueryTriggerInteraction _queryTriggerInteraction = QueryTriggerInteraction.Ignore;

		protected Ray _ray;
		private bool _isGrounded = false;
		private bool _isGroundedInitialized = false;

		// PROPERTIES

		public bool IsGrounded
		{
			get => _isGrounded;
			private set
			{
				if (_isGrounded == value && _isGroundedInitialized) return;

				switch (value)
				{
					case true:
						OnGrounded?.Invoke(RayHit);
						break;
					case false:
						OnUngrounded?.Invoke();
						break;
				}
				_isGrounded = value;
				_isGroundedInitialized = true;
			}
		}

		public RaycastHit RayHit { get; private set; }

		public Vector3 Direction => - transform.up;

		// EVENTS

		public event Action<RaycastHit> OnGrounded;
		public event Action OnUngrounded;

		// FIXED UPDATE

		protected virtual void FixedUpdate()
		{
			Check();
		}

		// GROUND CHECK

		protected void Check()
		{
			bool isGrounded;
			_ray = new Ray(transform.position, Direction);
			(isGrounded, RayHit) = Check(_ray);
			IsGrounded = isGrounded;
		}

		protected abstract (bool, RaycastHit) Check(Ray _ray);

		// UTILITIES

		public Vector3 HitPosition => IsGrounded ? RayHit.point : _ray.GetPoint(Range);

#if !SHIPPING
		// GIZMOS

		protected virtual void OnDrawGizmosSelected()
		{
			var hitPosition = HitPosition;

			// Draw ground check ray
			Gizmos.color = IsGrounded ? Color.green : Color.red;
			Gizmos.DrawLine(_ray.origin, hitPosition);
		}
#endif
	}
}
