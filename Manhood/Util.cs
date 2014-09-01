using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Manhood.Compiler;

namespace Manhood
{
    internal class Util
    {
        private static readonly Dictionary<char, Func<RNG, char>> _escapeChars = new Dictionary<char, Func<RNG, char>>
        {
            {'n', rng => '\n'},
            {'r', rng => '\r'},
            {'t', rng => '\t'},
            {'b', rng => '\b'},
            {'f', rng => '\f'},
            {'v', rng => '\v'},
            {'0', rng => '\0'},
            {'s', rng => ' '},
            {'d', rng => rng == null ? '0' : Convert.ToChar(rng.Next(48, 58))},
            {'c', rng => rng == null ? '?' : Convert.ToChar(rng.Next(97, 123))},
            {'C', rng => rng == null ? '?' : Convert.ToChar(rng.Next(65, 91))},
            {'x', rng => rng == null ? '?' : "0123456789abcdef"[rng.Next(16)]},
            {'X', rng => rng == null ? '?' : "0123456789ABCDEF"[rng.Next(16)]}
        };

        public static string UnescapeConstantLiteral(string literal)
        {
            if (String.IsNullOrEmpty(literal)) return literal;
            if (literal.StartsWith("\"")) literal = literal.Substring(1);
            if (literal.EndsWith("\"")) literal = literal.Substring(0, literal.Length - 1);
            return Regex.Replace(literal, @"""""", @"""");
        }

        public static string Unescape(string sequence, RNG rng = null)
        {
            var match = Lexer.EscapeRegex.Match(sequence);
            var count = Int32.Parse(Alt(match.Groups["count"].Value, "1"));
            bool unicode = match.Groups["unicode"].Success;
            var code = Alt(match.Groups["code"].Value, match.Groups["unicode"].Value);
            var sb = new StringBuilder();
            if (unicode)
            {
                    sb.Append((char)Convert.ToUInt16(code, 16), count);
            }
            else
            {
                Func<RNG, char> func;
                if (_escapeChars.TryGetValue(code[0], out func))
                {
                    for (int i = 0; i < count; i++)
                        sb.Append(func(rng));
                }
                else
                {
                    for (int i = 0; i < count; i++)
                        sb.Append(code[0]);
                }
            }
            return sb.ToString();
        }

        public static bool ValidateName(string input)
        {
            return input.All(c => Char.IsLetterOrDigit(c) || c == '_');
        }

        public static string Alt(string input, string alternate)
        {
            return String.IsNullOrEmpty(input) ? alternate : input;
        }
    }
}