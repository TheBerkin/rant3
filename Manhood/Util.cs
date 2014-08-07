using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manhood
{
    internal static class Util
    {
        public static bool ValidateName(string name)
        {
            return !String.IsNullOrEmpty(name) && name.All(c => Char.IsLetterOrDigit(c) || "_-".Contains(c));
        }

        public static IEnumerable<string> SplitArgs(string args)
        {
            var pair = args.Split(new[] { ' ', '\r', '\n', '\t' }, 2, StringSplitOptions.RemoveEmptyEntries);
            if (pair.Length == 0) yield break;
            yield return pair[0].Trim();
            if (pair.Length == 1) yield break;

            int balanceSquare = 0;
            int balanceTri = 0;

            var sb = new StringBuilder();
            bool escapeNext = false;

            foreach (char c in pair[1])
            {
                if (!escapeNext)
                {
                    switch (c)
                    {
                        case '\\':
                            sb.Append('\\');
                            escapeNext = true;
                            continue;
                        case '[':
                            balanceSquare--;
                            break;
                        case '<':
                            balanceTri--;
                            break;
                        case ']':
                            if (++balanceSquare > 0)
                            {
                                throw new FormatException("Too many closing square brackets.");
                            }
                            break;
                        case '>':
                            if (++balanceTri > 0)
                            {
                                throw new FormatException("Too many closing triangular brackets.");
                            }
                            break;
                        default:
                            if (c == '/' && balanceSquare == 0 && balanceTri == 0)
                            {
                                yield return sb.ToString().Trim();
                                sb.Clear();
                                continue;
                            }
                            break;
                    }
                }
                escapeNext = false;
                sb.Append(c);
            }

            yield return sb.ToString().Trim();

            if (balanceSquare < 0)
            {
                throw new FormatException("Too many opening square brackets.");
            }

            if (balanceTri < 0)
            {
                throw new FormatException("Too many opening triangular brackets.");
            }
        }
    }
}