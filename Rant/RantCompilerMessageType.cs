namespace Rant
{
	/// <summary>
	/// Defines message types used by the Rant compiler.
	/// </summary>
	public enum RantCompilerMessageType
	{
		/// <summary>
		/// Indicates a problem that did not interfere with compilation.
		/// </summary>
		Warning,

		/// <summary>
		/// Indicates a problem that made compilation impossible, usually a syntax error.
		/// </summary>
		Error
	}
}