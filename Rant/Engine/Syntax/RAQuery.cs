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
			var query = new Query(
				_info.Name.Value, 
				(_info.Subtype != null ? _info.Subtype.Value : null),
				_info.Carrier, 
				_info.IsExclusive, 
				_info.ClassFilters, 
				_info.RegexFilters, 
				null
			);
			var result = sb.Engine.Dictionary.Query(sb.RNG, query, sb.QueryState);
			sb.Print(result);
			yield break;
		}
	}
}
