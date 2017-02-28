#region License

// https://github.com/TheBerkin/Rant
// 
// Copyright (c) 2017 Nicholas Fleck
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in the
// Software without restriction, including without limitation the rights to use, copy,
// modify, merge, publish, distribute, sublicense, and/or sell copies of the Software,
// and to permit persons to whom the Software is furnished to do so, subject to the
// following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
// PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE
// OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

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

		public Regex Regex { get; set; }
		public bool Outcome { get; set; }
		public override int Priority => 100;
		public override bool Test(RantDictionary dictionary, RantDictionaryTable table, RantDictionaryEntry entry, int termIndex, Query query) => Regex.IsMatch(entry[termIndex].Value) == Outcome;

		public override void Deserialize(EasyReader input)
		{
			var options = RegexOptions.Compiled | RegexOptions.ExplicitCapture;
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