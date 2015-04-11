using Rant.Stringes;
using System.Collections.Generic;

namespace Rant.Engine.Syntax
{
	/// <summary>
	/// Represents an Abstract Syntax Tree (AST) node for a Rant pattern. This is the base class for all Rant actions.
	/// </summary>
	internal abstract class RantAction
	{
		private readonly Stringe _stringe;

		public Stringe Stringe => _stringe;
		
		public RantAction(Stringe stringe)
		{
			_stringe = stringe;
		}

		/// <summary>
		/// Performs the operations defined in the action, given a specific sandbox to operate upon.
		/// </summary>
		/// <param name="sb">The sandbox on which to operate.</param>
		/// <returns></returns>
		public abstract IEnumerator<RantAction> Run(Sandbox sb);
	}
}