using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rant.Vocabulary
{
    /// <summary>
    /// Diffs your marks.
    /// </summary>
    internal sealed class Diff
    {
        private readonly string _patternString;
        private readonly Rule[] _rules;

        /// <summary>
        /// The pattern string for the diff.
        /// </summary>
        public string Pattern => _patternString;

        /// <summary>
        /// Creates a new Diffmark pattern for you to enjoy.
        /// </summary>
        /// <param name="patternString">The pattern string.</param>
        public Diff(string patternString)
        {
            if (patternString == null)
                throw new ArgumentException("Pattern string cannot be null.");

            _patternString = patternString;
            _rules = Lexer.Lex(patternString).Select(tokens => Rule.Parse(tokens.ToArray())).ToArray();
        }

        /// <summary>
        /// Applies the pattern to a string.
        /// </summary>
        /// <param name="baseString">The string to apply the pattern to.</param>
        /// <returns></returns>
        public string Mark(string baseString)
        {
            foreach (var rule in _rules)
            {
                switch (rule.Type)
                {
                    case DiffRuleType.Add:
                        baseString = rule.Prepend
                            ? rule.ConcatString + baseString
                            : baseString + rule.ConcatString;
                        continue;
                    case DiffRuleType.Subtract:
                        baseString = Cut(baseString, rule.Factor, rule.Prepend);
                        baseString = rule.Prepend
                            ? rule.ConcatString + baseString
                            : baseString + rule.ConcatString;
                        continue;
                    case DiffRuleType.ReplaceWord:
                        baseString = ReplaceWord(baseString, rule.ConcatString, rule.Factor, rule.Prepend);
                        continue;
                }
            }
            return baseString;
        }

        /// <summary>
        /// Transforms one string to another.
        /// </summary>
        /// <param name="baseString">The base string to transform.</param>
        /// <param name="pattern">The Diffmark pattern to apply to the string.</param>
        /// <returns></returns>
        public static string Mark(string baseString, string pattern)
        {
            return new Diff(pattern).Mark(baseString);
        }

        internal static string Cut(string baseString, int factor, bool prepend)
        {
            if (factor > baseString.Length) return String.Empty;
            return prepend ? baseString.Substring(factor) : baseString.Substring(0, baseString.Length - factor);
        }

        internal static string ReplaceWord(string baseString, string replacement, int factor, bool prepend)
        {
            int a = 0;
            int b = 0;
            int currentWordIndex = 0;
            if (prepend)
            {
                while (currentWordIndex < factor)
                {
                    a = b;
                    while (a < baseString.Length && !Char.IsLetterOrDigit(baseString[a]))
                    {
                        a++;
                    }

                    if (a >= baseString.Length) return baseString;

                    b = a;
                    while (b < baseString.Length && Char.IsLetterOrDigit(baseString[b]))
                    {
                        b++;
                    }

                    currentWordIndex++;
                }

            }
            else
            {
                a = b = baseString.Length;
                while (currentWordIndex < factor)
                {
                    b = a;
                    while (b > 0 && !Char.IsLetterOrDigit(baseString[b - 1]))
                    {
                        b--;
                    }

                    if (b <= 0) return baseString;

                    a = b;
                    while (a > 0 && Char.IsLetterOrDigit(baseString[a - 1]))
                    {
                        a--;
                    }

                    currentWordIndex++;
                }
            }
            return String.Concat(baseString.Substring(0, a), replacement, baseString.Substring(b));
        }
    }

    internal class Rule
    {
        private static readonly Dictionary<DM, DiffRuleType> RuleMap = new Dictionary<DM, DiffRuleType>()
        {
            {DM.ReplaceWord, DiffRuleType.ReplaceWord},
            {DM.Subtract, DiffRuleType.Subtract},
            {DM.Add, DiffRuleType.Add}
        };

        public readonly string ConcatString;
        public readonly bool Prepend;
        public readonly DiffRuleType Type;
        public readonly int Factor;

        private Rule(string concatString, bool prepend, DiffRuleType type, int factor)
        {
            ConcatString = concatString;
            Prepend = prepend;
            Type = type;
            Factor = factor;
        }

        public static Rule Parse(Token[] tokens)
        {
            DiffRuleType ruleType;
            DM op;
            bool prepend = !RuleMap.TryGetValue(op = tokens[0].Type, out ruleType);
            if (prepend && !RuleMap.TryGetValue(op = tokens[tokens.Length - 1].Type, out ruleType))
            {
                prepend = false;
                ruleType = DiffRuleType.Add;
            }

            int factor = 0;

            if (ruleType != DiffRuleType.Add)
            {
                if (prepend)
                {
                    factor += tokens.Reverse().TakeWhile(t => t.Type == op).Count();
                }
                else
                {
                    factor += tokens.TakeWhile(t => t.Type == op).Count();
                }
            }
            else
            {
                factor++;
            }

            var sb = new StringBuilder();

            foreach (var token in
                tokens.SkipWhile(t => RuleMap.ContainsKey(t.Type))
                    .Reverse()
                    .SkipWhile(t => RuleMap.ContainsKey(t.Type))
                    .Reverse())
            {
                sb.Append(token.Value);
            }

            return new Rule(sb.ToString(), prepend, ruleType, factor);
        }
    }

    internal enum DiffRuleType
    {
        Add,
        Subtract,
        ReplaceWord
    }

    internal static class Lexer
    {
        private static IEnumerable<Token> GetTokens(string patternString)
        {
            patternString = patternString.Trim();
            var text = new StringBuilder();
            Token nextToken = null;
            for (int i = 0; i < patternString.Length; i++)
            {
                switch (patternString[i])
                {
                    case '\\':
                        nextToken = new Token(DM.Escape, Escape(patternString[++i]).ToString());
                        break;
                    case '+':
                        nextToken = new Token(DM.Add, "+");
                        break;
                    case '-':
                        nextToken = new Token(DM.Subtract, "-");
                        break;
                    case '*':
                        nextToken = new Token(DM.ReplaceWord, "*");
                        break;
                    case ';':
                        nextToken = new Token(DM.Delimiter, ";");
                        break;
                    case ' ':
                        if (text.Length > 0) goto default;
                        continue;
                    default:
                        text.Append(patternString[i]);
                        continue;
                }
                if (text.Length > 0)
                {
                    yield return new Token(DM.Text, text.ToString().Trim());
                    text.Length = 0;
                }
                yield return nextToken;
                nextToken = null;
            }
            if (text.Length > 0)
            {
                yield return new Token(DM.Text, text.ToString().Trim());
            }
        }

        public static IEnumerable<IEnumerable<Token>> Lex(string patternString)
        {
            var tokens = GetTokens(patternString);
            var list = new List<Token>();
            foreach (var token in tokens)
            {
                if (token.Type == DM.Delimiter && list.Any())
                {
                    yield return list.ToArray();
                    list.Clear();
                }
                else
                {
                    list.Add(token);
                }
            }
            if (list.Any()) yield return list.ToArray();
        }

        internal static char Escape(char escapeChar)
        {
            switch (escapeChar)
            {
                case 'n':
                    return '\n';
                case 's':
                    return ' ';
                case 'r':
                    return '\r';
                case 't':
                    return '\t';
                default:
                    return escapeChar;
            }
        }
    }

    internal class Token
    {
        public readonly DM Type;
        public readonly string Value;

        public Token(DM type, string value)
        {
            Type = type;
            Value = value;
        }
    }

    internal enum DM
    {
        Add,
        Subtract,
        ReplaceWord,
        Escape,
        Text,
        Delimiter
    }
}