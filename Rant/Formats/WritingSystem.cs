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

		/// <summary>
		/// Creates a new writing system with the specified configuration.
		/// </summary>
		/// <param name="alphabet">The alphabet to use.</param>
		/// <param name="space">The standard space to use.</param>
		/// <param name="quotations">The quotation marks to use.</param>
		public WritingSystem(IEnumerable<char> alphabet, string space, QuotationMarks quotations)
		{
			_alphabet = alphabet.ToArray();
			_space = space;
			Quotations = quotations;
		}

		/// <summary>
		/// Creates a new writing system with the default configuration.
		/// </summary>
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

		/// <summary>
		/// The alphabet used by the format.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<char> GetAlphabet() => _alphabet.AsEnumerable();

		/// <summary>
		/// The quotation marks used by the format.
		/// </summary>
		public QuotationMarks Quotations { get; }
		
		/// <summary>
		/// The standard space used by series and phrasals.
		/// </summary>
		public string Space => _space;
	}
}
