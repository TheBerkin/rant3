using System.Collections.Generic;

using Rant.Core.Stringes;

namespace Rant.Core.Compiler.Syntax
{
	/// <summary>
	/// Prints a string constant to the output.
	/// </summary>
	internal class RstText : RST
	{
		public RstText(Stringe token) : base(token)
		{
			Text = token.Value ?? string.Empty;
		}

		public RstText(Stringe token, string text) : base(token)
		{
			Text = text ?? string.Empty;
		}

		public string Text { get; }

		public override IEnumerator<RST> Run(Sandbox sb)
		{
			sb.Print(Text);
			yield break;
		}
	}
}