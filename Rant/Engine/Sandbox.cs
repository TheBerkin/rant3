using Rant.Engine.Compiler.Syntax;
using Rant.Engine.Constructs;
using Rant.Formats;
using System;
using System.Collections.Generic;
using System.Linq;

using Rant.Engine.ObjectModel;

namespace Rant.Engine
{
	/// <summary>
	/// Represents a Rant interpreter instance that produces a single output.
	/// </summary>
	internal class Sandbox
	{
		private readonly RantEngine _engine;
		private readonly OutputWriter _mainOutput;
		private readonly Stack<OutputWriter> _outputs; 
		private readonly RNG _rng;
		private readonly long _startingGen;
		private readonly RantFormat _format;
		private readonly RantPattern _pattern;
		private readonly ObjectStack _objects;
		private readonly Limit<int> _sizeLimit;
		
		private BlockAttribs _blockAttribs = new BlockAttribs();

		/// <summary>
		/// Gets the main output channel stack.
		/// </summary>
		public OutputWriter MainOutput => _mainOutput;

		/// <summary>
		/// Gets the current output channel stack.
		/// </summary>
		public OutputWriter CurrentOutput => _outputs.Peek();

		/// <summary>
		/// Gets the random number generator in use by the interpreter.
		/// </summary>
		public RNG RNG => _rng;

		/// <summary>
		/// Gets the currently set block attributes. 
		/// </summary>
		public BlockAttribs CurrentBlockAttribs => _blockAttribs;

		/// <summary>
		/// Gets the format used by the interpreter.
		/// </summary>
		public RantFormat Format => _format;

		/// <summary>
		/// The object stack used by the interpreter.
		/// </summary>
		public ObjectStack Objects => _objects;

		public Sandbox(RantEngine engine, RantPattern pattern, RNG rng, int sizeLimit = 0)
		{
			_format = engine.Format;
			_sizeLimit = new Limit<int>(0, sizeLimit, (a, b) => a + b, (a, b) => b == 0 || a <= b);
			_mainOutput = new OutputWriter(_format, _sizeLimit);
			_outputs = new Stack<OutputWriter>();
			_outputs.Push(_mainOutput);
			_rng = rng;
			_startingGen = rng.Generation;
			_pattern = pattern;
			_objects = new ObjectStack(engine.Objects);
		}

		/// <summary>
		/// Prints the specified value to the output channel stack.
		/// </summary>
		/// <param name="obj">The value to print.</param>
		public void Print(object obj) => CurrentOutput.Write(obj?.ToString() ?? String.Empty);

		public void AddOutputWriter() => _outputs.Push(new OutputWriter(_format, _sizeLimit));

		public RantOutput PopOutput() => new RantOutput(_rng.Seed, _startingGen, _outputs.Pop().Channels);

		/// <summary>
		/// Dequeues the current block attribute set and returns it, queuing a new attribute set.
		/// </summary>
		/// <returns></returns>
		public BlockAttribs NextAttribs()
		{
			var ba = _blockAttribs;
			_blockAttribs = new BlockAttribs();
			return ba;
		}

		public RantOutput Run(double timeout)
		{
			var callStack = new Stack<IEnumerator<RantAction>>();

			// Push the AST root
			callStack.Push(_pattern.Action.Run(this));

			top:
			while (callStack.Any())
			{
				// Get the topmost call stack item
				var action = callStack.Peek();

				// Execute the node until it runs out of children
				while (action.MoveNext())
				{
					if (callStack.Count == RantEngine.MaxStackSize)
						throw new RantRuntimeException(_pattern, null, $"Exceeded the maximum stack size ({RantEngine.MaxStackSize}).");

					// Push child node onto stack and start over
					callStack.Push(action.Current.Run(this));
					goto top;
				}

				// Remove node once finished
				callStack.Pop();
			}
			
			return PopOutput();
		}
	}
}