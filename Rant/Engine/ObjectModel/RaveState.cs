using System.Collections;

using Rant.Engine.ObjectModel.Parselets;
using System.Collections.Generic;

namespace Rant.Engine.ObjectModel
{
	/// <summary>
	/// Represents a state in a Rave call stack.
	/// </summary>
	internal class RaveState
	{
		private readonly Precedence _precedence;
		private readonly IEnumerator<bool> _parser;

		public Precedence Precedence => _precedence;

		public IEnumerator<bool> Parser => _parser;

		private Parselet _preParselet;
		private InfixParselet _postParselet;
		
		public RaveState(Rave rave, Precedence precedence)
		{
			_precedence = precedence;
			_parser = CreateParse(rave);
		}
		

		public IEnumerator<bool> CreateParse(Rave rave)
		{
			rave.Reader.SkipSpace();
			var token = rave.Reader.ReadToken();
			rave.Reader.SkipSpace();

			if (RaveParse.PreParselets.TryGetValue(token.ID, out _preParselet))
			{
				var preParser = _preParselet.Parse(token, rave);
				while (preParser.MoveNext()) yield return true;
			}
			else
			{
				throw new RantException(rave.Reader.Source, token, $"Unexpected prefix token: '{token.Value}'");
			}

			while (rave.GetPrecedence() > _precedence)
			{
				token = rave.Reader.ReadToken();
				rave.Reader.SkipSpace();

				if (RaveParse.PostParselets.TryGetValue(token.ID, out _postParselet))
				{
					var postParser = _postParselet.Parse(token, rave);
					while (postParser.MoveNext()) yield return true;
				}
				else
				{
					throw new RantException(rave.Reader.Source, token, $"Unexpected infix token: '{token.Value}'");
				}
			}
		}
	}
}