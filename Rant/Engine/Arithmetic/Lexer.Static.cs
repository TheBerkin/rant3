using System.Text.RegularExpressions;
using Rant.Stringes.Tokens;

namespace Rant.Arithmetic
{
    internal partial class Lexer
    {
        public static LexerRules<RMathToken> Rules;
		
        static Lexer()
        {
            Rules = new LexerRules<RMathToken>
            {
                {"+", RMathToken.Plus},
				{"-", RMathToken.Minus},
				{"*", RMathToken.Asterisk},
				{"/", RMathToken.Slash},
				{"^", RMathToken.Caret},
				{"(", RMathToken.LeftParen},
				{")", RMathToken.RightParen},
				{"++", RMathToken.Increment},
				{"--", RMathToken.Decrement},
				{"%", RMathToken.Modulo},
				{"=", RMathToken.Equals},
                {"$=", RMathToken.Swap},
                {"+=", RMathToken.AddAssign},
                {"-=", RMathToken.SubAssign},
                {"*=", RMathToken.MulAssign},
                {"/=", RMathToken.DivAssign},
                {"%=", RMathToken.ModAssign},
                {"^=", RMathToken.PowAssign},
                {"|", RMathToken.Pipe},
                {new Regex(@"(\d+(\.\d+)?|\.\d+)"), RMathToken.Number},
                {new Regex(@"[a-zA-Z_][a-zA-Z0-9_]*"), RMathToken.Name}
            };
            Rules.AddEndToken(RMathToken.End);
        }
    }
}