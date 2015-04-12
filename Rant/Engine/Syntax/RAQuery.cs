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
		private Query _query;

		public RAQuery(Query query)
			: base(query.OriginStringe)
		{
			_query = query;
		}

		public override IEnumerator<RantAction> Run(Sandbox sb)
		{
			if (sb.Engine.Dictionary == null)
			{
				sb.Print("[Missing Table]");
				yield break;
			}
			// carrier erase query
			if (_query.Name == null)
			{
				foreach (CarrierComponent type in Enum.GetValues(typeof(CarrierComponent)))
					foreach (string name in _query.Carrier.GetCarriers(type))
						sb.QueryState.RemoveType(type, name);
				yield break;
			}
			var result = sb.Engine.Dictionary.Query(sb.RNG, _query, sb.QueryState);
			sb.Print(result);
			yield break;
		}
	}
}
