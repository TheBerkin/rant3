using Rant.Core.Compiler;
using Rant.Core.Compiler.Syntax;
using Rant.Core.ObjectModel;

namespace Rant.Resources
{
	/// <summary>
	/// Represents a collection of subroutines that can be loaded and used by other patterns.
	/// </summary>
	public sealed class RantModule
	{
		private readonly ObjectStack _objects = new ObjectStack(new ObjectTable());

		/// <summary>
		/// Creates a new RantModule with the specified name.
		/// </summary>
		/// <param name="name">The name to assign to the module.</param>
		public RantModule(string name)
		{
			Name = name;
		}

		/// <summary>
		/// The name of the module.
		/// </summary>
		public string Name { get; }

		internal RST this[string name] => (RST)_objects[name].Value;

		/// <summary>
		/// Adds a RantPattern containing a subroutine to this module.
		/// </summary>
		/// <param name="name">The name of the function to add.</param>
		/// <param name="pattern">The pattern that will make up the body of the function.</param>
		public void AddSubroutineFunction(string name, RantPattern pattern)
		{
			var action = (pattern.SyntaxTree.GetType() == typeof(RstSequence)
				? ((RstSequence)pattern.SyntaxTree).Actions[0]
				: pattern.SyntaxTree);
			if (action.GetType() != typeof(RstDefineSubroutine))
				throw new RantRuntimeException(pattern, TokenLocation.Unknown,
					"Attempted to add non-subroutine pattern to a module.");
			_objects[name] = new RantObject(action);
		}

		internal void AddActionFunction(string name, RST body)
		{
			_objects[name] = new RantObject(body);
		}
	}
}