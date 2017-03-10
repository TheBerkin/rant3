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

using Rant.Core.IO;

namespace Rant.Core.Compiler.Syntax
{
    /// <summary>
    /// Prints a string constant to the output.
    /// </summary>
    [RST("text")]
    internal class RstText : RST
    {
        public RstText(string value, LineCol location) : base(location)
        {
            Text = value ?? string.Empty;
        }

        public RstText(LineCol location, string text) : base(location)
        {
            Text = text ?? string.Empty;
        }

        public RstText(LineCol location) : base(location)
        {
            // Used by serializer
        }

        public string Text { get; set; }

        public override IEnumerator<RST> Run(Sandbox sb)
        {
            sb.Print(Text);
            yield break;
        }

        protected override IEnumerator<RST> Serialize(EasyWriter output)
        {
            output.Write(Text);
            yield break;
        }

        protected override IEnumerator<DeserializeRequest> Deserialize(EasyReader input)
        {
            Text = input.ReadString();
            yield break;
        }
    }
}