using System;
using System.Collections.Generic;
using Rant.Engine.Compiler;
using Rant.Stringes;

namespace Rant.Engine.ObjectModel.Parselets
{
	internal class IndexerParselet : InfixParselet
	{
		public IndexerParselet()
			: base(Precedence.Postfix)
		{	
		}

		public override IEnumerator<bool> Parse(Token<R> token, Rave vm)
		{
			var listObj = vm.PopVal();
			if (listObj.Type != RantObjectType.List)
				throw new RantException(vm.Reader.Source, token, "Indexer is not valid on this object.");

			var list = listObj.Value as List<RantObject>;

			yield return true; // Get index

			var indexObj = vm.PopVal();
			if (indexObj.Type != RantObjectType.Number)
			{
				vm.PushVal(RantObject.No);
				yield break;
			}
			int index = (int)((double)indexObj.Value);
			if (index < 0 || index >= list.Count)
			{
				vm.PushVal(RantObject.No);
				yield break;
			}
            vm.PushVal(list[index]);
			vm.Reader.ReadLoose(R.RightSquare);
		}
	}
}