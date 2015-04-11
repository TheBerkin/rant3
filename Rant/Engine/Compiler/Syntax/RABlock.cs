using System;
using System.Collections.Generic;

namespace Rant.Engine.Compiler.Syntax
{
	/// <summary>
	/// Represents a block construct, which provides multiple options to the interpreter for the next sequence, one of which is chosen.
	/// </summary>
	internal class RABlock : RantAction
	{
		private readonly List<RantAction> _items = new List<RantAction>();

		public RABlock(params RantAction[] items)
		{
			_items.AddRange(items);
		}

		public RABlock(List<RantAction> items)
		{
			_items.AddRange(items);
		}

		public override IEnumerator<RantAction> Run(Sandbox sb)
		{
			var attribs = sb.NextAttribs();
			int next = -1;
			for (int i = 0; i < attribs.Repetitons; i++)
			{
				next = attribs.NextIndex(_items.Count, sb.RNG);
				if (next == -1) yield break;
				// TODO: Push repeater context to allow proper usage of [repcount], [repindex], etc...

				// Separator
				if (i > 0 && attribs.Separator != null) yield return attribs.Separator;
				// Prefix
				if (attribs.Before != null) yield return attribs.Before;
				// Content
				yield return _items[next];
				// Affix
				if (attribs.After != null) yield return attribs.After;
			}
		}
	}
}