namespace Rant.Formats
{
	/// <summary>
	/// The base class for pluralizers, which infer the plural form of a given noun.
	/// </summary>
	public abstract class Pluralizer
	{
		/// <summary>
		/// Converts the specified input noun to a plural version.
		/// </summary>
		/// <param name="input">The noun to convert.</param>
		/// <returns></returns>
		public abstract string Pluralize(string input);
	}
}