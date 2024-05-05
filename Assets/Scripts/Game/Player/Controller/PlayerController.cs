using UnityEngine;
using JoeBinns.Spacetime.GroundChecks;
using JoeBinns.Spacetime.Player.Shadow;

namespace JoeBinns.Spacetime.Player
{
	public class PlayerController : Singleton<PlayerController>
	{
		public const float DIAMETER = 1f;
		public const float RADIUS = 0.5f;

		[SerializeField] private SphereCastGroundCheck _groundCheck;
		[SerializeField] private SphereCastGroundCheck _longGroundCheck;
		[SerializeField] private PlayerShadowHandler _shadowHandler;
		[SerializeField] private PlayerCameraHandler _cameraHandler;
		[SerializeField] private PlayerLevitator _levitator;
		[SerializeField] private PlayerMovement _movement;
		[SerializeField] private PlayerErector _erector;
		[SerializeField] private PlayerGravity _gravity;
		[SerializeField] private PlayerJump _jump;
		[SerializeField] private Rigidbody _rigibody;
		[SerializeField] private Collider _collider;

		// PROPERTIES

		public SphereCastGroundCheck GroundCheck => _groundCheck;
		public SphereCastGroundCheck LongGroundCheck => _longGroundCheck;
		public PlayerShadowHandler ShadowHandler => _shadowHandler;
		public PlayerCameraHandler CameraHandler => _cameraHandler;
		public PlayerLevitator Levitator => _levitator;
		public PlayerMovement Movement => _movement;
		public PlayerErector Erector => _erector;
		public PlayerGravity Gravity => _gravity;
		public PlayerJump Jump => _jump;
		public Rigidbody Rigidbody => _rigibody;
		public Collider Collider => _collider;
	}
}
