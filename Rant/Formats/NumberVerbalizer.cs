namespace Rant.Formats
{
	/// <summary>
	/// The base class for all number verbalizers.
	/// </summary>
	public abstract class NumberVerbalizer
	{
		/// <summary>
		/// Verbalizes the specified value.
		/// </summary>
		/// <param name="number">The number to verbalize.</param>
		/// <returns></returns>
		public abstract string Verbalize(long number);
	}
}
