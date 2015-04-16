using Rant.Engine.Syntax;
using Rant.Engine.Constructs;
using Rant.Formats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Rant.Engine.ObjectModel;
using Rant.Vocabulary;

namespace Rant.Engine
{
	/// <summary>
	/// Represents a Rant interpreter instance that produces a single output.
	/// </summary>
	internal class Sandbox
	{
		private readonly RantEngine _engine;
		private readonly ChannelWriter _mainOutput;
		private readonly Stack<ChannelWriter> _outputs; 
		private readonly RNG _rng;
		private readonly long _startingGen;
		private readonly RantFormat _format;
		private readonly RantPattern _pattern;
		private readonly ObjectStack _objects;
		private readonly Limit _sizeLimit;
		private readonly Stack<BlockState> _blocks;
		private readonly Stack<Match> _matches;
		private readonly QueryState _queryState;
		private readonly Stack<Dictionary<string, RantAction>> _subroutineArgs;
		private readonly SyncManager _syncManager;
		
		private BlockAttribs _blockAttribs = new BlockAttribs();

		/// <summary>
		/// Gets the engine instance to which the sandbox is bound.
		/// </summary>
		public RantEngine Engine => _engine;

		/// <summary>
		/// Gets the main output channel stack.
		/// </summary>
		public ChannelWriter MainOutput => _mainOutput;

		/// <summary>
		/// Gets the current output channel stack.
		/// </summary>
		public ChannelWriter CurrentOutput => _outputs.Peek();

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
		/// Gest the object stack used by the interpreter.
		/// </summary>
		public ObjectStack Objects => _objects;

		/// <summary>
		/// Gets the block state stack.
		/// </summary>
		public Stack<BlockState> Blocks => _blocks;

		/// <summary>
		/// Gets the replacer match stack. The topmost item is the current match for the current replacer.
		/// </summary>
		public Stack<Match> RegexMatches => _matches;

		/// <summary>
		/// Gets the current query state.
		/// </summary>
		public QueryState QueryState => _queryState;

		/// <summary>
		/// Gets the current RantPattern.
		/// </summary>
		public RantPattern Pattern => _pattern;

		public Stack<Dictionary<string, RantAction>> SubroutineArgs => _subroutineArgs;

		/// <summary>
		/// Gets the synchronizer manager instance for the current Sandbox.
		/// </summary>
		public SyncManager SyncManager => _syncManager;

		public Sandbox(RantEngine engine, RantPattern pattern, RNG rng, int sizeLimit = 0)
		{
			_engine = engine;
			_format = engine.Format;
			_sizeLimit = new Limit(sizeLimit);
			_mainOutput = new ChannelWriter(_format, _sizeLimit);
			_outputs = new Stack<ChannelWriter>();
			_outputs.Push(_mainOutput);
			_rng = rng;
			_startingGen = rng.Generation;
			_pattern = pattern;
			_objects = new ObjectStack(engine.Objects);
			_blocks = new Stack<BlockState>();
			_matches = new Stack<Match>();
			_queryState = new QueryState();
			_subroutineArgs = new Stack<Dictionary<string, RantAction>>();
			_syncManager = new SyncManager(this);
		}

		/// <summary>
		/// Prints the specified value to the output channel stack.
		/// </summary>
		/// <param name="obj">The value to print.</param>
		public void Print(object obj) => CurrentOutput.Write(obj);

		public void PrintMany(Func<char> generator, int times)
		{
			if (times == 1)
			{
				CurrentOutput.Write(generator());
				return;
			}
			var buffer = new StringBuilder();
			for (int i = 0; i < times; i++) buffer.Append(generator());
			CurrentOutput.Write(buffer);
		}

		public void PrintMany(Func<string> generator, int times)
		{
			if (times == 1)
			{
				CurrentOutput.Write(generator());
				return;
			}
			var buffer = new StringBuilder();
			for (int i = 0; i < times; i++) buffer.Append(generator());
			CurrentOutput.Write(buffer);
		}

		public void AddOutputWriter() => _outputs.Push(new ChannelWriter(_format, _sizeLimit));

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
			IEnumerator<RantAction> action;

			// Push the AST root
			callStack.Push(_pattern.Action.Run(this));

			top:
			while (callStack.Any())
			{
				// Get the topmost call stack item
				action = callStack.Peek();

				// Execute the node until it runs out of children
				while (action.MoveNext())
				{
					if (callStack.Count == RantEngine.MaxStackSize)
						throw new RantRuntimeException(_pattern, action.Current.Range,
							$"Exceeded the maximum stack size ({RantEngine.MaxStackSize}).");

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