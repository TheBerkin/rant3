using System.Text;

namespace Rant.Core.Formatting
{
	internal static class CharConverter
	{
		public static char ToFullwidth(char c)
		{
			if (c < '!' || c > '~') return c;
			return (char)(c + 0xfee0);
		}

		public static string ToScript(char c)
		{
			switch (c)
			{
				case 'a':
				return "\U0001d4b6";
				case 'b':
				return "\U0001d4b7";
				case 'c':
				return "\U0001d4b8";
				case 'd':
				return "\U0001d4b9";
				case 'e':
				return "\u212f";
				case 'f':
				return "\U0001d4bb";
				case 'g':
				return "\u210a";
				case 'h':
				return "\U0001d4bd";
				case 'i':
				return "\U0001d4be";
				case 'j':
				return "\U0001d4bf";
				case 'k':
				return "\U0001d4c0";
				case 'l':
				return "\U0001d4c1";
				case 'm':
				return "\U0001d4c2";
				case 'n':
				return "\U0001d4c3";
				case 'o':
				return "\u2134";
				case 'p':
				return "\U0001d4c5";
				case 'q':
				return "\U0001d4c6";
				case 'r':
				return "\U0001d4c7";
				case 's':
				return "\U0001d4c8";
				case 't':
				return "\U0001d4c9";
				case 'u':
				return "\U0001d4ca";
				case 'v':
				return "\U0001d4cb";
				case 'w':
				return "\U0001d4cc";
				case 'x':
				return "\U0001d4cd";
				case 'y':
				return "\U0001d4ce";
				case 'z':
				return "\U0001d4cf";
				case 'A':
				return "\U0001d49c";
				case 'B':
				return "\u212c";
				case 'C':
				return "\U0001d49e";
				case 'D':
				return "\U0001d49f";
				case 'E':
				return "\u2130";
				case 'F':
				return "\u2131";
				case 'G':
				return "\U0001d4a2";
				case 'H':
				return "\u210b";
				case 'I':
				return "\u2110";
				case 'J':
				return "\U0001d4a5";
				case 'K':
				return "\U0001d4a6";
				case 'L':
				return "\u2112";
				case 'M':
				return "\u2133";
				case 'N':
				return "\U0001d4a9";
				case 'O':
				return "\U0001d4aa";
				case 'P':
				return "\U0001d4ab";
				case 'Q':
				return "\U0001d4ac";
				case 'R':
				return "\u211b";
				case 'S':
				return "\U0001d4ae";
				case 'T':
				return "\U0001d4af";
				case 'U':
				return "\U0001d4b0";
				case 'V':
				return "\U0001d4b1";
				case 'W':
				return "\U0001d4b2";
				case 'X':
				return "\U0001d4b3";
				case 'Y':
				return "\U0001d4b4";
				case 'Z':
				return "\U0001d4b5";
				default:
				return c.ToString();
			}
		}

		public static string ToBoldScript(char c)
		{
			const ushort a = 0x9390;
			const ushort b = 0x93aa;
			if (c >= 'A' && c <= 'Z')
			{
				ushort d = unchecked((ushort)(a + (c - 'A')));
				return Encoding.UTF8.GetString(new byte[] { 0xf0, 0x9d, (byte)((d & 0xff00) >> 8), (byte)(d & 0xff) }, 0, 4);
			}
			if (c >= 'a' && c <= 'z')
			{
				ushort d = unchecked((ushort)(b + (c - 'a')));
				return Encoding.UTF8.GetString(new byte[] { 0xf0, 0x9d, (byte)((d & 0xff00) >> 8), (byte)(d & 0xff) }, 0, 4);
			}
			return c.ToString();
		}
	}
}
