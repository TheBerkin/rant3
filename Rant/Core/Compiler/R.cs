namespace Rant.Core.Compiler
{
	internal enum R
	{
		Text,

		EscapeSequenceChar,

		EscapeSequenceQuantifier,

		EscapeSequenceUnicode,

		EscapeSequenceSurrogatePair,

		LeftSquare,

		RightSquare,

		LeftCurly,

		RightCurly,

		LeftAngle,

		RightAngle,

		LeftParen,

		RightParen,

		Pipe,

		Colon,

		Semicolon,

		DoubleColon,

		At,

		Question,

		ForwardSlash,

		Exclamation,

		Dollar,

		Hyphen,

		Comma,

		Without,

		Regex,

		RegexFlags,

		VerbatimString,

		Whitespace,

		Equal,

		Ampersand,

		Caret,

		Plus,

		Number,

		Period,

		EOF
	}
}