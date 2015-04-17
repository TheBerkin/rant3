using Rant.Stringes;
using System.Collections.Generic;

namespace Rant.Engine.Syntax
{
	/// <summary>
	/// Represents an Abstract Syntax Tree (AST) node for a Rant pattern. This is the base class for all Rant actions.
	/// </summary>
	internal abstract class RantAction
	{
		private readonly Stringe _range;

		public Stringe Range => _range;

		protected RantAction(Stringe range)
		{
			_range = range;
		}

		/// <summary>
		/// Performs the operations defined in the action, given a specific sandbox to operate upon.
		/// </summary>
		/// <param name="sb">The sandbox on which to operate.</param>
		/// <returns></returns>
		public abstract IEnumerator<RantAction> Run(Sandbox sb);
	}
}