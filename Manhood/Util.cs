using System;
using System.Collections.Generic;

namespace Manhood
{
    internal class Util
    {
        private static readonly Dictionary<char, Func<RNG, char>> _escapeChars = new Dictionary<char, Func<RNG, char>>
        {
            {'n', (rng) => '\n'},
            {'r', (rng) => '\r'},
            {'t', (rng) => '\t'},
            {'b', (rng) => '\b'},
            {'f', (rng) => '\f'},
            {'v', (rng) => '\v'},
            {'0', (rng) => '\0'},
            {'s', (rng) => ' '},
            {'d', (rng) => rng == null ? '0' : Convert.ToChar(rng.Next(48, 58))},
            {'c', (rng) => rng == null ? '?' : Convert.ToChar(rng.Next(97, 123))},
            {'C', (rng) => rng == null ? '?' : Convert.ToChar(rng.Next(65, 91))},
            {'x', (rng) => rng == null ? '?' : "0123456789abcdef"[rng.Next(16)]},
            {'X', (rng) => rng == null ? '?' : "0123456789ABCDEF"[rng.Next(16)]}
        };

        public static char Escape(char code, RNG rng = null)
        {
            Func<RNG, char> func;
            return !_escapeChars.TryGetValue(code, out func) ? code : func(rng);
        }
    }
}