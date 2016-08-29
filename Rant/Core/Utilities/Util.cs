using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Rant.Core.Utilities
{
	internal static class Util
	{
		private static readonly Dictionary<Type, HashSet<string>> _enumTable = new Dictionary<Type, HashSet<string>>();

		public static bool IsUppercase(string sample)
		{
			// All-caps?
			if (!sample.Where(char.IsLetter).All(char.IsUpper)) return false;

			int longest = 0;
			int curLength = 0;
			for (int i = 0; i < sample.Length; i++)
			{
				if (char.IsUpper(sample[i]))
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
			string name = SnakeToCamel(modeString.Trim());
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
			if (int.TryParse(value, out number)) return true;
			if (IsNullOrWhiteSpace(value)) return false;
			value = value.Trim();
			if (!char.IsLetter(value[value.Length - 1])) return false;
			char power = value[value.Length - 1];
			value = value.Substring(0, value.Length - 1);
			if (IsNullOrWhiteSpace(value)) return false;
			double n;
			if (!double.TryParse(value, out n)) return false;
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
			if (!char.IsLetter(value[value.Length - 1]))
				return double.TryParse(value, out number);
			char power = value[value.Length - 1];
			value = value.Substring(0, value.Length - 1);
			if (IsNullOrWhiteSpace(value)) return false;
			double n;
			if (!double.TryParse(value, out n)) return false;
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
			string v = input.ToLower().Trim();
			if (v == "false" || v == "0") return false;
			if (v == "true") return true;
			double d;
			return double.TryParse(v, out d);
		}

		public static string SnakeToCamel(string name)
		{
			if (string.IsNullOrEmpty(name)) return name;
			var sb = new StringBuilder();
			for (int i = 0; i < name.Length; i++)
			{
				if (i == 0)
				{
					sb.Append(char.ToUpper(name[i]));
				}
				else if ((name[i] == '_' || name[i] == '-') && i + 1 < name.Length)
				{
					sb.Append(char.ToUpper(name[++i]));
				}
				else
				{
					sb.Append(char.ToLower(name[i]));
				}
			}
			return sb.ToString();
		}

		public static string CamelToSnake(string camelName)
		{
			string name = camelName.Trim();
			if (IsNullOrWhiteSpace(name)) return name;
			if (name.Length == 1) return name.ToLower();
			var sb = new StringBuilder();
			bool a, b;
			bool last = false;
			for (int i = 0; i < name.Length - 1; i++)
			{
				a = char.IsUpper(name[i]);
				b = char.IsUpper(name[i + 1]);
				if ((last && a && !b)) sb.Append('-');
				sb.Append(char.ToLower(name[i]));
				if (!a && b) sb.Append('-');
				last = a;
			}
			sb.Append(char.ToLower(name[name.Length - 1]));
			return sb.ToString();
		}

		public static Regex ParseRegex(string regexLiteral)
		{
			if (string.IsNullOrEmpty(regexLiteral))
				throw new ArgumentException($"Argument '{nameof(regexLiteral)}' cannot be null nor empty.");
			bool noCase = regexLiteral.EndsWith("i");
			string literal = regexLiteral.TrimEnd('i');
			if (!literal.StartsWith("`") || !literal.EndsWith("`"))
				throw new FormatException("Regex literal was not in the correct format.");

			return new Regex(literal.Substring(1, literal.Length - 2),
				(noCase ? RegexOptions.IgnoreCase : RegexOptions.None) | RegexOptions.ExplicitCapture | RegexOptions.Compiled);
		}

		public static bool ValidateName(string input)
		{
			return input.All(c => char.IsLetterOrDigit(c) || c == '_');
		}

		public static string Alt(string input, string alternate)
		{
			return string.IsNullOrEmpty(input) ? alternate : input;
		}

		public static int Mod(int a, int b) => ((a % b) + b) % b;

#if !UNITY
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static char ReverseChar(char c)
		{
			switch (c)
			{
				case '(':
					return ')';
				case ')':
					return '(';
				case '[':
					return ']';
				case ']':
					return '[';
				case '{':
					return '}';
				case '}':
					return '{';
				case '<':
					return '>';
				case '«':
					return '»';
				case '»':
					return '«';
				case '‹':
					return '›';
				case '›':
					return '‹';
				case '\u201c':
					return '\u201d';
				case '\u201d':
					return '\u201c';
				case '\u2018':
					return '\u2019';
				case '\u2019':
					return '\u2018';
				default:
					return c;
			}
		}
	}
}