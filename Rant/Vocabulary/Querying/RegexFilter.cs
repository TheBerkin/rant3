using System.Text.RegularExpressions;

using Rant.Core.IO;

namespace Rant.Vocabulary.Querying
{
	internal sealed class RegexFilter : Filter
	{
		public RegexFilter(Regex regex, bool outcome)
		{
			Regex = regex;
			Outcome = outcome;
		}

		public RegexFilter()
		{
			// Used by serializer
		}

		public Regex Regex { get; set;  }

		public bool Outcome { get; set; }

		public override bool Test(RantDictionary dictionary, RantDictionaryTable table, RantDictionaryEntry entry, int termIndex, Query query) => Regex.IsMatch(entry[termIndex].Value) == Outcome;

		public override void Deserialize(EasyReader input)
		{
			RegexOptions options = RegexOptions.Compiled | RegexOptions.ExplicitCapture;
			Outcome = input.ReadBoolean();
			options |= (RegexOptions)input.ReadInt32();
			Regex = new Regex(input.ReadString(), options);
		}

		public override void Serialize(EasyWriter output)
		{
			output.Write(FILTER_REGEX);
			output.Write(Outcome);
			output.Write((int)Regex.Options);
			output.Write(Regex.ToString());
		}
	}
}