using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rant;

namespace Rant.Benchmark.Tests
{
	public class Query
	{
		[Test(Name = "Bare Query (10k)")]
		public string BareQuery(RantEngine rant)
		{
			return rant.Do(@"[rs:10k;\s]{<noun>}").Main;
		}

		[Test(Name = "Query With Class Filter (10k)")]
		public string QueryOneClass(RantEngine rant)
		{
			return rant.Do(@"[rs:10k;\s]{<noun-body>}").Main;
		}

		[Test(Name = "Query With Multiple Class Filters (10k)")]
		public string QueryMultiClass(RantEngine rant)
		{
			return rant.Do(@"[rs:10k;\s]{<noun-body-nsfw>}").Main;
		}

		[Test(Name = "Query With Syllable Count Filter (Min) (10k)")]
		public string QuerySyllableMin(RantEngine rant)
		{
			return rant.Do(@"[rs:10k;\s]{<noun(2-)>}");
		}

		[Test(Name = "Query With Syllable Count Filter (Exact) (10k)")]
		public string QuerySyllableExact(RantEngine rant)
		{
			return rant.Do(@"[rs:10k;\s]{<noun(2)>}");
		}

		[Test(Name = "Query With Syllable Count Filter (Max) (10k)")]
		public string QuerySyllableMax(RantEngine rant)
		{
			return rant.Do(@"[rs:10k;\s]{<noun(-4)>}");
		}

		[Test(Name = "Query With Syllable Count Filter (Range) (10k)")]
		public string QuerySyllableRange(RantEngine rant)
		{
			return rant.Do(@"[rs:10k;\s]{<noun(2-4)>}");
		}

		[Test(Name = "Query With Syllable Count And Class Filter (10k)")]
		public string QuerySyllableClassRange(RantEngine rant)
		{
			return rant.Do(@"[rs:10k;\s]{<noun-body(2-4)>}");
		}

		[Test(Name = "Negative Class Filter (10k)")]
		public string NegativeClassFilter(RantEngine rant)
		{
			return rant.Do(@"[rs:10k;\s]{<noun-!body>}").Main;
		}
	}
}
