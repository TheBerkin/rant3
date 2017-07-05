using System;

using Rant.Metadata;

namespace Rant.Vocabulary.Utilities
{
	[Flags]
	internal enum RhymeFlags : byte
	{
		[RantDescription("Everything after the first stressed vowel matches in pronunciation (picky / icky).")]
		Perfect = 0x01,

		[RantDescription("The penultimate syllable is stressed and the final syllable rhymes (coffin / raisin).")]
		Weak = 0x02,

		[RantDescription("The final syllable rhymes (senator / otter).")]
		Syllabic = 0x04,

		[RantDescription("The words would rhyme if not for the final syllable (broom / broomstick).")]
		Semirhyme = 0x08,

		[RantDescription("The words might rhyme if you really pushed it.")]
		Forced = 0x10,

		[RantDescription("The ending consonants are the same (rant / ant).")]
		SlantRhyme = 0x20,

		[RantDescription("All the consonants match (tuna / teen).")]
		Pararhyme = 0x40,

		[RantDescription("All consonants up to the first vowel rhyme (dog / dude).")]
		Alliteration = 0x80
	}
}