using System.Collections.Generic;

using Rant.Core.IO;
using Rant.Core.ObjectModel;

namespace Rant.Core.Compiler.Syntax
{
	[RST("dsub")]
	internal class RstDefineSubroutine : RstSubroutineBase
	{
		public Dictionary<string, SubroutineParameterType> Parameters;

		public RstDefineSubroutine(LineCol location) : base(location)
		{
			// Used by serializer
		}

		public override IEnumerator<RST> Run(Sandbox sb)
		{
			sb.Objects[Name] = new RantObject(this);
			yield break;
		}

		protected override IEnumerator<RST> Serialize(EasyWriter output)
		{
			var iterMain = base.Serialize(output);
			while (iterMain.MoveNext()) yield return iterMain.Current;
			output.Write(Parameters.Count);
			foreach (var kv in Parameters)
			{
				output.Write(kv.Key);
				output.Write((byte)kv.Value);
			}
		}

		protected override IEnumerator<DeserializeRequest> Deserialize(EasyReader input)
		{
			var iterMain = base.Deserialize(input);
			while (iterMain.MoveNext()) yield return iterMain.Current;
			int pCount = input.ReadInt32();
			if (Parameters == null) Parameters = new Dictionary<string, SubroutineParameterType>(pCount);
			for (int i = 0; i < pCount; i++)
			{
				string key = input.ReadString();
				Parameters[key] = (SubroutineParameterType)input.ReadByte();
			}
		}
	}

	internal enum SubroutineParameterType : byte
	{
		Loose,
		Greedy
	}
}