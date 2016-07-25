namespace Rant.Vocabulary.Querying
{
	internal class TableSource
	{
		public string Name { get; }
		public string Subtype { get; }
		public EntryVariantHint Hint { get; } = EntryVariantHint.None;

		public TableSource(string name, string subtype, EntryVariantHint hint = EntryVariantHint.None)
		{
			Name = name;
			Subtype = subtype;
			Hint = hint;
		}
	}
}