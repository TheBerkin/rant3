using System;
using System.Collections.Generic;

using Rant.Core.Stringes;

namespace Rant.Core.Compiler.Syntax
{
	/// <summary>
	/// Prints a string constant to the output.
	/// </summary>
	internal class RstText : RST
	{
		private readonly string _text;

		public string Text => _text;

		public RstText(Stringe token) : base(token)
		{
			_text = token.Value ?? String.Empty;
		}

		public RstText(Stringe token, string text) : base(token)
		{
			_text = text ?? String.Empty;
		}

		public override IEnumerator<RST> Run(Sandbox sb)
		{
			sb.Print(_text);
			yield break;
		}
	}
}