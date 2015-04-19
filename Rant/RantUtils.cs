using System.Collections.Generic;

using Rant.Engine;

namespace Rant
{
	/// <summary>
	/// Contains miscellaneous utility methods that provide information about the Rant engine.
	/// </summary>
	public static class RantUtils
	{
		/// <summary>
		/// Determines whether a function with the specified name is defined in the current engine version.
		/// </summary>
		/// <param name="functionName">The name of the function to search for. Argument is not case-sensitive.</param>
		/// <returns></returns>
		public static bool FunctionExists(string functionName)
		{
			return RantFunctions.FunctionExists(functionName);
		}

		/// <summary>
		/// Enumerates the names of all available Rant functions.
		/// </summary>
		/// <returns></returns>
		public static IEnumerable<string> GetFunctionNames() => RantFunctions.GetFunctionNames();

		/// <summary>
		/// Returns the description for the function with the specified name.
		/// </summary>
		/// <param name="functionName">The name of the function to get the description for.</param>
		/// <returns></returns>
		public static string GetFunctionDescription(string functionName) => RantFunctions.GetFunctionDescription(functionName);
	}
}