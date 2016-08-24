using System;
using System.Collections.Generic;
using System.Linq;

namespace Rant.Core.Output
{
	internal class OutputChainArticleBuffer : OutputChainBuffer
	{
		private static readonly HashSet<char> vowels =
			new HashSet<char>(new[] { 'a', 'e', 'i', 'o', 'u', 'A', 'E', 'I', 'O', 'U', 'é', 'É' });

		private static readonly string[] ignorePrefixes =
		{ "uni", "use", "uri", "urol", "U.", "one", "uvu", "eul", "euk", "eur" };

		private static readonly string[] allowPrefixes =
		{ "honest", "honor", "hour", "8" };

		private static readonly string[] ignoreWords = { "u" };

		private static readonly string[] allowWords =
		{ "f", "fbi", "fcc", "fda", "x", "l", "m", "n", "s", "h" };

		public OutputChainArticleBuffer(Sandbox sb, OutputChainBuffer prev) : base(sb, prev)
		{
			Initialize();
		}

		public OutputChainArticleBuffer(Sandbox sb, OutputChainBuffer prev, OutputChainBuffer targetOrigin)
			: base(sb, prev, targetOrigin)
		{
			Initialize();
		}

		private void Initialize()
		{
			Print("a");
		}

		protected override void OnNextBufferChange()
		{
			if (Next.Length == 0) return;
			var sb = Next.Buffer;
			char c;
			int start = -1;
			int end = 0;
			for (int i = 0; i < sb.Length; i++)
			{
				c = sb[i];
				if (start == -1)
				{
					if (char.IsWhiteSpace(c) || char.IsSeparator(c)) continue; // Must be padding, skip it
					if (!char.IsLetterOrDigit(c)) continue;
					start = i;
					if (i == sb.Length - 1) end = start + 1; // Word is one character long
				}
				else
				{
					end = i;
					if (!char.IsLetterOrDigit(c)) break;
					if (i == sb.Length - 1) end++; // Consume character if it's the last one in the buffer
				}
			}

			if (start == -1) return;

			var buffer = new char[end - start];
			sb.CopyTo(start, buffer, 0, end - start);
			Clear();
			Print(CheckRules(new string(buffer)) ? "an" : "a");
		}

		private static bool CheckRules(string value)
		{
			if (string.IsNullOrEmpty(value)) return false;
			return
				(allowWords.Any(word => string.Equals(word, value, StringComparison.InvariantCultureIgnoreCase))
				 || allowPrefixes.Any(pfx => value.StartsWith(pfx, StringComparison.InvariantCultureIgnoreCase)))
				|| (vowels.Contains(value[0])
				    && !ignorePrefixes.Any(pfx => value.StartsWith(pfx, StringComparison.InvariantCultureIgnoreCase))
				    && !ignoreWords.Any(word => string.Equals(word, value, StringComparison.InvariantCultureIgnoreCase)));
		}
	}
}