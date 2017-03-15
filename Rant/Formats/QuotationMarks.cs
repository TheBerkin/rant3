namespace Rant.Formats
{
	/// <summary>
	/// Represents a configuration for quotation marks.
	/// </summary>
	public sealed class QuotationMarks
	{
		/// <summary>
		/// Initializes a new instance of the QuotationFormat class with the default configuration.
		/// </summary>
		public QuotationMarks()
		{
		}

		/// <summary>
		/// Initializes a new instance of the QuotationFormat class with the specified quotation marks.
		/// </summary>
		/// <param name="openPrimary">The opening primary quote.</param>
		/// <param name="closePrimary">The closing primary quote.</param>
		/// <param name="openSecondary">The opening secondary quote.</param>
		/// <param name="closeSecondary">The closing secondary quote.</param>
		public QuotationMarks(char openPrimary, char closePrimary, char openSecondary, char closeSecondary)
		{
			OpeningPrimary = openPrimary;
			ClosingPrimary = closePrimary;
			OpeningSecondary = openSecondary;
			ClosingSecondary = closeSecondary;
		}

		/// <summary>
		/// The opening primary quotation mark.
		/// </summary>
		public char OpeningPrimary { get; } = '\u201c';

		/// <summary>
		/// The closing primary quotation mark.
		/// </summary>
		public char ClosingPrimary { get; } = '\u201d';

		/// <summary>
		/// The opening secondary quotation mark.
		/// </summary>
		public char OpeningSecondary { get; } = '\u2018';

		/// <summary>
		/// The closing secondary quotation mark.
		/// </summary>
		public char ClosingSecondary { get; } = '\u2019';

		/// <summary>
		/// Returns a string representation of the configuration.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return $"{{ {OpeningPrimary}Primary{ClosingPrimary}, {OpeningSecondary}Secondary{ClosingSecondary} }}";
		}
	}
}
