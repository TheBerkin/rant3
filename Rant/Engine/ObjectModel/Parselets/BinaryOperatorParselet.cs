using System;
using System.Collections.Generic;
using Rant.Engine.Compiler;
using Rant.Stringes;
using Rant.Engine.ObjectModel.Metas;

namespace Rant.Engine.ObjectModel.Parselets
{
	internal class BinaryOperatorParselet : InfixParselet
	{
		public BinaryOperatorParselet(Precedence precedence, bool rightAssociative) 
			: base(precedence)
		{
			PrecedenceOverride = rightAssociative ? (Precedence)((int)precedence - 1) : precedence;
		}

		public override IEnumerator<bool> Parse(Token<R> token, Rave vm)
		{
			// Request value
			yield return true;

			// Pop operands
			var b = vm.Pop();
			var a = vm.Pop();

			switch (token.ID)
			{
				case R.Plus:
					vm.PushVal(a.Resolve(vm) + b.Resolve(vm));
					break;
				case R.Hyphen:
					vm.PushVal(a.Resolve(vm) - b.Resolve(vm));
					break;
				case R.Asterisk:
					vm.PushVal(a.Resolve(vm) * b.Resolve(vm));
					break;
				case R.ForwardSlash:
					vm.PushVal(a.Resolve(vm) / b.Resolve(vm));
					break;
				case R.Equal:
				{
					var nameMeta = a as NameMeta;
					if (nameMeta == null)
						throw new RantException(vm.Reader.Source, token, "Left side of assignment was not a variable.");
					vm.Rant.Objects[nameMeta.Name] = b.Resolve(vm);
					vm.Reader.ReadLoose(R.Semicolon);
                    break;
				}
			}
		}
	}
}