using Unity.Mathematics;
using UnityEngine;

namespace JoeBinns.Extensions.Unity
{
	public static class UnityExtensions
	{
		public static Vector3 Multiply(this int3 a, Vector3 b)
		{
			var product = new Vector3()
			{
				x = a.x * b.x,
				y = a.y * b.y,
				z = a.z * b.z
			};
			return product;
		}

		public static Vector3 Multiply(this int3 a, float b)
		{
			var product = Multiply(a, b * Vector3.one);
			return product;
		}
	}
}
