using System.Collections.Generic;

using Rant.Engine.Compiler;
using Rant.Engine.ObjectModel.Parselets;

namespace Rant.Engine.ObjectModel
{
	internal static class RaveParse
	{
		public static readonly Dictionary<R, Parselet> PreParselets;
		public static readonly Dictionary<R, InfixParselet> PostParselets;

		static RaveParse()
		{
			PreParselets = new Dictionary<R, Parselet>
			{
				{R.Number, new NumberParselet()},
				{R.LeftParen, new GroupParselet()},
				{R.ConstantLiteral, new StringParselet()},
                {R.EscapeSequence, new EscapeParselet()},
				{R.Var, new VarParselet()},
				{R.Text, new NameParselet()}
			};

			PostParselets = new Dictionary<R, InfixParselet>
			{
				{R.Plus, new BinaryOperatorParselet(Precedence.Sum, false)},
				{R.Hyphen, new BinaryOperatorParselet(Precedence.Sum, false)},
				{R.Asterisk, new BinaryOperatorParselet(Precedence.Product, false)},
				{R.ForwardSlash, new BinaryOperatorParselet(Precedence.Product, false)},
				{R.Equal, new BinaryOperatorParselet(Precedence.Assignment, true)},

				{R.LeftSquare, new IndexerParselet()}
			};
		}
	}
}