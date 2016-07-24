using System.Collections.Generic;

using Rant.Internals.Stringes;

namespace Rant.Internals.Engine.Compiler.Syntax
{
	/// <summary>
	/// Represents an Abstract Syntax Tree (AST) node for a Rant pattern. This is the base class for all Rant actions.
	/// </summary>
	internal abstract class RantAction
	{
		internal Stringe Range;

		protected RantAction(Stringe range)
		{
			Range = range;
		}

		/// <summary>
		/// Performs the operations defined in the action, given a specific sandbox to operate upon.
		/// </summary>
		/// <param name="sb">The sandbox on which to operate.</param>
		/// <returns></returns>
		public abstract IEnumerator<RantAction> Run(Sandbox sb);
	}
}