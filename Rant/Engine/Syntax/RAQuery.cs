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
				null, 
				_info.IsExclusive, 
				_info.ClassFilters, 
				null, 
				null
			);
			var state = new QueryState();
			var result = sb.Engine.Dictionary.Query(sb.RNG, query, state);
			sb.Print(result);
			yield break;
		}
	}
}
