using System.Collections.Generic;

using Rant.Engine.Compiler;
using Rant.Stringes;

namespace Rant.Engine.ObjectModel.Parselets
{
	/// <summary>
	/// Represents a set of instructions for parsing a Rave code element.
	/// </summary>
	internal abstract class Parselet
	{
		public readonly Precedence Precedence;
		
		public abstract IEnumerator<bool> Parse(Token<R> token, Rave vm);
	}
}