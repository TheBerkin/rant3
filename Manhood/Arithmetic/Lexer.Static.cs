using System.Text.RegularExpressions;

using Stringes.Tokens;

namespace Manhood.Arithmetic
{
    internal partial class Lexer
    {
        public static LexerRules<TokenType> Rules;
		
        static Lexer()
        {
            Rules = new LexerRules<TokenType>
            {
                {"+", TokenType.Plus},
				{"-", TokenType.Minus},
				{"*", TokenType.Asterisk},
				{"/", TokenType.Slash},
				{"^", TokenType.Caret},
				{"(", TokenType.LeftParen},
				{")", TokenType.RightParen},
				{"++", TokenType.Increment},
				{"--", TokenType.Decrement},
				{"%", TokenType.Modulo},
				{"=", TokenType.Equals},
                {"$=", TokenType.Swap},
                {"+=", TokenType.AddAssign},
                {"-=", TokenType.SubAssign},
                {"*=", TokenType.MulAssign},
                {"/=", TokenType.DivAssign},
                {"%=", TokenType.ModAssign},
                {"^=", TokenType.PowAssign},
                {new Regex(@"-?(\d+(\.\d+)?|\.\d+)"), TokenType.Number},
                {new Regex(@"[a-zA-Z_][a-zA-Z0-9_]*"), TokenType.Name}
            };
            Rules.AddEndToken(TokenType.End);
        }
    }
}