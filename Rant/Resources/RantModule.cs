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
		/// The name of the module.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Creates a new RantModule with the specified name.
		/// </summary>
		/// <param name="name">The name to assign to the module.</param>
		public RantModule(string name)
		{
			Name = name;
		}

		internal RantAction this[string name] => (RantAction)_objects[name].Value;

		/// <summary>
		/// Adds a RantPattern containing a subroutine to this module.
		/// </summary>
		/// <param name="name">The name of the function to add.</param>
		/// <param name="pattern">The pattern that will make up the body of the function.</param>
		public void AddSubroutineFunction(string name, RantPattern pattern)
		{
			var action = (pattern.Action.GetType() == typeof(RASequence) ?
				((RASequence)pattern.Action).Actions[0] :
				pattern.Action);
			if (action.GetType() != typeof(RADefineSubroutine))
				throw new RantRuntimeException(pattern, pattern.Code, "Attempted to add non-subroutine pattern to a module.");
			_objects[name] = new RantObject(action);
		}

		internal void AddActionFunction(string name, RantAction body)
		{
			_objects[name] = new RantObject(body);
		}
	}
}
