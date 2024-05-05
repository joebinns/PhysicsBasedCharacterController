using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace JoeBinns.Spacetime.Player.Shadow
{
	public class PlayerShadow : MonoBehaviour
	{
		[SerializeField] private DecalProjector _decalProjector;
		[SerializeField, Min(0f)] private float _height = 2f;

		private Vector3 _pivot;
		private Vector3 _size;

		// INITIALIZATION

		private void Awake()
		{
			_pivot = _decalProjector.pivot;
			_size = _decalProjector.size;
		}

		private void OnEnable()
		{
			EnableProjector();
		}

		private void OnDisable()
		{
			DisableProjector();
		}

		// UPDATE

		private void Update()
		{
			UpdateDecal();
		}

		private void UpdateDecal()
		{
			var distance = PlayerController.Instance.LongGroundCheck.RadiusIndependentRayHitDistance;
			UpdateSize(distance);
			UpdatePivot(distance);
		}

		private void UpdateSize(float distance)
		{
			var ratio = 1f - distance / PlayerController.Instance.LongGroundCheck.RadiusIndependentRange;
			var length = ratio * PlayerController.DIAMETER;
			_size.x = length;
			_size.y = length;
			_size.z = _height;
			_decalProjector.size = _size;
		}

		private void UpdatePivot(float distance)
		{
			_pivot.z = distance + 0.5f * _height;
			_decalProjector.pivot = _pivot;
		}

		// PROJECTOR

		private void EnableProjector() => _decalProjector.enabled = true;
		private void DisableProjector() => _decalProjector.enabled = false;
	}
}
