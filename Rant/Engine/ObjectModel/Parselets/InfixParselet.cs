using System;

namespace Rant.Engine.ObjectModel.Parselets
{
	internal abstract class InfixParselet : Parselet
	{
		public Precedence Precedence { get; set; }

		public InfixParselet(Precedence precedence)
		{
			Precedence = precedence;
		}
	}
}