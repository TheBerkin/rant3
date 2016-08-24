namespace Rant.Core.IO.Bson
{
	/// <summary>
	/// Defines the available string table types for BSON documents.
	/// </summary>
	public enum BsonStringTableMode
	{
		/// <summary>
		/// Don't use a string table.
		/// </summary>
		None = 0,

		/// <summary>
		/// Store only keys.
		/// </summary>
		Keys = 1,

		/// <summary>
		/// Store keys and values.
		/// </summary>
		KeysAndValues = 2
	}
}