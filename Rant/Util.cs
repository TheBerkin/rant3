using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

using Rant.Compiler;

using Stringes;

namespace Rant
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

        private static readonly Regex RegCapsProper = new Regex(@"\b[a-z]", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
        private static readonly Regex RegCapsFirst = new Regex(@"(?<![a-z].*?)[a-z]", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Capitalize(string input, ref Capitalization caps, ref char lastChar)
        {
            if (String.IsNullOrEmpty(input)) return input;
            switch (caps)
            {
                case Capitalization.Lower:
                    input = input.ToLower();
                    break;
                case Capitalization.Upper:
                    input = input.ToUpper();
                    break;
                case Capitalization.First:
                    input = RegCapsFirst.Replace(input, m => m.Value.ToUpper());
                    caps = Capitalization.None;
                    break;
                case Capitalization.Word:
                    char _lastChar = lastChar;
                    input = RegCapsProper.Replace(input, m => (m.Index > 0 || Char.IsSeparator(_lastChar) || Char.IsWhiteSpace(_lastChar)) ? m.Value.ToUpper() : m.Value);
                    break;
            }
            lastChar = input[input.Length - 1];
            return input;
        }

        public static char SelectFromRanges(string rangeString, RNG rng)
        {
            if (String.IsNullOrEmpty(rangeString)) return '?';
            var list = new List<Tuple<char, char, int>>(); // <min, max, weight>
            var chars = rangeString.GetEnumerator();
            char a, b;
            bool stall = false;
            while (stall || chars.MoveNext())
            {
                stall = false;
                if (Char.IsWhiteSpace(chars.Current)) continue;
                if (!Char.IsLetterOrDigit(a = chars.Current)) return '?';
                
                if (!chars.MoveNext())
                {
                    list.Add(Tuple.Create(a, a, 1));
                    break;
                }

                if (chars.Current == '-')
                {
                    if (!chars.MoveNext()) return '?';
                    if (!Char.IsLetterOrDigit(b = chars.Current)) return '?';
                    if (Char.IsLetter(a) != Char.IsLetter(b) || Char.IsUpper(a) != Char.IsUpper(b)) return '?';
                    list.Add(Tuple.Create(a < b ? a : b, a > b ? a : b, Math.Abs(b - a) + 1));
                    continue;
                }

                list.Add(Tuple.Create(a, a, 1));

                stall = true;
            }

            if (!list.Any()) return '?';

            int wSelect = rng.Next(0, list.Sum(r => r.Item3)) + 1;
            var ranges = list.GetEnumerator();
            while (ranges.MoveNext())
            {
                if (wSelect > ranges.Current.Item3)
                {
                    wSelect -= ranges.Current.Item3;
                }
                else
                {
                    break;
                }
            }
            return Convert.ToChar(rng.Next(ranges.Current.Item1, ranges.Current.Item2 + 1));
        }

        public static string NameToCamel(string name)
        {
            if (String.IsNullOrEmpty(name)) return name;
            var sb = new StringBuilder();
            for (int i = 0; i < name.Length; i++)
            {
                if (i == 0)
                {
                    sb.Append(Char.ToUpper(name[i]));
                }
                else if ((name[i] == '_' || name[i] == '-') && i + 1 < name.Length)
                {
                    sb.Append(Char.ToUpper(name[++i]));
                }
                else
                {
                    sb.Append(Char.ToLower(name[i]));
                }
            }
            return sb.ToString();
        }

        public static Regex ParseRegex(string regexLiteral)
        {
            if (String.IsNullOrEmpty(regexLiteral)) throw new ArgumentException("Argument 'regexLiteral' cannot be null or empty.");
            bool noCase = regexLiteral.EndsWith("i");
            var literal = regexLiteral.TrimEnd('i');
            if (!literal.StartsWith("/") || !literal.EndsWith("/")) throw new FormatException("Regex literal was not in the correct format.");
            return new Regex(literal.Slice(1, literal.Length - 1), (noCase ? RegexOptions.IgnoreCase : RegexOptions.None) | RegexOptions.ExplicitCapture);
        }

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
            else if (code[0] == 'N') // The only escape sequence that returns more than 1 character without a quantifier
            {
                for (int i = 0; i < count; i++)
                    sb.Append(Environment.NewLine);
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