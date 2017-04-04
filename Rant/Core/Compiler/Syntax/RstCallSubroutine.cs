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

namespace Rant.Core.Compiler.Syntax
{
    [RST("csub")]
    internal class RstCallSubroutine : RstSubroutineBase
    {
        private bool _inModule = false;
        private string _moduleFunctionName = null;
        public List<RST> Arguments;

        public RstCallSubroutine(string name, LineCol location, string moduleFunctionName = null)
            : base(location)
        {
            if (moduleFunctionName != null)
                _inModule = true;
            _moduleFunctionName = moduleFunctionName;
            Name = name;
        }

        public RstCallSubroutine(LineCol location) : base(location)
        {
            // Used by serializer
        }

        public override IEnumerator<RST> Run(Sandbox sb)
        {
			if (sb.Objects[Name] == null)
                throw new RantRuntimeException(sb, this, "err-runtime-missing-subroutine", Name);
            var sub = (RstDefineSubroutine)(sb.Objects[Name].Value);
            if (sub.Parameters.Keys.Count != Arguments.Count)
                throw new RantRuntimeException(sb, this, "err-runtime-subarg-mismatch", Name);
            var action = sub.Body;
            var args = new Dictionary<string, RST>();
            var parameters = sub.Parameters.Keys.ToArray();
            for (int i = 0; i < Arguments.Count; i++)
            {
                if (sub.Parameters[parameters[i]] == SubroutineParameterType.Greedy)
                {
                    sb.AddOutputWriter();
                    yield return Arguments[i];
                    var output = sb.Return();
                    args[parameters[i]] = new RstText(Location, output.Main);
                }
                else
                {
                    args[parameters[i]] = Arguments[i];
                }
            }
            sb.SubroutineArgs.Push(args);
            yield return action;
            sb.SubroutineArgs.Pop();
        }

        protected override IEnumerator<RST> Serialize(EasyWriter output)
        {
            var iterMain = base.Serialize(output);
            while (iterMain.MoveNext()) yield return iterMain.Current;
            output.Write(_inModule);
            output.Write(_moduleFunctionName);
            output.Write(Arguments.Count);
            foreach (var arg in Arguments) yield return arg;
        }

        protected override IEnumerator<DeserializeRequest> Deserialize(EasyReader input)
        {
            var iterMain = base.Deserialize(input);
            while (iterMain.MoveNext()) yield return iterMain.Current;
            input.ReadBoolean(out _inModule);
            input.ReadString(out _moduleFunctionName);
            int argc = input.ReadInt32();
            if (Arguments == null) Arguments = new List<RST>(argc);
            for (int i = 0; i < argc; i++)
            {
                var request = new DeserializeRequest();
                yield return request;
                Arguments.Add(request.Result);
            }
        }

		public override string ToString()
		{
			return $"[${Name} ({Arguments.Count})]";
		}
	}
}