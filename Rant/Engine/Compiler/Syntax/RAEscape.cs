using Rant.Stringes;
using System;
using System.Collections.Generic;

namespace Rant.Engine.Compiler.Syntax
{
	internal class RAEscape : RantAction
	{
		private static readonly Dictionary<char, Action<Sandbox>> EscapeTable = new Dictionary<char, Action<Sandbox>>
		{
			{'n', sb => sb.Print('\n')},
			{'N', sb => sb.Print(Environment.NewLine)},
			{'r', sb => sb.Print('\r')},
			{'t', sb => sb.Print('\t')},
			{'b', sb => sb.Print('\b')},
			{'f', sb => sb.Print('\f')},
			{'v', sb => sb.Print('\v')},
			{'0', sb => sb.Print('\0')},
			{'s', sb => sb.Print(' ')},
			{'d', sb => sb.Print(Convert.ToChar(sb.RNG.Next(48, 58)))},
			{'D', sb => sb.Print(Convert.ToChar(sb.RNG.Next(49, 58)))},
			{'c', sb => sb.Print(Char.ToLowerInvariant(sb.Format.Letters[sb.RNG.Next(sb.Format.Letters.Length)]))},
			{'C', sb => sb.Print(Char.ToUpperInvariant(sb.Format.Letters[sb.RNG.Next(sb.Format.Letters.Length)]))},
			{'x', sb => sb.Print("0123456789abcdef"[sb.RNG.Next(16)])},
			{'X', sb => sb.Print("0123456789ABCDEF"[sb.RNG.Next(16)])},
			{
				'w', sb =>
				{
					int i = sb.RNG.Next(sb.Format.Letters.Length + 10);
					if (i >= 10)
					{
						sb.Print(Char.ToLowerInvariant(sb.Format.Letters[i - 10]));
					}
					else
					{
						sb.Print(Convert.ToChar(i + 48));
					}
				}
			},
			{
				'W', sb =>
				{
					int i = sb.RNG.Next(sb.Format.Letters.Length + 10);
					if (i >= 10)
					{
						sb.Print(Char.ToUpperInvariant(sb.Format.Letters[i - 10]));
					}
					else
					{
						sb.Print(Convert.ToChar(i + 48));
					}
				}
			},
			{
				'a', sb =>
				{
					foreach (var ch in sb.CurrentOutput.GetActive())
					{
						ch.WriteArticle();
					}
				}
			}
		};

		private readonly char _code;
		private readonly int _times;
		private bool _unicode;

		public RAEscape(string escapeSequence)
		{
			// The lexer already assures that the string isn't empty.
			int codeIndex = 1; // skip past the backslash

			// parse the quantifier
			if (Char.IsDigit(escapeSequence[codeIndex]) && escapeSequence[codeIndex] != '0')
			{
				int commaIndex = escapeSequence.IndexOf(',', codeIndex + 1);
				if (commaIndex != -1)
				{
					Util.ParseInt(escapeSequence.Substring(1, commaIndex - 1), out _times);
					codeIndex = commaIndex + 1;
				}
			}
			else
			{
				_times = 1;
			}

			// parse the code
			switch (escapeSequence[codeIndex])
			{
				// unicode character is the only special case
				case 'u':
					_code = (char)Convert.ToUInt16(escapeSequence.Substring(codeIndex + 1), 16);
					_unicode = true;
					break;
				// everything else
				default:
					_code = escapeSequence[codeIndex];
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
				Action<Sandbox> func;
				if (!EscapeTable.TryGetValue(_code, out func))
				{
					sb.Print(new string(_code, _times));
				}
				else
				{
					for (int i = 0; i < _times; i++)
					{
						func(sb);
					}
				}
			}
			yield break;
		}
	}
}