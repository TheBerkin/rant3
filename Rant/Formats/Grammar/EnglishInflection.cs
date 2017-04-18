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

using System;
using System.Collections.Generic;

namespace Rant.Formats.Grammar
{
	internal class EnglishInflection : Inflection
	{
		private static readonly Dictionary<string, PastForms> irregularSimplePastForms;

		static EnglishInflection()
		{
			irregularSimplePastForms = new Dictionary<string, PastForms>
			{
				{ "eat", new PastForms("ate", "eaten") },
				{ "begin", new PastForms("began", "begun") },
				{ "bend", new PastForms("bended", "bent") },
				{ "beat", new PastForms("beat", "beaten") },
				{ "bind", new PastForms("bound") },
				{ "bite", new PastForms("bit", "bitten") },
				{ "cast", new PastForms("cast") }
			};
		}

		public override string ConjugateVerb(string root, Person person, Gender gender, Tense tense, Aspect aspect)
		{
			throw new NotImplementedException();
		}

		private sealed class PastForms
		{
			public PastForms(string simple, string pastParticiple)
			{
				Simple = simple;
				PastParticiple = pastParticiple;
			}

			public PastForms(string form)
			{
				Simple = form;
				PastParticiple = form;
			}

			public string Simple { get; }
			public string PastParticiple { get; }
		}
	}
}