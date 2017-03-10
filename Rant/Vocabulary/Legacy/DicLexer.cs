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
using System.Globalization;
using System.IO;

namespace Rant.Vocabulary
{
    internal static class DicLexer
    {
        public static IEnumerable<DicToken> Tokenize(string source, string data)
        {
            var reader = new StringReader(data);
            string currentLine;
            char firstChar;
            int lineNumber = 1;

            while (reader.Peek() >= 0)
            {
                lineNumber++;

                currentLine = reader.ReadLine().Trim();
                if (string.IsNullOrWhiteSpace(currentLine)) continue;
                firstChar = currentLine[0];

                switch (firstChar)
                {
                    case '#':
                        yield return new DicToken(DicTokenType.Directive, lineNumber, currentLine.Substring(1).Trim());
                        break;
                    case '@':
                        break;
                    case '>':
                    {
                        currentLine = currentLine.Substring(1);

                        bool diffmark = false;
                        if (currentLine[0] == '>')
                        {
                            diffmark = true;
                            currentLine = currentLine.Substring(1);
                        }

                        yield return new DicToken(diffmark ? DicTokenType.DiffEntry : DicTokenType.Entry, lineNumber, currentLine.Trim());
                        break;
                    }
                    case '|':
                        yield return new DicToken(DicTokenType.Property, lineNumber, currentLine.Substring(1).Trim());
                        break;
                    default:
                        throw new InvalidDataException($"{source}: (Line {lineNumber}, Col 1) Unexpected token: '{firstChar}'.");
                }
            }

            yield return new DicToken(DicTokenType.EOF, lineNumber, null);
        }
    }

    internal enum DicTokenType
    {
        Directive,
        Entry,
        DiffEntry,
        Property,
        Ignore,
        EOF
    }

    internal struct DicToken
    {
        public static readonly DicToken None = new DicToken(DicTokenType.Ignore, 1, null);
        public DicTokenType Type;
        public string Value;
        public int Line;

        public DicToken(DicTokenType type, int line, string value)
        {
            Type = type;
            Value = value;
            Line = line;
        }

        public DicToken(DicTokenType type, int line, char value)
        {
            Type = type;
            Value = value.ToString(CultureInfo.InvariantCulture);
            Line = line;
        }

        public static implicit operator string(DicToken token)
        {
            return token.Value;
        }

        public static implicit operator DicToken(string data)
        {
            return new DicToken(DicTokenType.Ignore, 1, data);
        }

        public int Length => Value?.Length ?? 0;
    }
}