using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rant.Stringes;
using Rant.Vocabulary;

namespace Rant.Engine.Syntax
{
	internal class RAQuery : RantAction
	{
		private RantQueryInfo _info;

		public RAQuery(RantQueryInfo info)
			: base(info.Name)
		{
			_info = info;
		}

		public override IEnumerator<RantAction> Run(Sandbox sb)
		{
			var query = new Query();
			query.Name = _info.Name.Value;
			query.Subtype = (_info.Subtype != null ? _info.Subtype.Value : null);
			query.Carrier = _info.Carrier;
			query.Exclusive = _info.IsExclusive;
			query.ClassFilters = _info.ClassFilters;
			query.RegexFilters = _info.RegexFilters;
			query.SyllablePredicate = _info.Range;
			var result = sb.Engine.Dictionary.Query(sb.RNG, query, sb.QueryState);
			sb.Print(result);
			yield break;
		}
	}
}
