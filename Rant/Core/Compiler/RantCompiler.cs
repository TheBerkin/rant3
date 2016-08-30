using System;
using System.Collections.Generic;

using Rant.Core.Compiler.Parsing;
using Rant.Core.Compiler.Syntax;
using Rant.Resources;

using static Rant.Localization.Txtres;

namespace Rant.Core.Compiler
{
	internal class RantCompiler
	{
		private readonly Stack<CompileContext> _contextStack = new Stack<CompileContext>();
		private readonly TokenReader _reader;
		private readonly string _sourceName;
		internal readonly RantModule Module;
		private List<RantCompilerMessage> _errors;
		private Action<RST> _nextActionCallback = null;
		// private List<RantCompilerMessage> _warnings;
		internal bool HasModule = false;

		public RantCompiler(string sourceName, string source)
		{
			Module = new RantModule(sourceName);

			_sourceName = sourceName;
			Source = source;
			_reader = new TokenReader(sourceName, RantLexer.Lex(this, source), this);
		}

		internal CompileContext NextContext => _contextStack.Peek();
		public string Source { get; }
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
				while (stack.Count > 0)
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

				return new RstSequence(actionList, _reader.BaseLocation);
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

		public void SyntaxError(Token token, bool fatal, string errorMessageType, params object[] errorMessageArgs)
		{
			(_errors ?? (_errors = new List<RantCompilerMessage>()))
				.Add(new RantCompilerMessage(RantCompilerMessageType.Error, _sourceName,
					GetString(errorMessageType, errorMessageArgs),
					token.Line, token.Column, token.Index, token.Value?.Length ?? 0));
			if (fatal) throw new RantCompilerException(_sourceName, _errors);
		}

		public void SyntaxError(int line, int lastLineStart, int index, int length, bool fatal, string errorMessageType, params object[] errorMessageArgs)
		{
			(_errors ?? (_errors = new List<RantCompilerMessage>()))
				.Add(new RantCompilerMessage(RantCompilerMessageType.Error, _sourceName,
					GetString(errorMessageType, errorMessageArgs),
					line, index - lastLineStart + 1, index, length));
			if (fatal) throw new RantCompilerException(_sourceName, _errors);
		}

		public void SyntaxError(Token start, Token end, bool fatal, string errorMessageType, params object[] errorMessageArgs)
		{
			(_errors ?? (_errors = new List<RantCompilerMessage>()))
				.Add(new RantCompilerMessage(RantCompilerMessageType.Error, _sourceName,
					GetString(errorMessageType, errorMessageArgs),
					start.Line, start.Column, start.Index, end.Index - start.Index + end.Length));
			if (fatal) throw new RantCompilerException(_sourceName, _errors);
		}
	}
}