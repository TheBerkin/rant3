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

using static Rant.Localization.Txtres;

namespace Rant.Core
{
	/// <summary>
	/// Represents a Rant interpreter instance that produces a single output.
	/// </summary>
	internal class Sandbox
	{
		private static readonly object fallbackArgsLockObj = new object();
		private readonly BlockManager _blockManager;
		private readonly Stack<OutputWriter> _outputs;
		private readonly Stopwatch _stopwatch;
		private int _quoteLevel = 0;

		/// <summary>
		/// Gets the currently loaded modules.
		/// </summary>
		public Dictionary<string, RantModule> Modules = new Dictionary<string, RantModule>();

		/// <summary>
		/// Modules that were loaded from packages.
		/// </summary>
		public Dictionary<string, RantModule> PackageModules = new Dictionary<string, RantModule>();

		private bool shouldYield = false;

		/// <summary>
		/// Modules that were not loaded from code, but were provided to RantEngine by the user.
		/// </summary>
		public Dictionary<string, RantModule> UserModules = new Dictionary<string, RantModule>();

		public Sandbox(RantEngine engine, RantPattern pattern, RNG rng, int sizeLimit = 0, CarrierState carrierState = null,
			RantPatternArgs args = null)
		{
			Engine = engine;
			Format = engine.Format;
			SizeLimit = new Limit(sizeLimit);
			BaseOutput = new OutputWriter(this);
			_outputs = new Stack<OutputWriter>();
			_outputs.Push(BaseOutput);
			RNG = rng;
			StartingGen = rng.Generation;
			Pattern = pattern;
			Objects = new ObjectStack(engine.Objects);
			Blocks = new Stack<BlockState>();
			RegexMatches = new Stack<Match>();
			CarrierState = carrierState ?? new CarrierState();
			SubroutineArgs = new Stack<Dictionary<string, RST>>();
			SyncManager = new SyncManager(this);
			_blockManager = new BlockManager();
			ScriptObjectStack = new Stack<object>();
			PatternArgs = args;
			_stopwatch = new Stopwatch();
		}

		/// <summary>
		/// Gets the engine instance to which the sandbox is bound.
		/// </summary>
		public RantEngine Engine { get; }

		/// <summary>
		/// Gets the main output channel stack.
		/// </summary>
		public OutputWriter BaseOutput { get; }

		/// <summary>
		/// Gets the current output channel stack.
		/// </summary>
		public OutputWriter Output => _outputs.Peek();

		/// <summary>
		/// Gets the random number generator in use by the interpreter.
		/// </summary>
		public RNG RNG { get; }

		/// <summary>
		/// The starting generation of the RNG.
		/// </summary>
		public long StartingGen { get; }

		/// <summary>
		/// Gets the currently set block attributes.
		/// </summary>
		public BlockAttribs CurrentBlockAttribs { get; private set; } = new BlockAttribs();

		/// <summary>
		/// Gets the format used by the interpreter.
		/// </summary>
		public RantFormat Format { get; }

		/// <summary>
		/// Gest the object stack used by the interpreter.
		/// </summary>
		public ObjectStack Objects { get; }

		/// <summary>
		/// Gets the block state stack.
		/// </summary>
		public Stack<BlockState> Blocks { get; }

		/// <summary>
		/// Gets the replacer match stack. The topmost item is the current match for the current replacer.
		/// </summary>
		public Stack<Match> RegexMatches { get; }

		/// <summary>
		/// Gets the current query state.
		/// </summary>
		public CarrierState CarrierState { get; }

		/// <summary>
		/// Gets the current RantPattern.
		/// </summary>
		public RantPattern Pattern { get; }

		public Stack<Dictionary<string, RST>> SubroutineArgs { get; }

		/// <summary>
		/// Gets the synchronizer manager instance for the current Sandbox.
		/// </summary>
		public SyncManager SyncManager { get; }

		/// <summary>
		/// Gets the size limit for the pattern.
		/// </summary>
		public Limit SizeLimit { get; }

		/// <summary>
		/// Gets the current RantAction being executed.
		/// </summary>
		public RST CurrentAction { get; private set; }

		/// <summary>
		/// Gets the last used timeout.
		/// </summary>
		public double LastTimeout { get; internal set; }

		/// <summary>
		/// Gets the current object stack of the Richard engine.
		/// </summary>
		public Stack<object> ScriptObjectStack { get; }

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
		public RantPatternArgs PatternArgs { get; }

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
			=> Output.Do(chain => chain.Print(_quoteLevel == 1 ? Format.OpeningPrimaryQuote : Format.OpeningSecondaryQuote));

		public void PrintClosingQuote()
			=> Output.Do(chain => chain.Print(_quoteLevel == 1 ? Format.ClosingPrimaryQuote : Format.ClosingSecondaryQuote));

		/// <summary>
		/// Dequeues the current block attribute set and returns it, queuing a new attribute set.
		/// </summary>
		/// <returns></returns>
		public BlockAttribs NextAttribs(RstBlock block)
		{
			var attribs = CurrentBlockAttribs;

			_blockManager.Add(attribs, block);
			_blockManager.SetPrevAttribs(attribs);

			switch (attribs.Persistence)
			{
				case AttribPersistence.Off:
					CurrentBlockAttribs = new BlockAttribs();
					break;

				case AttribPersistence.On:
					CurrentBlockAttribs = new BlockAttribs();
					break;

				case AttribPersistence.Once:
					CurrentBlockAttribs = _blockManager.GetPrevious(1);
					break;
			}

			return attribs;
		}

		public void SetYield() => shouldYield = true;

		public RantOutput Run(double timeout, RantPattern pattern = null)
		{
			lock (PatternArgs ?? fallbackArgsLockObj)
			{
				if (pattern == null) pattern = Pattern;
				LastTimeout = timeout;
				long timeoutMS = (long)(timeout * 1000);
				bool timed = timeoutMS > 0;
				bool stopwatchAlreadyRunning = _stopwatch.IsRunning;
				if (!_stopwatch.IsRunning)
				{
					_stopwatch.Reset();
					_stopwatch.Start();
				}

				ScriptObjectStack.Clear();
				var callStack = new Stack<IEnumerator<RST>>();
				IEnumerator<RST> action;

				// Push the AST root
				CurrentAction = pattern.SyntaxTree;
				callStack.Push(pattern.SyntaxTree.Run(this));

				top:
				while (callStack.Any())
				{
					// Get the topmost call stack item
					action = callStack.Peek();

					// Execute the node until it runs out of children
					while (action.MoveNext())
					{
						if (timed && _stopwatch.ElapsedMilliseconds >= timeoutMS)
							throw new RantRuntimeException(pattern, action.Current.Location,
								GetString("err-pattern-timeout", timeout));

						if (callStack.Count >= RantEngine.MaxStackSize)
							throw new RantRuntimeException(pattern, action.Current.Location,
								GetString("err-stack-overflow", RantEngine.MaxStackSize));

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
			lock (PatternArgs ?? fallbackArgsLockObj)
			{
				if (pattern == null) pattern = Pattern;
				LastTimeout = timeout;
				long timeoutMS = (long)(timeout * 1000);
				bool timed = timeoutMS > 0;
				bool stopwatchAlreadyRunning = _stopwatch.IsRunning;
				if (!_stopwatch.IsRunning)
				{
					_stopwatch.Reset();
					_stopwatch.Start();
				}

				ScriptObjectStack.Clear();
				var callStack = new Stack<IEnumerator<RST>>();
				IEnumerator<RST> action;

				// Push the AST root
				CurrentAction = pattern.SyntaxTree;
				callStack.Push(pattern.SyntaxTree.Run(this));

				top:
				while (callStack.Any())
				{
					// Get the topmost call stack item
					action = callStack.Peek();

					// Execute the node until it runs out of children
					while (action.MoveNext())
					{
						if (timed && _stopwatch.ElapsedMilliseconds >= timeoutMS)
							throw new RantRuntimeException(pattern, action.Current.Location,
								GetString("err-pattern-timeout", timeout));

						if (callStack.Count >= RantEngine.MaxStackSize)
							throw new RantRuntimeException(pattern, action.Current.Location,
								GetString("err-stack-overflow", RantEngine.MaxStackSize));

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