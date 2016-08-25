using System.Collections.Generic;

using Rant.Core.IO;
using Rant.Core.Stringes;

namespace Rant.Core.Compiler.Syntax
{
	internal abstract class RstSubroutineBase : RST
	{
		public string Name;
		public RST Body;

		public RstSubroutineBase(Stringe name)
			: base(name)
		{
			Name = name.Value;
		}

		protected override IEnumerator<RST> Serialize(EasyWriter output)
		{
			output.Write(Name);
			yield return Body;
		}

		protected override IEnumerator<DeserializeRequest> Deserialize(EasyReader input)
		{
			input.ReadString(out Name);
			var bodyRequest = new DeserializeRequest(input.ReadUInt32());
			yield return bodyRequest;
			Body = bodyRequest.Result;
		}
	}
}