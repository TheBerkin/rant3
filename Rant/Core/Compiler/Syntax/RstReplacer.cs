using System.Collections.Generic;
using System.Text.RegularExpressions;

using Rant.Core.IO;
using Rant.Core.Stringes;

namespace Rant.Core.Compiler.Syntax
{
	/// <summary>
	/// Replaces text in a pattern output according to a regular expression and evaluator pattern.
	/// </summary>
	[RST("repl")]
	internal class RstReplacer : RST
	{
		private Regex _regex;
		private RST _rstMatchEval;
		private RST _rstSource;

		public RstReplacer(Stringe location, Regex regex, RST rstSource, RST rstMatchEval) : base(location)
		{
			_regex = regex;
			_rstSource = rstSource;
			_rstMatchEval = rstMatchEval;
		}

		public RstReplacer(TokenLocation location) : base(location)
		{
			// Used by serializer
		}

		public override IEnumerator<RST> Run(Sandbox sb)
		{
			sb.AddOutputWriter();
			yield return _rstSource;
			string input = sb.Return().Main;
			var matches = _regex.Matches(input);
			int start = 0;
			foreach (Match match in matches)
			{
				sb.RegexMatches.Push(match);
				sb.AddOutputWriter();
				yield return _rstMatchEval;
				string result = sb.Return().Main;
				sb.Print(input.Substring(start, match.Index - start));
				sb.Print(result);
				sb.RegexMatches.Pop();
				start = match.Index + match.Length;
			}
			sb.Print(input.Substring(start, input.Length - start));
		}

		protected override IEnumerator<RST> Serialize(EasyWriter output)
		{
			output.Write((_regex.Options & RegexOptions.IgnoreCase) == RegexOptions.IgnoreCase); // Ignore case?
			output.Write(_regex.ToString());
			yield return _rstSource;
			yield return _rstMatchEval;
		}

		protected override IEnumerator<DeserializeRequest> Deserialize(EasyReader input)
		{
			var options = RegexOptions.Compiled | RegexOptions.ExplicitCapture;
			if (input.ReadBoolean()) options |= RegexOptions.IgnoreCase;
			_regex = new Regex(input.ReadString(), options);
			var request = new DeserializeRequest();
			yield return request;
			_rstSource = request.Result;
			request = new DeserializeRequest();
			yield return request;
			_rstMatchEval = request.Result;
		}
	}
}