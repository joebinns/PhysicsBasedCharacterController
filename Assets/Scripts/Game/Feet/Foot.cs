using UnityEngine;
using JoeBinns.Spacetime.GroundChecks;

namespace JoeBinns.Spacetime.Feet
{
    public class Foot : MonoBehaviour
    {
		[SerializeField] private GroundCheck _groundCheck;
		[SerializeField] private FootstepHandler _footstepHandler;

		// INITIALIZATION

		private void OnEnable()
		{
			_groundCheck.OnGrounded += EnableFootstepHandler;
			_groundCheck.OnUngrounded += DisableFootstepHandler;

			_footstepHandler.enabled = _groundCheck.IsGrounded;
		}

		private void OnDisable()
		{
			_groundCheck.OnGrounded -= EnableFootstepHandler;
			_groundCheck.OnUngrounded -= DisableFootstepHandler;

			_footstepHandler.enabled = false;
		}

		// FOOTSTEP HANDLER

		private void EnableFootstepHandler(RaycastHit _) => _footstepHandler.enabled = true;
		private void DisableFootstepHandler() => _footstepHandler.enabled = false;
	}
}
