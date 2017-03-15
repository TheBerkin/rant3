using System.Collections.Generic;
using System.Linq;

namespace Rant.Formats
{
	/// <summary>
	/// Represents configuration settings for a language's writing system.
	/// </summary>
	public sealed class WritingSystem
	{
		private readonly char[] _alphabet;
		private readonly string _space;

		public WritingSystem(IEnumerable<char> alphabet, string space, QuotationMarks quotations)
		{
			_alphabet = alphabet.ToArray();
			_space = space;
			Quotations = quotations;
		}

		public WritingSystem()
		{
			_alphabet = new[] 
			{
				'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
				'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'
			};
			_space = " ";
			Quotations = new QuotationMarks();
		}

		internal char[] AlphabetArray => _alphabet;

		public IEnumerable<char> GetAlphabet() => _alphabet.AsEnumerable();

		public QuotationMarks Quotations { get; }

		public string Space => _space;
	}
}
