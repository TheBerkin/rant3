namespace Rant.Core.Compiler
{
	/// <summary>
	/// Distinguishes execution contexts to the compiler to make it aware of context-specific token behaviors.
	/// </summary>
	internal enum CompileContext
	{
		/// <summary>
		/// Point of execution is outside of any specialized constructs.
		/// </summary>
		DefaultSequence,

		/// <summary>
		/// Point of execution is inside a tag argument.
		/// </summary>
		ArgumentSequence,

		/// <summary>
		/// Point of execution is inside of a block.
		/// </summary>
		BlockSequence
	}
}