using UnityEngine;

namespace JoeBinns.Utilities.Maths
{
	public static class MathsUtilities
	{
		/// <summary>
		/// Calculates the shortest rotation between two Quaternions.
		/// </summary>
		/// <param name="a">The first Quaternion, from which the rotation is to be calculated.</param>
		/// <param name="b">The second Quaternion, for which the rotation is the goal.</param>
		/// <returns>The shortest rotation from a to b.</returns>
		public static Quaternion ShortestRotation(Quaternion a, Quaternion b)
		{
			if (Quaternion.Dot(a, b) < 0)
			{
				return a * Quaternion.Inverse(Multiply(b, -1f));
			}

			else return a * Quaternion.Inverse(b);
		}

		/// <summary>
		/// Calculates the multiplication of a Quaternion by a scalar, such as to alter the scale of a rotation.
		/// </summary>
		/// <param name="input">The Quaternion rotation to be scaled.</param>
		/// <param name="scalar">The scale by which to multiply the rotation.</param>
		/// <returns>The scale adjusted Quaternion.</returns>
		public static Quaternion Multiply(Quaternion input, float scalar)
		{
			return new Quaternion(input.x * scalar, input.y * scalar, input.z * scalar, input.w * scalar);
		}

		public static float Remap(float inputMin, float inputMax, float outputMin, float outputMax, float value)
		{
			return (value - inputMin) / (inputMax - inputMin) * (outputMax - outputMin) + outputMin;
		}

		public static float ClampedRemap(float value, float inputMin, float inputMax, float outputMin, float outputMax)
		{
			value = Mathf.Clamp(value, inputMin, inputMax);
			return (value - inputMin) / (inputMax - inputMin) * (outputMax - outputMin) + outputMin;
		}
	}
}
