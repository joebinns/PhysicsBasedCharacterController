using Cinemachine;
using UnityEngine;

namespace JoeBinns.Spacetime.Player
{
	public class PlayerCameraHandler : MonoBehaviour
	{
		[SerializeField] private CinemachineVirtualCamera _virtualCamera;

		private CinemachineOrbitalTransposer _orbitalTransposer;

		// PROPERTIES

		public CinemachineOrbitalTransposer OrbitalTransposer => _orbitalTransposer;

		// INITIALIZATION

		private void Awake()
		{
			_orbitalTransposer = _virtualCamera.GetCinemachineComponent<CinemachineOrbitalTransposer>();
		}
	}
}
