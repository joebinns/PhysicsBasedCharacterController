using UnityEngine;

namespace JoeBinns.Spacetime.Player
{
    public class PlayerGravity : MonoBehaviour
    {
        [SerializeField, Min(0.01f)] private float _forceMultiplier = 1f;

        // PROPERTIES

		public float ForceMagnitude { get; private set; }
		public Vector3 Force { get; private set; }

		// FIXED UPDATE

		private void FixedUpdate()
		{
			ForceMagnitude = _forceMultiplier * PlayerController.Instance.Rigidbody.mass * Physics.gravity.magnitude;
			Force = ForceMagnitude * Physics.gravity.normalized;
			PlayerController.Instance.Rigidbody.AddForce(Force, ForceMode.Force);
		}
	}
}
