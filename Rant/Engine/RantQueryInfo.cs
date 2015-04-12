using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rant.Stringes;
using Rant.Vocabulary;
using System.Text.RegularExpressions;

namespace Rant.Engine
{
	internal class RantQueryInfo
	{
		private Stringe _name;

		public Stringe Name => _name;
		public Stringe Subtype = null;
		public bool IsExclusive = false;
		public List<_<bool, string>[]> ClassFilters;
		public List<_<bool, Regex>> RegexFilters;
		public Carrier Carrier;

		public RantQueryInfo(Stringe name)
		{
			_name = name;
			ClassFilters = new List<_<bool, string>[]>();
			RegexFilters = new List<_<bool, Regex>>();
			Carrier = new Carrier();
		}
	}
}
