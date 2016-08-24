namespace Rant.Core.Stringes
{
	/// <summary>
	/// Represents a token with a string value and a custom identifier.
	/// </summary>
	/// <typeparam name="T">The identifier type.</typeparam>
	internal sealed class Token<T> : Stringe where T : struct
	{
		public Token(T id, string value) : base(value)
		{
			ID = id;
		}

		public Token(T id, Stringe value) : base(value)
		{
			ID = id;
		}

		/// <summary>
		/// The token identifier.
		/// </summary>
		public T ID { get; }

		/// <summary>
		/// Returns a string representation of the current token.
		/// </summary>
		/// <returns></returns>
		public override string ToString() => $"{ID}, L{Line}, C{Column}{(string.IsNullOrEmpty(Value) ? "" : $", {Value} ")}";
	}
}