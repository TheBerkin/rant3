using System;
using System.Collections.Generic;
using System.Linq;

using Rant.Core.Compiler.Parsing;
using Rant.Core.Compiler.Syntax;
using Rant.Core.Stringes;
using Rant.Resources;

using static Rant.Localization.Txtres;

namespace Rant.Core.Compiler
{
	internal class RantCompiler
	{
		private readonly Stringe _source;
		private readonly string _sourceName;
		private readonly TokenReader _reader;
		private readonly Stack<CompileContext> _contextStack = new Stack<CompileContext>();
		private Action<RST> _nextActionCallback = null;

		private List<RantCompilerMessage> _errors;
		// private List<RantCompilerMessage> _warnings;
		internal bool HasModule = false;
		internal readonly RantModule Module;
		internal CompileContext NextContext => _contextStack.Peek();

		public RantCompiler(string sourceName, string source)
		{
			Module = new RantModule(sourceName);

			_sourceName = sourceName;
			_source = source.ToStringe();
			_reader = new TokenReader(sourceName, RantLexer.GenerateTokens(sourceName, _source, SyntaxError), this);
		}

		public Stringe Source => _source;

		public void AddContext(CompileContext context) => _contextStack.Push(context);

		public void LeaveContext() => _contextStack.Pop();

		public void SetNextActionCallback(Action<RST> callback) => _nextActionCallback = callback;

		public RST Compile()
		{
			try
			{
				var parser = Parser.Get<SequenceParser>();
				var stack = new Stack<IEnumerator<Parser>>();
				var actionList = new List<RST>();
				_contextStack.Push(CompileContext.DefaultSequence);

				_nextActionCallback = a => actionList.Add(a);

				stack.Push(parser.Parse(this, NextContext, _reader, _nextActionCallback));

				top:
				while (stack.Any())
				{
					var p = stack.Peek();

					while (p.MoveNext())
					{
						if (p.Current == null) continue;

						stack.Push(p.Current.Parse(this, NextContext, _reader, _nextActionCallback));
						goto top;
					}

					stack.Pop();
				}

				if (_errors?.Count > 0)
					throw new RantCompilerException(_sourceName, _errors);

				return new RstSequence(actionList, _source);
			}

			catch (RantCompilerException)
			{
				throw;
			}
#if !DEBUG
			catch (Exception ex)
			{
				throw new RantCompilerException(_sourceName, _errors, ex);
			}
#endif
		}

		public void SyntaxError(Stringe token, bool fatal, string errorMessageType, params object[] errorMessageArgs)
		{
			(_errors ?? (_errors = new List<RantCompilerMessage>()))
				.Add(new RantCompilerMessage(RantCompilerMessageType.Error, _sourceName, GetString(errorMessageType, errorMessageArgs),
					token?.Line ?? 0, token?.Column ?? 0, token?.Offset ?? -1));
			if (fatal) throw new RantCompilerException(_sourceName, _errors);
		}
	}
}
