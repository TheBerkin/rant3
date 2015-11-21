using System;
using System.Collections.Generic;
using System.Linq;

using Rant.Engine;

namespace Rant.Formats
{
	/// <summary>
	/// Pluralizer for English nouns.
	/// </summary>
	public sealed class EnglishPluralizer : Pluralizer
	{
		private static readonly HashSet<char> consonants = new HashSet<char>(new[] { 'b', 'c', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'n', 'p', 'q', 'r', 's', 't', 'v', 'w', 'x', 'y', 'z' });
		private static readonly HashSet<char> hardConsonants = new HashSet<char>(new[] { 'b', 'c', 'd', 'f', 'g', 'j', 'm', 'q', 's', 'v', 'x', 'z' });

		private static readonly Dictionary<string, string> irregulars = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
		{
			{"man", "men"},
			{"foot", "feet"},
			{"tooth", "teeth"},
			{"antenna", "antennae"},
			{"thesis", "theses"},
			{"axis", "axes"},
			{"basis", "bases"},
			{"leaf", "leaves"},
			{"calf", "calves"},
			{"knife", "knives"},
			{"life", "lives"},
			{"dwarf", "dwarves"},
			{"wolf", "wolves"},
			{"hoof", "hooves"},
			{"elf", "elves"},
			{"goose", "geese"},
			{"louse", "lice"},
			{"mouse", "mice"},
			{"dormouse", "dormice"},
			{"person", "people"},
			{"die", "dice"},
			{"index", "indices"},
			{"matrix", "matrices"},
			{"vertex", "vertices"},
			{"criterion", "criteria"},
			{"passerby", "passersby"},

			{"alumnus", "alumni"},
			{"cactus", "cacti"},
			{"fungus", "fungi"},
			{"focus", "foci"},
			{"nucleus", "nuclei"},
			{"radius", "radii"},
			{"stimulus", "stimuli"},
			{"child", "children"}
		};

		private static readonly HashSet<string> ignore = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase)
		{
			"deer",
			"fish",
			"means",
			"offspring",
			"series",
			"sheep",
			"species",
			"trout",
			"bison",
			"buffalo",
			"moose",
			"pike",
			"plankton",
			"salmon",
			"squid",
			"swine"
		};


		public override string Pluralize(string input)
		{
			if (Util.IsNullOrWhiteSpace(input)) return input;
			input = input.Trim().ToLowerInvariant();
			int l = input.Length;
			if (l == 1) return input.ToUpperInvariant() + "'s";
			if (ignore.Contains(input)) return input;
			string result;
			if (irregulars.TryGetValue(input, out result)) return result;
			if ((result = irregulars.Keys.FirstOrDefault(w => input.EndsWith(w))) != null)
				return input.Substring(0, l - result.Length) + irregulars[result];

			// With nouns ending in o preceded by a consonant, the plural in many cases is spelled by adding -es...
			if (input.EndsWith("o") && consonants.Contains(input[l - 2])) return input + "es";

			// Nouns ending in a y preceded by a consonant usually drop the y and add -ies...
			if ((input.EndsWith("y") || input.EndsWith("quy")) && consonants.Contains(input[l - 2])) return input.Substring(0, l - 1) + "ies";

			// Plurals of words ending in "man" end in "men"
			if (input.EndsWith("man")) return input.Substring(0, l - 2) + "en";
			
			// Add -es to words ending in a hard consonant
			if (input.EndsWith("ch") || input.EndsWith("sh") || hardConsonants.Contains(input[l - 1])) return input + "es";

			// Default
			return input + "s";
		}
	}
}