using UnityEngine;

namespace JoeBinns
{
	public abstract class Singleton : MonoBehaviour
	{
	}

	public abstract class Singleton<T> : Singleton where T : Singleton
	{
		public static T Instance { get; private set; }

		// INITIALIZE

		protected virtual void Awake()
		{
			InitializeSingleton();
		}

		// INITIALIZE SINGLETON

		private void InitializeSingleton()
		{
			var instance = this as T;
			if (Instance == null)
			{
				Instance = instance;
			}
			else if (Instance != instance)
			{
				Debug.LogWarning($"Tried to initialize a singleton with an existing instance. " +
					$"Destroying {this}.");
				Destroy(this);
			}
		}
	}
}
