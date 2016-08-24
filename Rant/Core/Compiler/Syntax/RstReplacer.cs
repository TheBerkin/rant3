using System.Collections.Generic;
using System.Text.RegularExpressions;

using Rant.Core.Stringes;

namespace Rant.Core.Compiler.Syntax
{
	/// <summary>
	/// Replaces text in a pattern output according to a regular expression and evaluator pattern.
	/// </summary>
	internal class RstReplacer : RST
	{
		private readonly RST _matchEvalAction;
		private readonly Regex _regex;
		private readonly RST _sourceAction;

		public RstReplacer(Stringe location, Regex regex, RST sourceAction, RST matchEvalAction) : base(location)
		{
			_regex = regex;
			_sourceAction = sourceAction;
			_matchEvalAction = matchEvalAction;
		}

		public override IEnumerator<RST> Run(Sandbox sb)
		{
			sb.AddOutputWriter();
			yield return _sourceAction;
			string input = sb.Return().Main;
			var matches = _regex.Matches(input);
			int start = 0;
			foreach (Match match in matches)
			{
				sb.RegexMatches.Push(match);
				sb.AddOutputWriter();
				yield return _matchEvalAction;
				string result = sb.Return().Main;
				sb.Print(input.Substring(start, match.Index - start));
				sb.Print(result);
				sb.RegexMatches.Pop();
				start = match.Index + match.Length;
			}
			sb.Print(input.Substring(start, input.Length - start));
		}
	}
}