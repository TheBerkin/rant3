using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

using Rant.Internals.Stringes;

namespace Rant.Vocabulary
{
    internal static class DicLexer
    {
        public static IEnumerable<Token<DicTokenType>> Tokenize(string source, string data)
        {
            var reader = new StringeReader(data);
            Chare currentChar;
            while (!reader.EndOfStringe)
            {
                if (char.IsWhiteSpace((currentChar = reader.ReadChare()).Character)) continue;
                switch (currentChar.Character)
                {
                    case '#':
                        yield return new Token<DicTokenType>(DicTokenType.Directive, ReadRestOfLine(reader).Trim());
                        break;
                    case '@':
                        reader.ReadUntilAny('\n', '\r');
                        break;
                    case '>':
                    {
                        bool diffmark = reader.Eat('>');
                        yield return new Token<DicTokenType>(diffmark ? DicTokenType.DiffEntry : DicTokenType.Entry,
                            ReadRestOfLine(reader, diffmark).Trim());
                        break;
                    }
                    case '|':
                        yield return new Token<DicTokenType>(DicTokenType.Property, ReadRestOfLine(reader).Trim());
                    break;
                    default:
                        throw new InvalidDataException($"{source}: (Line {currentChar.Line}, Col {currentChar.Column}) Unexpected token: '{currentChar}'.");
                }
            }
            yield return new Token<DicTokenType>(DicTokenType.EOF, "");
		}

        private static Stringe ReadRestOfLine(StringeReader reader, bool isDiffmark = false) => isDiffmark
            ? reader.ReadUntilAny('>', '\n', '\r', '#', '@')
            : reader.ReadUntilAny('>', '|', '\n', '\r', '#', '@');
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
}