namespace Rant.Formats
{
	/// <summary>
	/// The base class for pluralizers, which infer the plural form of a given noun.
	/// </summary>
	public abstract class Pluralizer
	{
		public abstract string Pluralize(string input);
	}
}