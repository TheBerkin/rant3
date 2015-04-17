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
	}
}