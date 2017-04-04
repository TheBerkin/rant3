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
    /// <summary>
    /// Performs a sequence of actions.
    /// </summary>
    [RST("patt")]
    internal class RstSequence : RST
    {
        public RstSequence(List<RST> actions, LineCol loc)
            : base(actions.Any() ? actions[0].Location : loc)
        {
            if (actions == null) return;
            Actions.AddRange(actions);
        }

        public RstSequence(LineCol location) : base(location)
        {
            // Used by serializer
        }

        public List<RST> Actions { get; } = new List<RST>();

        public override IEnumerator<RST> Run(Sandbox sb)
        {
            return Actions.GetEnumerator();
        }

        protected override IEnumerator<RST> Serialize(EasyWriter output)
        {
            output.Write(Actions.Count);
            foreach (var action in Actions) yield return action;
        }

        protected override IEnumerator<DeserializeRequest> Deserialize(EasyReader input)
        {
            int count = input.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                var request = new DeserializeRequest();
                yield return request;
                Actions.Add(request.Result);
            }
        }

		public override string ToString()
		{
			return "Pattern";
		}
	}
}