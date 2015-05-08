using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Rant.Engine.Formatters;
using Rant.Stringes;

namespace Rant.Engine.Compiler
{
#if EDITOR
	/// <summary>
	/// Generates tokens from Rant patterns.
	/// </summary>
	public static class RantLexer
#else
    internal static class RantLexer
#endif

    {
        internal static readonly Lexer<R> Rules;

        private static Stringe TruncatePadding(Stringe input)
        {
            var ls = input.LeftPadded ? input.TrimStart() : input;
            return ls.RightPadded ? ls.TrimEnd() : ls;
        }

        static RantLexer()
        {
            Rules = new Lexer<R>
            {
                {reader => reader.EatWhile(Char.IsWhiteSpace), R.Whitespace},
                // Comments
                {
                    reader =>
                    {
                        reader.SkipWhiteSpace();
                        if (!reader.Eat('#')) return false;
                        reader.ReadUntilAny('\r', '\n');
                        return true;
                    }, R.Ignore, 3
                },
                // Blackspace (^\s*)
                {
                    reader =>
                    {
                        if (reader.Position == 0) return reader.EatWhile(Char.IsWhiteSpace);
                        return reader.IsNext('\r', '\n') && reader.EatWhile(Char.IsWhiteSpace);
                    }, R.Ignore, 2
                },
                // Blackspace (\s*[\r\n]+\s*)
                {
                    reader =>
                    {
                        reader.EatWhile(
                            c => Char.IsWhiteSpace(c.Character) && c.Character != '\r' && c.Character != '\n');
                        if (!reader.EatAll('\r', '\n')) return false;
                        reader.EatWhile(Char.IsWhiteSpace);
                        return true;
                    }, R.Ignore, 2
                },
                // Blackspace (\s*$)
                {
                    reader =>
                    {
                        while (!reader.EndOfStringe)
                        {
                            if (reader.EatAll('\r', '\n')) return true;
                            if (!reader.SkipWhiteSpace()) return false;
                        }
                        return true;
                    }, R.Ignore, 2
                },
                // Escape sequence
                {
                    reader =>
                    {
                        if (!reader.Eat('\\')) return false;
                        if (reader.EatWhile(Char.IsDigit))
                        {
                            if (reader.Eat('.')) reader.EatWhile(Char.IsDigit);
                            reader.EatAny('k', 'M', 'B');
                            if (!reader.Eat(','))
                                throw new RantCompilerException(reader.Origin, reader.Stringe.Substringe(reader.Position - 1, 1), "Expected ',' after quantifier.");
                        }
                        if (reader.Eat('u'))
                        {   
                            for (int i = 0; i < 4; i++)
                            {
                                var c = reader.ReadChare().Character;
                                if (!((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F')))
                                    return false;
                            }
                        }
                        else
                        {
                            reader.ReadChare();
                        }
                        return true;
                    }, R.EscapeSequence
                },
                // Regex
                {
                    reader =>
                    {
                        if (!reader.Eat('`')) return false;
                        var token = reader.Stringe.Substringe(reader.Position - 1, 1);
                        while (!reader.EndOfStringe)
                        {
                            reader.EatAll("\\`");
                            if (reader.ReadChare() == '`') break;
                        }
                        if (reader.EndOfStringe) 
                            throw new RantCompilerException(reader.Origin, token, "Unterminated regular expression.");
                        reader.Eat('i');
                        return true;
                    }, R.Regex
                },
                // Constant literal
                {
                    reader =>
                    {
                        if (!reader.Eat('"')) return false;
                        var token = reader.Stringe.Substringe(reader.Position - 1, 1);
                        while (!reader.EndOfStringe)
                        {
                            reader.EatAll("\"\"");
                            if (reader.ReadChare() == '"') return true;
                        }
                        throw new RantCompilerException(reader.Origin, token, "Unterminated constant literal.");
                    }, R.ConstantLiteral
                },
                {"[", R.LeftSquare}, {"]", R.RightSquare},
                {"{", R.LeftCurly}, {"}", R.RightCurly},
                {"<", R.LeftAngle}, {">", R.RightAngle},
                {"(", R.LeftParen},
                {")", R.RightParen},
                {"|", R.Pipe},
                {";", R.Semicolon},
                {":", R.Colon},
                {"@", R.At},
                {"?", R.Question},
                {"::", R.DoubleColon},
                {"?!", R.Without},
                {"-", R.Hyphen},
                {SymbolCodes.EnDash, R.Text},
                {SymbolCodes.EmDash, R.Text},
                {SymbolCodes.Copyright, R.Text, true},
                {SymbolCodes.RegisteredTM, R.Text, true},
                {SymbolCodes.Trademark, R.Text, true},
                {SymbolCodes.Eszett, R.Text, true},
                {SymbolCodes.Bullet, R.Text, true},
                {"!", R.Exclamation},
                {"$", R.Dollar},
                {"=", R.Equal},
                {"&", R.Ampersand},
                {"%", R.Percent},
                {"+", R.Plus},
                {"^", R.Caret},
                {"`", R.Backtick},
                {"*", R.Asterisk},
                {"/", R.ForwardSlash},
                {",", R.Comma},
                {".", R.Subtype}
            };
            Rules.AddUndefinedCaptureRule(R.Text, TruncatePadding);
            Rules.AddEndToken(R.EOF);
            Rules.Ignore(R.Ignore);
        }

        /// <summary>
        /// Generates beautiful tokens.
        /// </summary>
        /// <param name="name">The source name of the input.</param>
        /// <param name="input">The input string to tokenize.</param>
        /// <returns></returns>
        public static IEnumerable<Token<R>> GenerateTokens(string name, Stringe input)
        {
            var reader = new StringeReader(input)
            {
                Origin = name
            };
            while (!reader.EndOfStringe)
            {
                yield return reader.ReadToken(Rules);
            }
        }
    }
}