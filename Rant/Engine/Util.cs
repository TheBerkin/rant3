using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Rant.Engine.Compiler;
using Rant.Stringes;

namespace Rant.Engine
{
    internal static class Util
    {
        private static readonly Dictionary<char, Func<VM, char>> EscapeTable = new Dictionary<char, Func<VM, char>>
        {
            {'n', vm => '\n'},
            {'r', vm => '\r'},
            {'t', vm => '\t'},
            {'b', vm => '\b'},
            {'f', vm => '\f'},
            {'v', vm => '\v'},
            {'0', vm => '\0'},
            {'s', vm => ' '},
            {'d', vm => Convert.ToChar(vm.RNG.Next(48, 58))},
            {'D', vm => Convert.ToChar(vm.RNG.Next(49, 58))},
            {'c', vm => Char.ToLowerInvariant(vm.Format.Letters[vm.RNG.Next(vm.Format.Letters.Length)])},
            {'C', vm => Char.ToUpperInvariant(vm.Format.Letters[vm.RNG.Next(vm.Format.Letters.Length)])},
            {'x', vm => "0123456789abcdef"[vm.RNG.Next(16)]},
            {'X', vm => "0123456789ABCDEF"[vm.RNG.Next(16)]},
            {'w', vm =>
            {
                int i = vm.RNG.Next(vm.Format.Letters.Length + 10);
                if (i >= 10)
                {
                    return Char.ToLowerInvariant(vm.Format.Letters[i - 10]);
                }
                return Convert.ToChar(i + 48);
            } },
            {'W', vm =>
            {   
                int i = vm.RNG.Next(vm.Format.Letters.Length + 10);
                if (i >= 10)
                {
                    return Char.ToUpperInvariant(vm.Format.Letters[i - 10]);
                }
                return Convert.ToChar(i + 48);
            } }
        };

        public static bool TryParseMode<TEnum>(string modeString, out TEnum value)
        {
            value = default(TEnum);
            if (!typeof(TEnum).IsEnum) throw new ArgumentException("TEnum must be an enumerated type.");
            try
            {
                value = (TEnum)Enum.Parse(typeof(TEnum), NameToCamel(modeString), true);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsNullOrWhiteSpace(string value)
        {
            return value == null || value.Trim().Length == 0;
        }

        public static int HashOf(params object[] objects)
        {
            return unchecked(objects.Select(o => o.GetHashCode()).Aggregate(17, (hash, next) => hash * 31 + next));
        }

        public static bool ParseInt(string value, out int number)
        {
            if (Int32.TryParse(value, out number)) return true;
            if (IsNullOrWhiteSpace(value)) return false;
            value = value.Trim();
            if (!Char.IsLetter(value[value.Length - 1])) return false;
            char power = value[value.Length - 1];
            value = value.Substring(0, value.Length - 1);
            if (IsNullOrWhiteSpace(value)) return false;
            double n;
            if (!Double.TryParse(value, out n)) return false;
            switch (power)
            {
                case 'k': // Thousand
                    number = (int)(n*1000);
                    return true;
                case 'M': // Million
                    number = (int)(n*1000000);
                    return true;
                case 'B': // Billion
                    number = (int)(n*1000000000);
                    return true;
                default:
                    return false;
            }
        }

        public static bool BooleanRep(string input)
        {
            if (IsNullOrWhiteSpace(input)) return false;
            var v = input.ToLower().Trim();
            if (v == "false" || v == "0") return false;
            if (v == "true") return true;
            double d;
            return Double.TryParse(v, out d);
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
            if (!literal.StartsWith("//") || !literal.EndsWith("//")) throw new FormatException("Regex literal was not in the correct format.");
            return new Regex(literal.Slice(2, literal.Length - 2), (noCase ? RegexOptions.IgnoreCase : RegexOptions.None) | RegexOptions.ExplicitCapture);
        }

        public static string UnescapeConstantLiteral(string literal)
        {
            if (String.IsNullOrEmpty(literal)) return literal;
            if (literal.StartsWith("\"")) literal = literal.Substring(1);
            if (literal.EndsWith("\"")) literal = literal.Substring(0, literal.Length - 1);
            return Regex.Replace(literal, @"""""", @"""");
        }

        public static string Unescape(string sequence, VM vm, RNG rng = null)
        {
            var match = RantLexer.EscapeRegex.Match(sequence);
            int count;
            ParseInt(Alt(match.Groups["count"].Value, "1"), out count);
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
                Func<VM, char> func;
                if (code[0] == 'a')
                {
                    foreach (var ch in vm.CurrentState.Output.GetActive())
                    {
                        for (int i = 0; i < count; i++) ch.WriteArticle();
                    }
                }
                else if (EscapeTable.TryGetValue(code[0], out func))
                {
                    for (int i = 0; i < count; i++)
                        sb.Append(func(vm));
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

        internal static RantException Error(RantPattern source, Stringe token, string message = "A generic syntax error was encountered.")
        {
            return new RantException(source, token, message);
        }

        internal static RantException Error(IEnumerable<Stringes.Tokens.Token<R>> tokens, RantPattern source, string message = "A generic syntax error was encountered.")
        {
            return new RantException(tokens, source, message);
        }

        internal static RantException Error(string source, Stringe token, string message = "A generic syntax error was encountered.")
        {
            return new RantException(source, token, message);
        }
    }
}