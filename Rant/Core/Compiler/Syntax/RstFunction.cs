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

using Rant.Core.Framework;
using Rant.Core.IO;
using Rant.Core.Utilities;

namespace Rant.Core.Compiler.Syntax
{
	[RST("nfnc")]
	internal class RstFunction : RST
	{
		private int _argc;
		private List<RST> _args;
		private RantFunctionSignature _funcInfo;

		public RstFunction(LineCol location, RantFunctionSignature funcInfo, List<RST> args)
			: base(location)
		{
			_funcInfo = funcInfo;
			_args = args;
			_argc = args.Count;
		}

		public RstFunction(LineCol location) : base(location)
		{
			// Used by serializer
		}

		private RantFunctionParameter GetParameter(int index)
		{
			if (index >= _funcInfo.Parameters.Length - 1 && _funcInfo.HasParamArray)
				return _funcInfo.Parameters[_funcInfo.Parameters.Length - 1];
			return _funcInfo.Parameters[index];
		}

		public override IEnumerator<RST> Run(Sandbox sb)
		{
			// Convert arguments to their native types
			int paramc = _funcInfo.Parameters.Length;
			var args = new object[_argc];
			double d;
			RantFunctionParameter p;
			for (int i = 0; i < _argc; i++)
			{
				p = GetParameter(i);
				switch (p.RantType)
				{
					// Patterns are passed right to the method
					case RantFunctionParameterType.Pattern:
						args[i] = _args[i];
						break;

					// Strings are evaluated
					case RantFunctionParameterType.String:
						sb.AddOutputWriter();
						yield return _args[i];
						args[i] = sb.Return().Main;
						break;

					// Numbers are evaluated, verified, and converted
					case RantFunctionParameterType.Number:
					{
						sb.AddOutputWriter();
						yield return _args[i];
						string strNum = sb.Return().Main;
						if (!double.TryParse(strNum, out d))
						{
							d = 0;
							int n;
							if (Util.ParseInt(strNum, out n)) d = n;
						}
						args[i] = Convert.ChangeType(d, p.NativeType);
						break;
					}

					// Modes are parsed into enumeration members
					case RantFunctionParameterType.Mode:
					{
						sb.AddOutputWriter();
						yield return _args[i];
						string strMode = sb.Return().Main;
						object value;
						if (!Util.TryParseEnum(p.NativeType, strMode, out value))
							throw new RantRuntimeException(sb.Pattern, _args[i].Location,
								$"Unknown mode value '{strMode}'.");
						args[i] = value;
						break;
					}

					// Flags are parsed from strings to enum members and combined with OR.
					case RantFunctionParameterType.Flags:
					{
						var enumType = p.NativeType;
						sb.AddOutputWriter();
						yield return _args[i];
						long flags = 0;
						string strFlags = sb.Return().Main;
						object value;
						foreach (string flag in strFlags.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
						{
							if (!Util.TryParseEnum(enumType, flag, out value))
								throw new RantRuntimeException(sb.Pattern, _args[i].Location,
									$"Unknown flag value '{flag}'.");
							flags |= Convert.ToInt64(value);
						}
						args[i] = Enum.ToObject(enumType, flags);
						break;
					}
				}
			}

			// Invoke the function
			IEnumerator<RST> requester;
			if (_funcInfo.HasParamArray)
			{
				int required = paramc - 1;
				int parrayCount = _argc - required;
				var parray = Array.CreateInstance(_funcInfo.Parameters[required].NativeType, parrayCount);
				Array.Copy(args, required, parray, 0, parrayCount);
				requester = _funcInfo.Invoke(sb, args.Take(required).Concat(new[] { parray }).ToArray());
			}
			else
			{
				requester = _funcInfo.Invoke(sb, args);
			}

			while (requester.MoveNext())
				yield return requester.Current;
		}

		protected override IEnumerator<RST> Serialize(EasyWriter output)
		{
			output.Write(_argc);
			output.Write(_funcInfo.Name);
			foreach (var arg in _args) yield return arg;
		}

		protected override IEnumerator<DeserializeRequest> Deserialize(EasyReader input)
		{
			input.ReadInt32(out _argc);
			string funcName = input.ReadString();
			_funcInfo = RantFunctionRegistry.GetFunction(funcName, _argc);
			if (_args == null) _args = new List<RST>(_argc);
			for (int i = 0; i < _argc; i++)
			{
				var request = new DeserializeRequest();
				yield return request;
				_args.Add(request.Result);
			}
		}
	}
}