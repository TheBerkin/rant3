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
		private const RegexOptions DefaultOptions = RegexOptions.Compiled | RegexOptions.ExplicitCapture;

		internal static readonly Regex EscapeRegex = new Regex(@"\\((?<count>\d+((\.\d+)?[kMB])?),)?((?<code>[^u\s\r\n])|u(?<unicode>[0-9a-f]{4}))", DefaultOptions);
		internal static readonly Regex RegexRegex = new Regex(@"`((\\[\\`]|.)*?)?`i?", DefaultOptions);

		private static readonly Regex WeightRegex = new Regex(@"\*[ ]*(?<value>\d+(\.\d+)?)[ ]*\*", DefaultOptions);
		private static readonly Regex WhitespaceRegex = new Regex(@"\s+", DefaultOptions);
		private static readonly Regex BlackspaceRegex = new Regex(@"((^|(?<=(?<!\\)[\[\{]))\s+|\s*[\r\n]+\s*|\s+($|(?=[\]\}])))", DefaultOptions | RegexOptions.Multiline);
		private static readonly Regex CommentRegex = new Regex(@"\s*#.*?(?=[\r\n]|$)", DefaultOptions | RegexOptions.Multiline);
		private static readonly Regex ConstantLiteralRegex = new Regex(@"""([^""]|"""")*""", DefaultOptions);
		private static readonly Regex SyllableRangeRegex = new Regex(@"\(\s*(~?\d+|\d+\s*~(\s*\d+)?)\s*\)", DefaultOptions);
		private static readonly Regex NumberRegex = new Regex(@"(?<!\w)-?\d+(\.\d+)?", DefaultOptions);

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
				{CommentRegex, R.Ignore, 3},
				{BlackspaceRegex, R.Ignore, 2},
				{EscapeRegex, R.EscapeSequence},
				{RegexRegex, R.Regex},
				{ConstantLiteralRegex, R.ConstantLiteral},
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
				{"var", R.Var},
				{NumberRegex, R.Number},
				//{SyllableRangeRegex, R.RangeLiteral},
				//{WeightRegex, R.Weight},
				{WhitespaceRegex, R.Whitespace}
			};
			Rules.AddUndefinedCaptureRule(R.Text, TruncatePadding);
			Rules.AddEndToken(R.EOF);
			Rules.Ignore(R.Ignore);
		}

		/// <summary>
		/// Generates beautiful tokens.
		/// </summary>
		/// <param name="input">The input string to tokenize.</param>
		/// <returns></returns>
		public static IEnumerable<Token<R>> GenerateTokens(Stringe input)
		{
			var reader = new StringeReader(input);
			while (!reader.EndOfStringe)
			{
				yield return reader.ReadToken(Rules);
			}
		}
	}
}