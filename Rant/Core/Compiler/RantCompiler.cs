using System;
using System.Collections.Generic;
using System.Linq;

using Rant.Core.Compiler.Parsing;
using Rant.Core.Compiler.Syntax;
using Rant.Core.Framework;
using Rant.Core.Stringes;
using Rant.Resources;

namespace Rant.Core.Compiler
{
	internal class RantCompiler
	{
		private readonly Stringe _source;
		private readonly string _sourceName;
		private readonly TokenReader _reader;

		private CompileContext _nextContext = CompileContext.DefaultSequence;
		private Action<RantAction> _nextActionCallback = null;

		internal bool HasModule = false;
		internal readonly RantModule Module;

		public RantCompiler(string sourceName, string source)
		{
			Module = new RantModule(sourceName);

			_sourceName = sourceName;

			_reader = new TokenReader(sourceName, RantLexer.GenerateTokens(sourceName, _source = source.ToStringe()));
		}

		public void SetNextContext(CompileContext context) => _nextContext = context;

		public void SetNextActionCallback(Action<RantAction> callback) => _nextActionCallback = callback;

		public RantAction Compile()
		{
			var parser = Parser.Get<SequenceParser>();
			var stack = new Stack<IEnumerator<Parser>>();
			var actionList = new List<RantAction>();

			_nextActionCallback = a => actionList.Add(a);

			stack.Push(parser.Parse(this, _nextContext, _reader, _nextActionCallback));

			top:
			while (stack.Any())
			{
				var p = stack.Peek();

				while (p.MoveNext())
				{
					if (p.Current == null) continue;

					stack.Push(p.Current.Parse(this, _nextContext, _reader, _nextActionCallback));
					goto top;
				}

				stack.Pop();
			}

			return new RASequence(actionList, _source);
		}

		public void SyntaxError(Stringe token, string message, bool fatal = true)
		{
			throw new RantCompilerException(_sourceName, token, message);
		}

		public void SyntaxError(Stringe token, Exception innerException)
		{
			throw new RantCompilerException(_sourceName, token, innerException);
		}
	}
}
