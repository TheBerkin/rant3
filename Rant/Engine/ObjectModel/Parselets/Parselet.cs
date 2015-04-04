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

		public Precedence PrecedenceOverride { get; protected set; } = Precedence.Never;
		
		/// <summary>
		/// Returns an enumerator for sequential parsing operations. 
		/// Yield to request an expression. 
		/// Yielding "true" will override the next state's precedence with the PrecedenceOverride value. 
		/// Yielding "false" will use the current expression's precedence.
		/// </summary>
		/// <param name="token">The token that triggered the parselet.</param>
		/// <param name="vm">The VM instance that called the parselet.</param>
		/// <returns></returns>
		public abstract IEnumerator<bool> Parse(Token<R> token, Rave vm);
	}
}