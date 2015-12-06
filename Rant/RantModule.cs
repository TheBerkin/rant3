using System.Collections.Generic;
using Rant.Engine.Syntax;
using Rant.Engine.ObjectModel;

namespace Rant
{
	public class RantModule
	{
		private ObjectStack _objects = new ObjectStack(new ObjectTable());

		public string Name { get; private set; }

		public RantModule(string name)
		{
			Name = name;
		}

		internal RantAction this[string name]
		{
			get
			{
				return (RantAction)_objects[name].Value;
			}
		}

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
