using System;
using System.Collections.Generic;

using Rant.Core.Stringes;
using Rant.Localization;
using Rant.Vocabulary.Querying;

namespace Rant.Core.Compiler.Syntax
{
	internal class RstQuery : RST
	{
		private readonly Query _query;

		public RstQuery(Query query, Stringe location)
			: base(location)
		{
			_query = query;
		}

		public override IEnumerator<RST> Run(Sandbox sb)
		{
			if (sb.Engine.Dictionary == null)
			{
				sb.Print(Txtres.GetString("missing-table"));
				yield break;
			}
			// carrier erase query
			if (_query.Name == null)
			{
				foreach (CarrierComponent type in Enum.GetValues(typeof(CarrierComponent)))
					foreach (string name in _query.Carrier.GetCarriers(type))
						sb.CarrierState.RemoveType(type, name);
				yield break;
			}
			var result = sb.Engine.Dictionary.Query(sb.RNG, _query, sb.CarrierState);

			if (result == null)
			{
				sb.Print("[No Match]");
			}
			else
			{
				if (result.IsSplit)
				{
					if (result.ValueSplitIndex == 0) // Pushes complement to the left of the query result
					{
						yield return _query.Complement;
						sb.Print(' ');
						sb.Print(result.Value);
					}
					else if (result.ValueSplitIndex == result.Value.Length)
					{
						sb.Print(result.Value);
						sb.Print(' ');
						yield return _query.Complement;
					}
					else
					{
						sb.Print(result.Value.Substring(0, result.ValueSplitIndex));
						sb.Print(' ');
						yield return _query.Complement;
						sb.Print(' ');
						sb.Print(result.Value.Substring(result.ValueSplitIndex));
					}	
				}
				else
				{

					sb.Print(result.Value);
					if (_query.Complement != null)
					{
						sb.Print(' '); // TODO: Pull phrasal separator from format info
						yield return _query.Complement;
					}
				}
			}
		}
	}
}
