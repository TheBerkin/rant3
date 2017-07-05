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

using Rant.Core.Compiler.Syntax;

namespace Rant.Core.Constructs
{
	internal sealed class Subroutine
	{
		public string Name { get; }
		private readonly Dictionary<int, Overload> _overloads;

		public Subroutine(string name)
		{
			Name = name;
			_overloads = new Dictionary<int, Overload>();
		}

		public void DefineOverload(IEnumerable<SubroutineParameter> parameters, RST body)
		{
			var pArray = parameters.ToArray();
			_overloads[pArray.Length] = new Overload(pArray, body);
		}

		public Overload GetOverload(int paramCount) => _overloads.TryGetValue(paramCount, out Overload ol) ? ol : null;

		internal class Overload
		{
			public RST Body { get; }

			public SubroutineParameter[] Params { get; }

			public int ParamCount => Params.Length;

			public Overload(SubroutineParameter[] parameters, RST body)
			{
				Params = parameters;
				Body = body;
			}

			public int GetParamIndex(string name)
			{
				for (int i = 0; i < Params.Length; i++)
				{
					if (Params[i].Name == name) return i;
				}
				return -1;
			}
		}
	}
}