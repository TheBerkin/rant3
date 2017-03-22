using Rant.Core.Compiler.Syntax;
using Rant.Core.Constructs;
using Rant.Core.ObjectModel;
using Rant.Core.Output;
using Rant.Core.Utilities;
using Rant.Formats;
using Rant.Resources;
using Rant.Vocabulary.Querying;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Rant.Core
{
	internal sealed partial class Sandbox
	{
		private readonly BlockAttribManager _blockManager;
		private readonly Stack<OutputWriter> _outputs;
		private readonly Stopwatch _stopwatch;
		private bool _plural = false;
		private int _quoteLevel = 0;
		private bool shouldYield = false;
		private Stack<RantOutput> _redirOutputs;

		public QueryBuilder QueryBuilder { get; } = new QueryBuilder();

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
		/// Gets the format used by the interpreter.
		/// </summary>
		public RantFormat Format { get; }

		/// <summary>
		/// Gets the object stack used by the interpreter.
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
		public RantProgram Pattern { get; }

		/// <summary>
		/// Subroutine argument stack.
		/// </summary>
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
		public RantProgramArgs PatternArgs { get; }

		/// <summary>
		/// The block manager.
		/// </summary>
		public BlockAttribManager AttribManager => _blockManager;
	}
}
