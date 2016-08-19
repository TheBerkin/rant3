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
		BlockSequence,

		/// <summary>
		/// We're reading a block weight.
		/// </summary>
		BlockWeight,

		/// <summary>
		/// We're reading a subroutine body.
		/// </summary>
		SubroutineBody,

		/// <summary>
		/// The end of a block.
		/// </summary>
		BlockEndSequence,

		/// <summary>
		/// The end of function arguments.
		/// </summary>
		FunctionEndContext,

		/// <summary>
		/// Complement pattern for queries.
		/// </summary>
		QueryComplement
	}
}