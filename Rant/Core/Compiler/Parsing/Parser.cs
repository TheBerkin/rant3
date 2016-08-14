using System;
using System.Collections.Generic;

using Rant.Core.Compiler.Syntax;

namespace Rant.Core.Compiler.Parsing
{
	internal abstract class Parser
	{
		private static readonly Dictionary<Type, Parser> _parserMap = new Dictionary<Type, Parser>();

		public static T Get<T>() where T : Parser
		{
			Parser parser;
			if(!_parserMap.TryGetValue(typeof(T), out parser))
			{
				parser = _parserMap[typeof(T)] = Activator.CreateInstance(typeof(T)) as T;
			}
			return parser as T;
		}

		public abstract IEnumerator<Parser> Parse(RantCompiler compiler, CompileContext context, TokenReader reader, Action<RantAction> actionCallback);
	}
}