using System.Collections.Generic;

using Rant.Core.IO;

namespace Rant.Core.Compiler.Syntax
{
	/// <summary>
	/// Prints a string constant to the output.
	/// </summary>
	[RST("text")]
	internal class RstText : RST
	{
		public RstText(string value, LineCol location) : base(location)
		{
			Text = value ?? string.Empty;
		}

		public RstText(LineCol location, string text) : base(location)
		{
			Text = text ?? string.Empty;
		}

		public RstText(LineCol location) : base(location)
		{
			// Used by serializer
		}

		public string Text { get; set; }

		public override IEnumerator<RST> Run(Sandbox sb)
		{
			sb.Print(Text);
			yield break;
		}

		protected override IEnumerator<RST> Serialize(EasyWriter output)
		{
			output.Write(Text);
			yield break;
		}

		protected override IEnumerator<DeserializeRequest> Deserialize(EasyReader input)
		{
			Text = input.ReadString();
			yield break;
		}
	}
}