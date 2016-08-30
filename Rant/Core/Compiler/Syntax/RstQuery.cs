using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Rant.Core.IO;
using Rant.Localization;
using Rant.Vocabulary.Querying;

namespace Rant.Core.Compiler.Syntax
{
	[RST("quer")]
	internal class RstQuery : RST
	{
		private Query _query;

		public RstQuery(Query query, LineCol location)
			: base(location)
		{
			_query = query;
		}

		public RstQuery(LineCol location) : base(location)
		{
			// Used by serializer
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
				foreach (CarrierComponentType type in Enum.GetValues(typeof(CarrierComponentType)))
					foreach (string name in _query.Carrier.GetComponentsOfType(type))
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

		protected override IEnumerator<RST> Serialize(EasyWriter output)
		{
			output.Write(_query.Name);
			output.Write(_query.Subtype);
			output.Write(_query.Exclusive);

			// Syllable predicate
			output.Write(_query.SyllablePredicate != null);
			if (_query.SyllablePredicate != null)
			{
				output.Write(_query.SyllablePredicate.Minimum != null);
				if (_query.SyllablePredicate.Minimum != null)
					output.Write(_query.SyllablePredicate.Minimum.Value);
				output.Write(_query.SyllablePredicate.Maximum != null);
				if (_query.SyllablePredicate.Maximum != null)
					output.Write(_query.SyllablePredicate.Maximum.Value);
			}

			// Regex filters
			output.Write(_query.RegexFilters.Count);
			foreach (var regexFilter in _query.RegexFilters)
			{
				output.Write(regexFilter.Item1);
				output.Write((regexFilter.Item2.Options & RegexOptions.IgnoreCase) != 0);
				output.Write(regexFilter.Item2.ToString());
			}

			// Class filter
			if (_query.ClassFilter != null)
			{
				output.Write(true);
				_query.ClassFilter.Serialize(output);
			}
			else
			{
				output.Write(false);
			}

			// Carrier
			if (_query.Carrier != null)
			{
				output.Write(true);
				_query.Carrier.Serialize(output);
			}
			else
			{
				output.Write(false);
			}

			// Complement
			yield return _query.Complement;
		}

		protected override IEnumerator<DeserializeRequest> Deserialize(EasyReader input)
		{
			_query = new Query();

			_query.Name = input.ReadString();
			_query.Subtype = input.ReadString();
			_query.Exclusive = input.ReadBoolean();

			// Syllable predicate
			if (input.ReadBoolean())
			{
				int? min = null;
				int? max = null;
				if (input.ReadBoolean()) min = input.ReadInt32();
				if (input.ReadBoolean()) max = input.ReadInt32();
				_query.SyllablePredicate = new Range(min, max);
			}

			// Regex filters
			int regexFilterCount = input.ReadInt32();
			if (regexFilterCount > 0)
			{
				_query.RegexFilters = new List<_<bool, Regex>>(regexFilterCount);

				for (int i = 0; i < regexFilterCount; i++)
				{
					var options = RegexOptions.Compiled | RegexOptions.ExplicitCapture;
					bool match = input.ReadBoolean();
					if (input.ReadBoolean()) options |= RegexOptions.IgnoreCase;
					_query.RegexFilters.Add(new _<bool, Regex>(match, new Regex(input.ReadString(), options)));
				}
			}

			// Class filter
			if (input.ReadBoolean())
			{
				_query.ClassFilter = new ClassFilter();
				_query.ClassFilter.Deserialize(input);
			}

			// Carrier
			if (input.ReadBoolean())
			{
				_query.Carrier = new Carrier();
				_query.Carrier.Deserialize(input);
			}

			var complementRequest = new DeserializeRequest();
			yield return complementRequest;
			_query.Complement = complementRequest.Result;
		}
	}
}