using System;
using System.Collections.Generic;
using Rant.Engine.Compiler;
using Rant.Stringes;

namespace Rant.Engine.ObjectModel.Parselets
{
	internal class NameParselet : Parselet
	{
		public override IEnumerator<bool> Parse(Token<R> token, Rave vm)
		{
			var name = token.Value;
			if (!Util.ValidateName(name))
				throw new RantException(vm.Reader.Source, token, $"Invalid identifier name '{name}'");

			var value = vm.Rant.Objects[name];

			if (value == null)
				throw new RantException(vm.Reader.Source, token, $"Tried to access nonexistent variable '{name}'");

			vm.PushVal(value);

			yield break;
		}
	}
}