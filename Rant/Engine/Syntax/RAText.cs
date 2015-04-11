using Rant.Stringes;
using System;
using System.Collections.Generic;

namespace Rant.Engine.Syntax
{
	/// <summary>
	/// Prints a string constant to the output.
	/// </summary>
	internal class RAText : RantAction
	{
		private readonly string _text;

		public RAText(Stringe token) : base(token)
		{
			_text = token.Value ?? String.Empty;
		}

		public RAText(Stringe token, string text) : base(token)
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