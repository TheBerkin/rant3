using System;

namespace Rant
{
	/// <summary>
	/// Represents an error that has been caused by a problem inside the Rant engine. This typically indicates the presence of
	/// a bug.
	/// </summary>
	public sealed class RantInternalException : Exception
	{
		internal RantInternalException()
			: base("An internal error has occurred. This is often caused by a bug in Rant.")
		{
		}

		internal RantInternalException(string message)
			: base($"An internal error has occurred. This is often caused by a bug in Rant. Message: {message}.")
		{
		}

		internal RantInternalException(string message, Exception inner)
			: base($"An internal error has occurred. This is often caused by a bug in Rant. Message: {message}.", inner)
		{
		}
	}
}