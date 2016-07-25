namespace Rant.Vocabulary
{
	/// <summary>
	/// Defines merging behaviors for Rant dictionary tables.
	/// </summary>
	public enum TableMergeBehavior
	{
		/// <summary>
		/// Combine all entries, don't remove duplicates.
		/// </summary>
		Naive,
		/// <summary>
		/// Remove entries whose first term matches another entry.
		/// </summary>
		RemoveFirstTermDuplicates,
		/// <summary>
		/// Remove entries whose terms match another entry.
		/// </summary>
		RemoveEntryDuplicates
	}
}