using System.Collections.Generic;

using Rant.Core.Stringes;

namespace Rant.Core.Compiler.Syntax
{
	/// <summary>
	/// Represents a Rant Syntax Tree (RST) node for a Rant pattern. This is the base class for all Rant actions.
	/// </summary>
	internal abstract class RST
	{
		internal Stringe Range;

		protected RST(Stringe range)
		{
			Range = range;
		}

		/// <summary>
		/// Performs the operations defined in the action, given a specific sandbox to operate upon.
		/// </summary>
		/// <param name="sb">The sandbox on which to operate.</param>
		/// <returns></returns>
		public abstract IEnumerator<RST> Run(Sandbox sb);
	}
}