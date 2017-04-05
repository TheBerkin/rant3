#region License

// https://github.com/TheBerkin/Rant
// 
// Copyright (c) 2017 Nicholas Fleck
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in the
// Software without restriction, including without limitation the rights to use, copy,
// modify, merge, publish, distribute, sublicense, and/or sell copies of the Software,
// and to permit persons to whom the Software is furnished to do so, subject to the
// following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
// PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE
// OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

using System.Collections.Generic;
using System.Linq;

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
			if (sb.Objects.ContainsKey(Name))
			{
				var subroutines = sb.Objects[Name].Value as List<RantObject>;
				if (subroutines.Any(s => (s.Value as RstDefineSubroutine).Parameters.Count == Parameters.Count))
				{
					subroutines.RemoveAll(s => (s.Value as RstDefineSubroutine).Parameters.Count == Parameters.Count);
				}
				subroutines.Add(new RantObject(this));
			}
			else
			{
				var list = new List<RantObject>();
				list.Add(new RantObject(this));
				sb.Objects[Name] = new RantObject(list);
			}
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