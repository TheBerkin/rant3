using Rant.Stringes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rant.Engine.Syntax
{
	internal class RAEscape : RantAction
	{
		private static readonly Dictionary<char, Action<Sandbox, int>> EscapeTable = new Dictionary<char, Action<Sandbox, int>>
		{
			{'n', (sb, c) => sb.Print(new string('\n', c))},
			{'N', (sb, c) =>
			{
				var b = new StringBuilder();
				for (int i = 0; i < c; i++) b.Append(Environment.NewLine);
				sb.Print(b);
			}},
			{'r', (sb, c) => sb.Print(new string('\r', c))},
			{'t', (sb, c) => sb.Print(new string('\t', c))},
			{'b', (sb, c) => sb.Print(new string('\b', c))},
			{'f', (sb, c) => sb.Print(new string('\f', c))},
			{'v', (sb, c) => sb.Print(new string('\v', c))},
			{'s', (sb, c) => sb.Print(new string(' ', c))},
			{'d', (sb, c) => sb.PrintMany(() => Convert.ToChar(sb.RNG.Next(48, 58)), c)},
			{'D', (sb, c) => sb.PrintMany(() => Convert.ToChar(sb.RNG.Next(49, 58)), c)},
			{'c', (sb, c) => sb.PrintMany(() => Char.ToLowerInvariant(sb.Format.Letters[sb.RNG.Next(sb.Format.Letters.Length)]), c)},
			{'C', (sb, c) => sb.PrintMany(() => Char.ToUpperInvariant(sb.Format.Letters[sb.RNG.Next(sb.Format.Letters.Length)]), c)},
			{'x', (sb, c) => sb.PrintMany(() => "0123456789abcdef"[sb.RNG.Next(16)], c)},
			{'X', (sb, c) => sb.PrintMany(() => "0123456789ABCDEF"[sb.RNG.Next(16)], c)},
			{
				'w', (sb, c) =>
				{
					int i;
					sb.PrintMany(() =>
					{
						i = sb.RNG.Next(sb.Format.Letters.Length + 10);
						return i >= 10 
						? Char.ToLowerInvariant(sb.Format.Letters[i - 10]) 
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
						i = sb.RNG.Next(sb.Format.Letters.Length + 10);
						return i >= 10
						? Char.ToUpperInvariant(sb.Format.Letters[i - 10])
						: Convert.ToChar(i + 48);
					}, c);
				}
			},
			{
				'a', (sb, c) => sb.Output.Do(chain => chain.AddArticleBuffer())
			}
		};

		private readonly char _code;
		private readonly int _times;
		private readonly bool _unicode;

		public RAEscape(Stringe escapeSequence) : base(escapeSequence)
		{
			var escape = escapeSequence.Value;

			// The lexer already assures that the string isn't empty.
			int codeIndex = 1; // skip past the backslash

			// parse the quantifier
			if (Char.IsDigit(escape[codeIndex]) && escape[codeIndex] != '0')
			{
				int commaIndex = escape.IndexOf(',', codeIndex + 1);
				if (commaIndex != -1)
				{
					Util.ParseInt(escape.Substring(1, commaIndex - 1), out _times);
					codeIndex = commaIndex + 1;
				}
			}
			else
			{
				_times = 1;
			}

			// parse the code
			switch (escape[codeIndex])
			{
				// unicode character is the only special case
				case 'u':
					_code = (char)Convert.ToUInt16(escape.Substring(codeIndex + 1), 16);
					_unicode = true;
					break;
				// everything else
				default:
					_code = escape[codeIndex];
					break;
			}
		}

		public override IEnumerator<RantAction> Run(Sandbox sb)
		{
			if (_unicode)
			{
				sb.Print(new string(_code, _times));
			}
			else
			{
				Action<Sandbox, int> func;
				if (!EscapeTable.TryGetValue(_code, out func))
				{
					sb.Print(new string(_code, _times));
				}
				else
				{
					func(sb, _times);
				}
			}
			yield break;
		}
	}
}