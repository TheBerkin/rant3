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
using System.Text;

using Rant.Core.IO;

namespace Rant.Core.Compiler.Syntax
{
    [RST("eseq")]
    internal class RstEscape : RST
    {
        private static readonly Dictionary<char, Action<Sandbox, int>> EscapeTable = new Dictionary
            <char, Action<Sandbox, int>>
            {
                { 'n', (sb, c) => sb.Print(new string('\n', c)) },
                {
                    'N', (sb, c) =>
                    {
                        var b = new StringBuilder();
                        for (int i = 0; i < c; i++) b.Append(Environment.NewLine);
                        sb.Print(b);
                    }
                },
                { 'r', (sb, c) => sb.Print(new string('\r', c)) },
                { 't', (sb, c) => sb.Print(new string('\t', c)) },
                { 'b', (sb, c) => sb.Print(new string('\b', c)) },
                { 'f', (sb, c) => sb.Print(new string('\f', c)) },
                { 'v', (sb, c) => sb.Print(new string('\v', c)) },
                { 's', (sb, c) => sb.Print(new string(' ', c)) },
                { 'd', (sb, c) => sb.PrintMany(() => Convert.ToChar(sb.RNG.Next(48, 58)), c) },
                { 'D', (sb, c) => sb.PrintMany(() => Convert.ToChar(sb.RNG.Next(49, 58)), c) },
                {
                    'c',
                    (sb, c) =>
                        sb.PrintMany(
                            () => char.ToLowerInvariant(sb.Format.WritingSystem.AlphabetArray[sb.RNG.Next(sb.Format.WritingSystem.AlphabetArray.Length)]), c)
                },
                {
                    'C',
                    (sb, c) =>
                        sb.PrintMany(
                            () => char.ToUpperInvariant(sb.Format.WritingSystem.AlphabetArray[sb.RNG.Next(sb.Format.WritingSystem.AlphabetArray.Length)]), c)
                },
                { 'x', (sb, c) => sb.PrintMany(() => "0123456789abcdef"[sb.RNG.Next(16)], c) },
                { 'X', (sb, c) => sb.PrintMany(() => "0123456789ABCDEF"[sb.RNG.Next(16)], c) },
                {
                    'w', (sb, c) =>
                    {
                        int i;
                        sb.PrintMany(() =>
                        {
                            i = sb.RNG.Next(sb.Format.WritingSystem.AlphabetArray.Length + 10);
                            return i >= 10
                                ? char.ToLowerInvariant(sb.Format.WritingSystem.AlphabetArray[i - 10])
                                : Convert.ToChar(i + 48);
                        }, c);
                    }
                },
                {
                    'W', (sb, c) =>
                    {
                        int i;
                        sb.PrintMany(() =>
                        {
                            i = sb.RNG.Next(sb.Format.WritingSystem.AlphabetArray.Length + 10);
                            return i >= 10
                                ? char.ToUpperInvariant(sb.Format.WritingSystem.AlphabetArray[i - 10])
                                : Convert.ToChar(i + 48);
                        }, c);
                    }
                },
                {
                    'a', (sb, c) => sb.Output.Do(chain => chain.AddArticleBuffer())
                }
            };

        private char _codeLow, _codeHigh;
        private int _times;
        private bool _unicode;

        public RstEscape(LineCol location, int quantity, bool unicode, char codeHighSurrogate, char codeLowSurrogate = '\0')
            : base(location)
        {
            _codeLow = codeLowSurrogate;
            _codeHigh = codeHighSurrogate;
            _times = quantity;
            _unicode = unicode;
        }

        public RstEscape(LineCol location) : base(location)
        {
            // Used by serializer
        }

        public override IEnumerator<RST> Run(Sandbox sb)
        {
            if (_unicode)
            {
                if (_codeLow != '\0')
                {
                    for (int i = 0; i < _times; i++)
                    {
                        sb.Print(_codeHigh);
                        sb.Print(_codeLow);
                    }
                }
                else
                    sb.Print(new string(_codeHigh, _times));
            }
            else
            {
                Action<Sandbox, int> func;
                if (!EscapeTable.TryGetValue(_codeHigh, out func))
                    sb.Print(new string(_codeHigh, _times));
                else
                    func(sb, _times);
            }
            yield break;
        }

        protected override IEnumerator<RST> Serialize(EasyWriter output)
        {
            output.Write(_codeHigh);
            output.Write(_codeLow);
            output.Write(_times);
            output.Write(_unicode);
            yield break;
        }

        protected override IEnumerator<DeserializeRequest> Deserialize(EasyReader input)
        {
            input.ReadChar(out _codeHigh);
            input.ReadChar(out _codeLow);
            input.ReadInt32(out _times);
            input.ReadBoolean(out _unicode);
            yield break;
        }
    }
}