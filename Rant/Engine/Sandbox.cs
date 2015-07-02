using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Rant.Engine.ObjectModel;
using Rant.Vocabulary;
using Rant.Engine.Syntax;
using Rant.Engine.Constructs;
using Rant.Engine.Syntax.Richard;
using Rant.Formats;

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
		private readonly Stack<object> _scriptObjectStack;
        private readonly Stopwatch _stopwatch;

		private BlockAttribs _newAttribs = new BlockAttribs();
        private BlockManager _blockManager;
		private int _quoteLevel = 0;

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

        /// <summary>
        /// Gets the current RantAction being executed.
        /// </summary>
        public RantAction CurrentAction { get; private set; }

		/// <summary>
		/// Gets the last used timeout.
		/// </summary>
		public double LastTimeout { get; internal set; }

		public Stack<object> ScriptObjectStack => _scriptObjectStack;

        /// <summary>
        /// Gets or sets the expected result for the current flag condition.
        /// </summary>
        public bool FlagConditionExpectedResult { get; set; }

        /// <summary>
        /// Gets a collection of the flags currently being used for the flag condition.
        /// </summary>
        public HashSet<string> ConditionFlags { get; } = new HashSet<string>(); 

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
            _blockManager = new BlockManager();
			_scriptObjectStack = new Stack<object>();
            _stopwatch = new Stopwatch();
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

		public RantOutput Return() => new RantOutput(_rng.Seed, _startingGen, _outputs.Pop());

		public void IncreaseQuote() => _quoteLevel++;

		public void DecreaseQuote() => _quoteLevel--;

		public void PrintOpeningQuote()
			=> CurrentOutput.Write(_quoteLevel == 1 ? _format.OpeningPrimaryQuote : _format.OpeningSecondaryQuote);

		public void PrintClosingQuote()
			=> CurrentOutput.Write(_quoteLevel == 1 ? _format.ClosingPrimaryQuote : _format.ClosingSecondaryQuote);

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

		public RantOutput EvalPattern(double timeout, RantPattern pattern)
		{
			_outputs.Push(new ChannelWriter(_format, _sizeLimit));
			return Run(timeout, pattern);
		}

		public RantOutput Run(double timeout, RantPattern pattern = null)
		{
			if (pattern == null)
				pattern = _pattern;
			LastTimeout = timeout;

            _scriptObjectStack.Clear();
            _objects.Clear();

			var callStack = new Stack<IEnumerator<RantAction>>();
			var actionStack = new Stack<RantAction>();
			IEnumerator<RantAction> action;

			long timeoutMS = (long)(timeout * 1000);
			bool timed = timeoutMS > 0;
            bool stopwatchAlreadyRunning = _stopwatch.IsRunning;

		    if (!_stopwatch.IsRunning)
		    {
		        _stopwatch.Reset();
		        _stopwatch.Start();
		    }

			// Push the AST root
		    CurrentAction = pattern.Action;
			actionStack.Push(CurrentAction);
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

                    if (action.Current == null)
                        break;

                    // i'm sorry to put this here but it's the only way to do it!!
                    if (action.Current is REABreak)
                    {
                        // move back up the call stack until we pass something "breakable"
                        while (
                            callStack.Any() &&
                            (callStack.Peek().Current is RantExpressionAction) &&
                            !(callStack.Peek().Current as RantExpressionAction).Breakable)
                        {
                            actionStack.Pop();
                            callStack.Pop();
                        }
                        // there was nothing to break from
                        if (!callStack.Any() || !(callStack.Peek().Current is RantExpressionAction))
                            throw new RantRuntimeException(Pattern, (action.Current as REABreak).Range, "Nothing to break from.");
                        goto top;
                    }

					// Push child node onto stack and start over
				    CurrentAction = action.Current;
					actionStack.Push(CurrentAction);
					callStack.Push(action.Current.Run(this));
					goto top;
				}
				var lastAction = actionStack.Peek();
				// Special processing for scripts
				if (lastAction is RantExpressionAction)
				{
					var val = (lastAction as RantExpressionAction).GetValue(this);
					if(val != null)
						_scriptObjectStack.Push(val);
				}
                // i also wish this could be moved to somewhere else 
                // but someone else figure out how to do that for me
                if (lastAction is REAReturn)
                {
                    // same thing as the processing for break
                    // todo: abstract this, DRY
                    while (
                        actionStack.Any() &&
                        (actionStack.Peek() is RantExpressionAction) &&
                        !(actionStack.Peek() as RantExpressionAction).Returnable)
                    {
                        actionStack.Pop();
                        callStack.Pop();
                    }

                    if (!(lastAction as REAReturn).HasReturnValue)
                        _scriptObjectStack.Push(new RantObject(RantObjectType.Undefined));
                    // if we're not returning from anything, we've returned ourself from the Richard scope
                    goto top;
                }


				// Remove node once finished
				callStack.Pop();
                actionStack.Pop();
			}

            if(!stopwatchAlreadyRunning)
			    _stopwatch.Stop();

			return Return();
		}
	}
}