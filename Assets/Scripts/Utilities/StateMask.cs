namespace JoeBinns.Utilities
{
	public class StateMask
	{
		// PROPERTIES

		public int Value { get; private set; }

		// UTILITIES

		public void Add(int state)
		{
			Value |= state;
		}

		public void Clear()
		{
			Value = 0;
		}

		public bool Contains(int state)
		{
			return (state & Value) > 0;
		}

		public void Remove(int state)
		{
			// If and only if the state is included in the state mask
			if ((state & Value) == state)
			{
				// XOR is used to remove the state from the state mask
				Value ^= state;
			}
		}
	}
}
