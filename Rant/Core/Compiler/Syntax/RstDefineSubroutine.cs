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

using Rant.Core.Constructs;
using Rant.Core.IO;
using Rant.Core.ObjectModel;

namespace Rant.Core.Compiler.Syntax
{
	[RST("dsub")]
	internal class RstDefineSubroutine : RstSubroutineBase
	{
		public List<SubroutineParameter> Parameters;

		public RstDefineSubroutine(LineCol location) : base(location)
		{
			// Used by serializer
		}

		public override IEnumerator<RST> Run(Sandbox sb)
		{
			if (sb.Objects[Name]?.Value is Subroutine sub) // Subroutine exists, simply add overload
			{
				sub.DefineOverload(Parameters, Body);
			}
			else // Create new subroutine object and add overload
			{
				var s = new Subroutine(Name);
				sb.Objects[Name] = new RantObject(s);
				s.DefineOverload(Parameters, Body);
			}
			yield break;
		}

		protected override IEnumerator<RST> Serialize(EasyWriter output)
		{
			var iterMain = base.Serialize(output);
			while (iterMain.MoveNext()) yield return iterMain.Current;
			output.Write(Parameters.Count);
			foreach (var subParam in Parameters)
			{
				output.Write(subParam.Name);
				output.Write((byte)subParam.Type);
			}
		}

		protected override IEnumerator<DeserializeRequest> Deserialize(EasyReader input)
		{
			var iterMain = base.Deserialize(input);
			while (iterMain.MoveNext()) yield return iterMain.Current;
			int pCount = input.ReadInt32();
			if (Parameters == null) Parameters = new List<SubroutineParameter>();
			for (int i = 0; i < pCount; i++)
			{
				string paramName = input.ReadString();
				var paramType = (SubroutineParameterType)input.ReadByte();
				Parameters.Add(new SubroutineParameter(paramName, paramType));
			}
		}
	}
}