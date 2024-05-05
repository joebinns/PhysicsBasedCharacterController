using UnityEngine;

namespace JoeBinns.Spacetime.GroundChecks
{ 
	public class RaycastGroundCheck : GroundCheck
	{
		// GROUND CHECK

		protected override (bool, RaycastHit) Check(Ray ray)
		{
			var isGrounded = Physics.Raycast(ray, out var rayHit, Range, _layerMask, _queryTriggerInteraction);
			return (isGrounded, rayHit);
		}
	}
}
