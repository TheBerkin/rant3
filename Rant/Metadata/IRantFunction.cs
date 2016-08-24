using System.Collections.Generic;

namespace Rant.Metadata
{
	/// <summary>
	/// Provides access to metadata for a Rant function overload.
	/// </summary>
	public interface IRantFunction
	{
		/// <summary>
		/// Gets the name of the function.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets the description for the function overload.
		/// </summary>
		string Description { get; }

		/// <summary>
		/// Indicates whether the last parameter accepts multiple values.
		/// </summary>
		bool HasParamArray { get; }

		/// <summary>
		/// Gets the number of parameters accepted by the function overload.
		/// </summary>
		int ParamCount { get; }

		/// <summary>
		/// Enumerates the parameters for the function overload.
		/// </summary>
		/// <returns></returns>
		IEnumerable<IRantParameter> GetParameters();
	}
}