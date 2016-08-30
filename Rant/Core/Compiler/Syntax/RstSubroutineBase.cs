using System.Collections.Generic;

using Rant.Core.IO;

namespace Rant.Core.Compiler.Syntax
{
	internal abstract class RstSubroutineBase : RST
	{
		public RST Body;
		public string Name;

		public RstSubroutineBase(string name, LineCol location)
			: base(location)
		{
			Name = name;
		}

		public RstSubroutineBase(LineCol location) : base(location)
		{
			// Used by serializer
		}

		protected override IEnumerator<RST> Serialize(EasyWriter output)
		{
			output.Write(Name);
			yield return Body;
		}

		protected override IEnumerator<DeserializeRequest> Deserialize(EasyReader input)
		{
			input.ReadString(out Name);
			var bodyRequest = new DeserializeRequest();
			yield return bodyRequest;
			Body = bodyRequest.Result;
		}
	}
}