#region License

// https://github.com/TheBerkin/Rant
// 
// Copyright (c) 2017 Nicholas Fleck
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in the
// Software without restriction, including without limitation the rights to use, copy,
// modify, merge, publish, distribute, sublicense, and/or sell copies of the Software,
// and to permit persons to whom the Software is furnished to do so, subject to the
// following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
// PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE
// OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

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
using Rant.Vocabulary.Querying;

using static Rant.Localization.Txtres;

namespace Rant.Core
{
	/// <summary>
	/// Represents a Rant interpreter instance that produces a single output.
	/// </summary>
	internal sealed partial class Sandbox
	{
		private static readonly object fallbackArgsLockObj = new object();

		public Sandbox(RantEngine engine, RantProgram pattern, RNG rng, int sizeLimit = 0, CarrierState carrierState = null,
			RantProgramArgs args = null)
		{
			// Public members
			RNG = rng;
			StartingGen = rng.Generation;
			Engine = engine;
			Format = engine.Format;
			SizeLimit = new Limit(sizeLimit);			
			Pattern = pattern;
			Objects = new ObjectStack(engine.Objects);
			Blocks = new Stack<BlockState>();
			RegexMatches = new Stack<Match>();
			CarrierState = carrierState ?? new CarrierState();
			SubroutineArgs = new Stack<Dictionary<string, RST>>();
			SyncManager = new SyncManager(this);			
			PatternArgs = args;

			// Private members
			_blockManager = new BlockManager();
			_stopwatch = new Stopwatch();			

			// Output initialization
			BaseOutput = new OutputWriter(this);
			_outputs = new Stack<OutputWriter>();
			_outputs.Push(BaseOutput);
		}

		/// <summary>
		/// Prints the specified value to the output channel stack.
		/// </summary>
		/// <param name="obj">The value to print.</param>
		public void Print(object obj) => Output.Do(chain => chain.Print(obj));

		public void SetPlural(bool plural) => _plural = plural;

		public bool TakePlural()
		{
			bool p = _plural;
			_plural = false;
			return p;
		}

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
			=> Output.Do(chain => chain.Print(_quoteLevel == 1 ? Format.WritingSystem.Quotations.OpeningPrimary : Format.WritingSystem.Quotations.OpeningSecondary));

		public void PrintClosingQuote()
			=> Output.Do(chain => chain.Print(_quoteLevel == 1 ? Format.WritingSystem.Quotations.ClosingPrimary : Format.WritingSystem.Quotations.ClosingSecondary));

		public void SetYield() => shouldYield = true;

		public void PushRedirectedOutput()
		{
			if (_redirOutputs == null) _redirOutputs = new Stack<RantOutput>();
			_redirOutputs.Push(_outputs.Pop().ToRantOutput());
		}

		public RantOutput PopRedirectedOutput() => _redirOutputs.Pop();

		public RantOutput GetRedirectedOutput() => _redirOutputs.Count > 0 ? _redirOutputs.Peek() : null;

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
					CurrentBlockAttribs = _blockManager.GetPrevious();
					break;
			}

			return attribs;
		}

		public RantOutput Run(double timeout, RantProgram pattern = null)
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
						{
							throw new RantRuntimeException(pattern, action.Current.Location,
								GetString("err-pattern-timeout", timeout));
						}

						if (callStack.Count >= RantEngine.MaxStackSize)
						{
							throw new RantRuntimeException(pattern, action.Current.Location,
								GetString("err-stack-overflow", RantEngine.MaxStackSize));
						}

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

		public IEnumerable<RantOutput> RunSerial(double timeout, RantProgram pattern = null)
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
						{
							throw new RantRuntimeException(pattern, action.Current.Location,
								GetString("err-pattern-timeout", timeout));
						}

						if (callStack.Count >= RantEngine.MaxStackSize)
						{
							throw new RantRuntimeException(pattern, action.Current.Location,
								GetString("err-stack-overflow", RantEngine.MaxStackSize));
						}

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