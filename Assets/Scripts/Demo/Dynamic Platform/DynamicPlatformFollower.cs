using System;
using UnityEngine;

namespace JoeBinns.Spacetime.Platform
{
	public class DynamicPlatformFollower : MonoBehaviour
	{
		private DynamicPlatform _dynamicPlatform;

		// EVENTS

		public event Action<DynamicPlatform> OnDynamicPlatformChanged;

		// PROPERTIES

		public DynamicPlatform DynamicPlatform 
		{
			get => _dynamicPlatform;
			set
			{
				if (_dynamicPlatform == value) return;

				enabled = value != null;

				OnDynamicPlatformChanged?.Invoke(value);
				_dynamicPlatform = value;
			}
		}

		// FIXED UPDATE

		private void FixedUpdate()
		{
			_dynamicPlatform.Follow(transform);
		}
	}
}
