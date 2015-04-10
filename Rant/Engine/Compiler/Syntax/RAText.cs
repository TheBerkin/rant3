using System;
using System.Collections.Generic;

namespace Rant.Engine.Compiler.Syntax
{
	/// <summary>
	/// Prints a string constant to the output.
	/// </summary>
	internal class RAText : RantAction
	{
		private readonly string _text;

		public RAText(string text)
		{
			_text = text ?? String.Empty;
		}

		public override IEnumerator<RantAction> Run(Sandbox sb)
		{
			sb.Print(_text);
			yield break;
		}
	}
}