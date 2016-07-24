namespace Rant.Internals.Engine.Constructs
{
	/// <summary>
	/// Defines persistence modes for block attributes.
	/// </summary>
	internal enum AttribPersistence
	{
		/// <summary>
		/// The next block consumes attributes immediately.
		/// </summary>
		Off,
		/// <summary>
		/// The next block consumes attributes and restores them when finished.
		/// </summary>
		Outer,
		/// <summary>
		/// The next block uses but does not consume attributes.
		/// They are inherited by and restored at the end of each child block.
		/// </summary>
		OuterShared,
		/// <summary>
		/// The current attributes are inherited by all blocks inside the current block.
		/// They are consumed at the end of the block.
		/// </summary>
		Inner,
		/// <summary>
		/// The current attributes are inherited by all blocks inside the current block, as well as their children.
		/// They are consumed at the end of the block.
		/// </summary>
		InnerShared,
		/// <summary>
		/// Disable changes to the current block attributes until the persistence mode is changed.
		/// </summary>
		ReadOnly,
		/// <summary>
		/// Attributes will persist over the next block, but any block after or inside it will consume them.
		/// </summary>
		Once,
		/// <summary>
		/// The next block uses but does not consume attributes. This also affects child blocks.
		/// </summary>
		On
	}
}