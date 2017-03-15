using Rant.Vocabulary.Querying;
using System.Collections.Generic;

namespace Rant.Core.Constructs
{
	internal sealed class QueryBuilder
	{
		private readonly Dictionary<string, Query> _queries;
		private Query _activeQuery;

		public QueryBuilder()
		{
			_queries = new Dictionary<string, Query>();
		}

		public Query CurrentQuery => _activeQuery;

		public Query GetQuery(string name)
		{
			if (!_queries.TryGetValue(name, out Query q))
			{
				q = _queries[name] = new Query();
			}
			return _activeQuery = q;
		}
	}
}
