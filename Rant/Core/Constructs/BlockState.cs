namespace Rant.Core.Constructs
{
	/// <summary>
	/// Supplies state information about an active block, such as the iteration, item count, and current index.
	/// </summary>
	internal class BlockState
	{
		public BlockState(int count)
		{
			Count = count;
		}

		public int Count { get; }
		public int Iteration { get; private set; }
		public int Index { get; private set; }

		public void Next(int index)
		{
			Index = index;
			Iteration++;
		}
	}
}