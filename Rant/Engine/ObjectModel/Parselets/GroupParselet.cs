using System;
using System.Collections.Generic;
using Rant.Engine.Compiler;
using Rant.Stringes;

namespace Rant.Engine.ObjectModel.Parselets
{
	internal class GroupParselet : Parselet
	{
		public override IEnumerator<bool> Parse(Token<R> token, Rave vm)
		{
			int itemCount = 1;

			yield return true; // Override precedence to zero

			while (!vm.Reader.TakeLoose(R.RightParen))
			{
				itemCount++;
				vm.Reader.ReadLoose(R.Comma);
				yield return true;
			}

			if (itemCount > 1)
			{
				var list = new List<RantObject>();
				for (int i = 0; i < itemCount; i++)
				{
					list.Add(vm.PopVal());
				}
				list.Reverse();
				vm.PushVal(new RantObject(list));
			}
		}
	}
}