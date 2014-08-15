using System;
using System.Collections.Generic;
using System.Linq;

namespace Manhood
{
    internal partial class Lexer
    {
        public static readonly List<Tuple<string, TokenType>> Operators;
        private static readonly HashSet<char> Punctuation = new HashSet<char>();
		
        static Lexer()
        {
			Operators = new List<Tuple<string, TokenType>>()
			{
			    Tuple.Create("+", TokenType.Plus),
				Tuple.Create("-", TokenType.Minus),
				Tuple.Create("*", TokenType.Asterisk),
				Tuple.Create("/", TokenType.Slash),
				Tuple.Create("^", TokenType.Caret),
				Tuple.Create("(", TokenType.LeftParen),
				Tuple.Create(")", TokenType.RightParen),
				Tuple.Create("++", TokenType.Increment),
				Tuple.Create("--", TokenType.Decrement),
				Tuple.Create("%", TokenType.Modulo),
				Tuple.Create("=", TokenType.Equals)
			};

            Operators = Operators.OrderByDescending(o => o.Item1.Length).ToList();

            foreach (var value in Operators.Select(o => o.Item1[0]))
            {
                Punctuation.Add(value);
            }
        }
    }
}