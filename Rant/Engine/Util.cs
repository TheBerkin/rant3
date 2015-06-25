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
		private static readonly Dictionary<Type, HashSet<string>> _enumTable = new Dictionary<Type, HashSet<string>>();

	    public static bool IsUppercase(string sample)
	    {
            // All-caps?
	        if (!sample.Where(Char.IsLetter).All(Char.IsUpper)) return false;

            int longest = 0;
            int curLength = 0;
            for (int i = 0; i < sample.Length; i++)
            {
                if (Char.IsUpper(sample[i]))
                {
                    if (++curLength > longest) longest++;
                }
                else
                {
                    curLength = 0;
                }
            }

	        return longest > 1;
	    }

		private static void CacheEnum(Type type)
		{
			if (!type.IsEnum || _enumTable.ContainsKey(type)) return;
			_enumTable[type] = new HashSet<string>(Enum.GetNames(type));
		}

		public static bool TryParseEnum(Type enumType, string modeString, out object value)
		{
			value = null;
			if (!enumType.IsEnum) throw new ArgumentException("TEnum must be an enumerated type.");
			CacheEnum(enumType);
			var name = SnakeToCamel(modeString.Trim());
            var cache = _enumTable[enumType];
			if (!cache.Contains(name)) return false;
			value = Enum.Parse(enumType, name, true);
			return true;
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
					number = (int)(n * 1000);
					return true;
				case 'M': // Million
					number = (int)(n * 1000000);
					return true;
				case 'B': // Billion
					number = (int)(n * 1000000000);
					return true;
				default:
					return false;
			}
		}

		public static bool ParseDouble(string value, out double number)
		{
			number = 0;
			if (IsNullOrWhiteSpace(value)) return false;
			value = value.Trim();
			if (!Char.IsLetter(value[value.Length - 1]))
				return Double.TryParse(value, out number);
			char power = value[value.Length - 1];
			value = value.Substring(0, value.Length - 1);
			if (IsNullOrWhiteSpace(value)) return false;
			double n;
			if (!Double.TryParse(value, out n)) return false;
			switch (power)
			{
				case 'k': // Thousand
					number = (int)(n * 1000);
					return true;
				case 'M': // Million
					number = (int)(n * 1000000);
					return true;
				case 'B': // Billion
					number = (int)(n * 1000000000);
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
			var list = new List<_<char, char, int>>(); // <min, max, weight>
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
					list.Add(_.Create(a, a, 1));
					break;
				}

				if (chars.Current == '-')
				{
					if (!chars.MoveNext()) return '?';
					if (!Char.IsLetterOrDigit(b = chars.Current)) return '?';
					if (Char.IsLetter(a) != Char.IsLetter(b) || Char.IsUpper(a) != Char.IsUpper(b)) return '?';
					list.Add(_.Create(a < b ? a : b, a > b ? a : b, Math.Abs(b - a) + 1));
					continue;
				}

				list.Add(_.Create(a, a, 1));

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

		public static string SnakeToCamel(string name)
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

	    public static string CamelToSnake(string camelName)
	    {
            var name = camelName.Trim();
            if (Util.IsNullOrWhiteSpace(name)) return name;
	        if (name.Length == 1) return name.ToLower();
            var sb = new StringBuilder();
	        bool a, b;
	        bool last = false;
	        for (int i = 0; i < name.Length - 1; i++)
	        {
	            a = Char.IsUpper(name[i]);
	            b = Char.IsUpper(name[i + 1]);
	            if ((last && a && !b)) sb.Append('-');
	            sb.Append(Char.ToLower(name[i]));
	            if (!a && b) sb.Append('-');
	            last = a;
	        }
            sb.Append(Char.ToLower(name[name.Length - 1]));
            return sb.ToString();
	    }

		public static Regex ParseRegex(string regexLiteral)
		{
			if (String.IsNullOrEmpty(regexLiteral))
				throw new ArgumentException($"Argument '{nameof(regexLiteral)}' cannot be null nor empty.");
			bool noCase = regexLiteral.EndsWith("i");
			var literal = regexLiteral.TrimEnd('i');
			if (!literal.StartsWith("`") || !literal.EndsWith("`"))
				throw new FormatException("Regex literal was not in the correct format.");
			
			return new Regex(literal.Substring(1, literal.Length - 2), 
				(noCase ? RegexOptions.IgnoreCase : RegexOptions.None) | RegexOptions.ExplicitCapture | RegexOptions.Compiled);
		}

		public static string UnescapeConstantLiteral(string literal)
		{
			if (String.IsNullOrEmpty(literal)) return literal;
			if (literal.StartsWith("\"")) literal = literal.Substring(1);
			if (literal.EndsWith("\"")) literal = literal.Substring(0, literal.Length - 1);
			return Regex.Replace(literal, @"""""", @"""");
		}

		public static bool ValidateName(string input)
		{
			return input.All(c => Char.IsLetterOrDigit(c) || c == '_');
		}

		public static string Alt(string input, string alternate)
		{
			return String.IsNullOrEmpty(input) ? alternate : input;
		}

		public static int Mod(int a, int b) => ((a % b) + b) % b;

		internal static RantRuntimeException Error(RantPattern source, Stringe token, string message = "A generic syntax error was encountered.")
		{
			return new RantRuntimeException(source, token, message);
		}

		internal static RantRuntimeException Error(IEnumerable<Token<R>> tokens, RantPattern source, string message = "A generic syntax error was encountered.")
		{
			return new RantRuntimeException(tokens, source, message);
		}

		internal static RantRuntimeException Error(string source, Stringe token, string message = "A generic syntax error was encountered.")
		{
			return new RantRuntimeException(source, token, message);
		}
	}
}