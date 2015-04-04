using System;
using System.Collections.Generic;
using Rant.Engine.Compiler;
using Rant.Stringes;

namespace Rant.Engine.ObjectModel.Parselets
{
	internal class BinaryOperatorParselet : InfixParselet
	{
		public BinaryOperatorParselet(Precedence precedence, bool rightAssociative) 
			: base(rightAssociative ? (Precedence)((int)precedence - 1) : precedence)
		{	
		}

		public override IEnumerator<bool> Parse(Token<R> token, Rave vm)
		{
			// Request value
			yield return true;

			// Pop operands
			var b = vm.PopVal();
			var a = vm.PopVal();

			switch (token.ID)
			{
				case R.Plus:
					vm.PushVal(a + b);
					break;
				case R.Hyphen:
					vm.PushVal(a - b);
					break;
				case R.Asterisk:
					vm.PushVal(a * b);
					break;
				case R.ForwardSlash:
					vm.PushVal(a / b);
					break;
			}
		}
	}
}