using Rant.Stringes;
using System;
using System.Collections.Generic;
using System.Linq;

using Rant.Engine.Constructs;

namespace Rant.Engine.Syntax
{
	/// <summary>
	/// Represents a block construct, which provides multiple options to the interpreter for the next sequence, one of which is chosen.
	/// </summary>
	internal class RABlock : RantAction
	{
		private readonly List<RantAction> _items = new List<RantAction>();

		public RABlock(Stringe range, params RantAction[] items)
			: base(range)
		{
			_items.AddRange(items);
		}

		public RABlock(Stringe range, List<RantAction> items)
			: base(range)
		{
			_items.AddRange(items);
		}

		public override IEnumerator<RantAction> Run(Sandbox sb)
		{
			var attribs = sb.NextAttribs();
			int next = -1;
			var block = new BlockState(attribs.Repetitons);
			sb.Blocks.Push(block);
			for (int i = 0; i < attribs.Repetitons; i++)
			{
				next = attribs.NextIndex(_items.Count, sb.RNG);
				if (next == -1) break;
				block.Next(next);
				sb.Blocks.Pop(); // Don't allow separator to access block state
				// Separator
				if (i > 0 && attribs.Separator != null) yield return attribs.Separator;
				sb.Blocks.Push(block); // Now put it back
				// Prefix
				if (attribs.Before != null) yield return attribs.Before;
				// Content
				yield return _items[next];
				// Affix
				if (attribs.After != null) yield return attribs.After;
			}
			sb.Blocks.Pop();
		}
	}
}