using System.Collections.Generic;

namespace Rant.Engine.Constructs
{
	internal class BlockManager
	{
		private readonly Stack<BlockState> _blocks = new Stack<BlockState>();
		private BlockAttribs _attribs = new BlockAttribs();

		public BlockManager()
		{
			
		}
	}
}