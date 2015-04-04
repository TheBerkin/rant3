using System;
using System.Collections.Generic;
using Rant.Engine.Compiler;
using Rant.Stringes;

namespace Rant.Engine.ObjectModel.Parselets
{
	internal class VarParselet : Parselet
	{
		public override IEnumerator<bool> Parse(Token<R> token, Rave vm)
		{
			vm.Reader.SkipSpace();
			var nameToken = vm.Reader.ReadToken();
			if (nameToken.ID != R.Text || !Util.ValidateName(nameToken.Value))
				throw new RantException(vm.Reader.Source, nameToken, $"Invalid identifier '{nameToken.Value}'");

			var name = nameToken.Value;

			vm.Rant.Objects[name] = RantObject.No;

			if (vm.Reader.TakeLoose(R.Equal))
			{
				yield return true;
				vm.Rant.Objects[name] = vm.PopVal();
			}

			if (!vm.Reader.TakeLoose(R.Semicolon))
				throw new RantException(vm.Reader.Source, vm.Reader.End ? vm.Reader.PrevToken : vm.Reader.PeekToken(), "Expected ';'");
		}
	}
}