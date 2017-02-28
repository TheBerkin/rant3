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

using System;
using System.Collections.Generic;
using System.Linq;

using Rant.Metadata;

namespace Rant.Core.Framework
{
	internal class RantFunction : IRantFunctionGroup
	{
		private readonly Dictionary<int, RantFunctionSignature> _overloads = new Dictionary<int, RantFunctionSignature>();
		private RantFunctionSignature _paramsArrayFunc = null;

		public RantFunction(string name)
		{
			Name = name;
		}

		public string Name { get; }
		public IEnumerable<IRantFunction> Overloads => _overloads.Select(fn => fn.Value as IRantFunction);

		public void Add(RantFunctionSignature func)
		{
			RantFunctionSignature existing;
			if (_overloads.TryGetValue(func.Parameters.Length, out existing))
				throw new ArgumentException(
					$"Cannot load function {func} becaue its signature is ambiguous with existing function {existing}.");
			if (_paramsArrayFunc != null)
			{
				if (func.HasParamArray)
					throw new ArgumentException(
						$"Cannot load function {func} because another function with a parameter array was already loaded.");
				if (func.Parameters.Length >= _paramsArrayFunc.Parameters.Length)
					throw new ArgumentException(
						$"Cannot load function {func} because its signature is ambiguous with {_paramsArrayFunc}.");
			}

			_overloads[func.Parameters.Length] = func;
			if (func.HasParamArray) _paramsArrayFunc = func;
		}

		public RantFunctionSignature GetFunction(int argc)
		{
			if (_paramsArrayFunc != null && argc >= _paramsArrayFunc.Parameters.Length - 1) return _paramsArrayFunc;
			RantFunctionSignature func;
			return _overloads.TryGetValue(argc, out func) ? func : null;
		}
	}
}