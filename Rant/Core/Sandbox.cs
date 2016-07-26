using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Rant.Core.Compiler.Syntax;
using Rant.Core.Constructs;
using Rant.Core.ObjectModel;
using Rant.Core.Output;
using Rant.Core.Utilities;
using Rant.Formats;
using Rant.Resources;
using Rant.Vocabulary.Querying;

namespace Rant.Core
{
	/// <summary>
	/// Represents a Rant interpreter instance that produces a single output.
	/// </summary>
	internal class Sandbox
	{
		private static readonly object fallbackArgsLockObj = new object();

		private readonly RantEngine _engine;
		private readonly OutputWriter _baseOutput;
		private readonly Stack<OutputWriter> _outputs;
		private readonly RNG _rng;
		private readonly long _startingGen;
		private readonly RantFormat _format;
		private readonly RantPattern _pattern;
		private readonly ObjectStack _objects;
		private readonly Limit _sizeLimit;
		private readonly Stack<BlockState> _blocks;
		private readonly Stack<Match> _matches;
		private readonly CarrierState _queryState;
		private readonly Stack<Dictionary<string, RantAction>> _subroutineArgs;
		private readonly SyncManager _syncManager;
		private readonly Stack<object> _scriptObjectStack;
		private readonly Stopwatch _stopwatch;
		private readonly BlockManager _blockManager;
		private readonly RantPatternArgs _patternArgs;

		private BlockAttribs _newAttribs = new BlockAttribs();
		private int _quoteLevel = 0;
		private bool shouldYield = false;

		/// <summary>
		/// Gets the engine instance to which the sandbox is bound.
		/// </summary>
		public RantEngine Engine => _engine;

		/// <summary>
		/// Gets the main output channel stack.
		/// </summary>
		public OutputWriter BaseOutput => _baseOutput;

		/// <summary>
		/// Gets the current output channel stack.
		/// </summary>
		public OutputWriter Output => _outputs.Peek();

		/// <summary>
		/// Gets the random number generator in use by the interpreter.
		/// </summary>
		public RNG RNG => _rng;

		/// <summary>
		/// The starting generation of the RNG.
		/// </summary>
		public long StartingGen => _startingGen;

		/// <summary>
		/// Gets the currently set block attributes. 
		/// </summary>
		public BlockAttribs CurrentBlockAttribs => _newAttribs;

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
		public CarrierState QueryState => _queryState;

		/// <summary>
		/// Gets the current RantPattern.
		/// </summary>
		public RantPattern Pattern => _pattern;

		public Stack<Dictionary<string, RantAction>> SubroutineArgs => _subroutineArgs;

		/// <summary>
		/// Gets the synchronizer manager instance for the current Sandbox.
		/// </summary>
		public SyncManager SyncManager => _syncManager;

		/// <summary>
		/// Gets the size limit for the pattern.
		/// </summary>
		public Limit SizeLimit => _sizeLimit;

		/// <summary>
		/// Gets the current RantAction being executed.
		/// </summary>
		public RantAction CurrentAction { get; private set; }

		/// <summary>
		/// Gets the last used timeout.
		/// </summary>
		public double LastTimeout { get; internal set; }

		/// <summary>
		/// Gets the current object stack of the Richard engine.
		/// </summary>
		public Stack<object> ScriptObjectStack => _scriptObjectStack;

		/// <summary>
		/// Gets or sets the expected result for the current flag condition.
		/// </summary>
		public bool FlagConditionExpectedResult { get; set; }

		/// <summary>
		/// Gets a collection of the flags currently being used for the flag condition.
		/// </summary>
		public HashSet<string> ConditionFlags { get; } = new HashSet<string>();

		/// <summary>
		/// Gets the arguments passed to the pattern.
		/// </summary>
		public RantPatternArgs PatternArgs => _patternArgs;

		/// <summary>
		/// Gets the currently loaded modules.
		/// </summary>
		public Dictionary<string, RantModule> Modules = new Dictionary<string, RantModule>();

		/// <summary>
		/// Modules that were not loaded from code, but were provided to RantEngine by the user.
		/// </summary>
		public Dictionary<string, RantModule> UserModules = new Dictionary<string, RantModule>();

		/// <summary>
		/// Modules that were loaded from packages.
		/// </summary>
		public Dictionary<string, RantModule> PackageModules = new Dictionary<string, RantModule>();

		public Sandbox(RantEngine engine, RantPattern pattern, RNG rng, int sizeLimit = 0, RantPatternArgs args = null)
		{
			_engine = engine;
			_format = engine.Format;
			_sizeLimit = new Limit(sizeLimit);
			_baseOutput = new OutputWriter(this);
			_outputs = new Stack<OutputWriter>();
			_outputs.Push(_baseOutput);
			_rng = rng;
			_startingGen = rng.Generation;
			_pattern = pattern;
			_objects = new ObjectStack(engine.Objects);
			_blocks = new Stack<BlockState>();
			_matches = new Stack<Match>();
			_queryState = new CarrierState();
			_subroutineArgs = new Stack<Dictionary<string, RantAction>>();
			_syncManager = new SyncManager(this);
			_blockManager = new BlockManager();
			_scriptObjectStack = new Stack<object>();
			_patternArgs = args;
			_stopwatch = new Stopwatch();
		}

		/// <summary>
		/// Prints the specified value to the output channel stack.
		/// </summary>
		/// <param name="obj">The value to print.</param>
		public void Print(object obj) => Output.Do(chain => chain.Print(obj));

		public void PrintMany(Func<char> generator, int times)
		{
			if (times == 1)
			{
				Output.Do(chain => chain.Print(generator()));
				return;
			}
			var buffer = new StringBuilder();
			for (int i = 0; i < times; i++) buffer.Append(generator());
			Output.Do(chain => chain.Print(buffer));
		}

		public void PrintMany(Func<string> generator, int times)
		{
			if (times == 1)
			{
				Output.Do(chain => chain.Print(generator()));
				return;
			}
			var buffer = new StringBuilder();
			for (int i = 0; i < times; i++) buffer.Append(generator());
			Output.Do(chain => chain.Print(buffer));
		}

		public void AddOutputWriter() => _outputs.Push(new OutputWriter(this));

		public RantOutput Return() => _outputs.Pop().ToRantOutput();

		public void IncreaseQuote() => _quoteLevel++;

		public void DecreaseQuote() => _quoteLevel--;

		public void PrintOpeningQuote()
			=> Output.Do(chain => chain.Print(_quoteLevel == 1 ? _format.OpeningPrimaryQuote : _format.OpeningSecondaryQuote));

		public void PrintClosingQuote()
			=> Output.Do(chain => chain.Print(_quoteLevel == 1 ? _format.ClosingPrimaryQuote : _format.ClosingSecondaryQuote));

		/// <summary>
		/// Dequeues the current block attribute set and returns it, queuing a new attribute set.
		/// </summary>
		/// <returns></returns>
		public BlockAttribs NextAttribs(RABlock block)
		{
			BlockAttribs attribs = _newAttribs;

			_blockManager.Add(attribs, block);
			_blockManager.SetPrevAttribs(attribs);

			switch (attribs.Persistence)
			{
				case AttribPersistence.Off:
					_newAttribs = new BlockAttribs();
					break;

				case AttribPersistence.On:
					_newAttribs = new BlockAttribs();
					break;

				case AttribPersistence.Once:
					_newAttribs = _blockManager.GetPrevious(1);
					break;
			}

			return attribs;
		}

		public void SetYield() => shouldYield = true;

		public RantOutput Run(double timeout, RantPattern pattern = null)
		{
			lock (_patternArgs ?? fallbackArgsLockObj)
			{
				if (pattern == null) pattern = _pattern;
				LastTimeout = timeout;
				long timeoutMS = (long)(timeout * 1000);
				bool timed = timeoutMS > 0;
				bool stopwatchAlreadyRunning = _stopwatch.IsRunning;
				if (!_stopwatch.IsRunning)
				{
					_stopwatch.Reset();
					_stopwatch.Start();
				}

				_scriptObjectStack.Clear();
				var callStack = new Stack<IEnumerator<RantAction>>();
				IEnumerator<RantAction> action;

				// Push the AST root
				CurrentAction = pattern.Action;
				callStack.Push(pattern.Action.Run(this));

				top:
				while (callStack.Any())
				{
					// Get the topmost call stack item
					action = callStack.Peek();

					// Execute the node until it runs out of children
					while (action.MoveNext())
					{
						if (timed && _stopwatch.ElapsedMilliseconds >= timeoutMS)
							throw new RantRuntimeException(pattern, action.Current.Range,
								$"The pattern has timed out ({timeout}s).");

						if (callStack.Count >= RantEngine.MaxStackSize)
							throw new RantRuntimeException(pattern, action.Current.Range,
								$"Exceeded the maximum stack size ({RantEngine.MaxStackSize}).");

						if (action.Current == null) break;

						// Push child node onto stack and start over
						CurrentAction = action.Current;
						callStack.Push(CurrentAction.Run(this));
						goto top;
					}

					// Remove node once finished
					callStack.Pop();
				}

				if (!stopwatchAlreadyRunning) _stopwatch.Stop();

				return Return();
			}
		}

		public IEnumerable<RantOutput> RunSerial(double timeout, RantPattern pattern = null)
		{
			lock (_patternArgs ?? fallbackArgsLockObj)
			{
				if (pattern == null) pattern = _pattern;
				LastTimeout = timeout;
				long timeoutMS = (long)(timeout * 1000);
				bool timed = timeoutMS > 0;
				bool stopwatchAlreadyRunning = _stopwatch.IsRunning;
				if (!_stopwatch.IsRunning)
				{
					_stopwatch.Reset();
					_stopwatch.Start();
				}

				_scriptObjectStack.Clear();
				var callStack = new Stack<IEnumerator<RantAction>>();
				IEnumerator<RantAction> action;

				// Push the AST root
				CurrentAction = pattern.Action;
				callStack.Push(pattern.Action.Run(this));

				top:
				while (callStack.Any())
				{
					// Get the topmost call stack item
					action = callStack.Peek();

					// Execute the node until it runs out of children
					while (action.MoveNext())
					{
						if (timed && _stopwatch.ElapsedMilliseconds >= timeoutMS)
							throw new RantRuntimeException(pattern, action.Current.Range,
								$"The pattern has timed out ({timeout}s).");

						if (callStack.Count >= RantEngine.MaxStackSize)
							throw new RantRuntimeException(pattern, action.Current.Range,
								$"Exceeded the maximum stack size ({RantEngine.MaxStackSize}).");

						if (action.Current == null) break;

						// Push child node onto stack and start over
						CurrentAction = action.Current;
						callStack.Push(CurrentAction.Run(this));
						goto top;
					}

					if (shouldYield)
					{
						shouldYield = false;
						yield return Return();
						AddOutputWriter();
					}

					// Remove node once finished
					callStack.Pop();
				}

				if (!stopwatchAlreadyRunning) _stopwatch.Stop();
			}
		}
	}
}