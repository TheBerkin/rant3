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
using System.Text.RegularExpressions;

using Rant.Core.IO;

namespace Rant.Core.Compiler.Syntax
{
    /// <summary>
    /// Replaces text in a pattern output according to a regular expression and evaluator pattern.
    /// </summary>
    [RST("repl")]
    internal class RstReplacer : RST
    {
        private Regex _regex;
        private RST _rstMatchEval;
        private RST _rstSource;

        public RstReplacer(LineCol location, Regex regex, RST rstSource, RST rstMatchEval) : base(location)
        {
            _regex = regex;
            _rstSource = rstSource;
            _rstMatchEval = rstMatchEval;
        }

        public RstReplacer(LineCol location) : base(location)
        {
            // Used by serializer
        }

        public override IEnumerator<RST> Run(Sandbox sb)
        {
            sb.AddOutputWriter();
            yield return _rstSource;
            string input = sb.Return().Main;
            var matches = _regex.Matches(input);
            int start = 0;
            foreach (Match match in matches)
            {
                sb.RegexMatches.Push(match);
                sb.AddOutputWriter();
                yield return _rstMatchEval;
                string result = sb.Return().Main;
                sb.Print(input.Substring(start, match.Index - start));
                sb.Print(result);
                sb.RegexMatches.Pop();
                start = match.Index + match.Length;
            }
            sb.Print(input.Substring(start, input.Length - start));
        }

        protected override IEnumerator<RST> Serialize(EasyWriter output)
        {
            output.Write((_regex.Options & RegexOptions.IgnoreCase) == RegexOptions.IgnoreCase); // Ignore case?
            output.Write(_regex.ToString());
            yield return _rstSource;
            yield return _rstMatchEval;
        }

        protected override IEnumerator<DeserializeRequest> Deserialize(EasyReader input)
        {
            var options = RegexOptions.Compiled | RegexOptions.ExplicitCapture;
            if (input.ReadBoolean()) options |= RegexOptions.IgnoreCase;
            _regex = new Regex(input.ReadString(), options);
            var request = new DeserializeRequest();
            yield return request;
            _rstSource = request.Result;
            request = new DeserializeRequest();
            yield return request;
            _rstMatchEval = request.Result;
        }
    }
}