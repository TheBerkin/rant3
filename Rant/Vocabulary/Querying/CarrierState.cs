using System.Collections.Generic;
using System.Linq;

using Rant.Core.Utilities;
using Rant.Vocabulary.Utilities;

namespace Rant.Vocabulary.Querying
{
	/// <summary>
	/// Maintains state information used by queries.
	/// </summary>
	internal sealed class CarrierState
	{
		/// <summary>
		/// Associative/Disassociative/Relational/Divergent carrier table.
		/// </summary>
		private readonly Dictionary<string, RantDictionaryEntry> _assocTable = new Dictionary<string, RantDictionaryEntry>();

		/// <summary>
		/// Match table.
		/// </summary>
		private readonly Dictionary<string, RantDictionaryEntry> _matchTable = new Dictionary<string, RantDictionaryEntry>();

		/// <summary>
		/// Rhyme carrier table.
		/// Wherein the KEY is the RHYME ID and SUBTYPE, and the VALUE is the ROOT TERM and USED TERMS.
		/// </summary>
		private readonly Dictionary<string, _<RantDictionaryTerm, HashSet<RantDictionaryEntry>>> _rhymeTable =
			new Dictionary<string, _<RantDictionaryTerm, HashSet<RantDictionaryEntry>>>();

		/// <summary>
		/// Distinct carrier table.
		/// Wherein the KEY is the UNIQUE ID, and the VALUE is the collection of used entries.
		/// </summary>
		private readonly Dictionary<string, HashSet<RantDictionaryEntry>> _uniqueTable =
			new Dictionary<string, HashSet<RantDictionaryEntry>>();

		internal Rhymer Rhymer { get; } = new Rhymer();
		internal void DeleteUnique(string name) => _uniqueTable.Remove(name);
		internal void DeleteRhyme(string name) => _rhymeTable.Remove(name);
		internal void DeleteMatch(string name) => _matchTable.Remove(name);
		internal void DeleteAssociation(string name) => _assocTable.Remove(name);

		public void Reset()
		{
			_rhymeTable.Clear();
			_matchTable.Clear();
			_assocTable.Clear();
			_uniqueTable.Clear();
		}

		internal RantDictionaryEntry GetEntry(Carrier carrier, int subtypeIndex, IEnumerable<RantDictionaryEntry> pool,
			RNG rng)
		{
			if (carrier == null) return pool.PickEntry(rng);

			RantDictionaryEntry result = null;

			// Handle match carriers
			foreach (string match in carrier.GetComponentsOfType(CarrierComponentType.Match))
				if (_matchTable.TryGetValue(match, out result)) return result;

			// Handle associative carriers
			foreach (string assoc in carrier.GetComponentsOfType(CarrierComponentType.Associative))
			{
				if (_assocTable.TryGetValue(assoc, out result))
					pool = pool.Where(e => e.AssociatesWith(result));
				break;
			}

			// Handle match-associative carriers
			foreach (string massoc in carrier.GetComponentsOfType(CarrierComponentType.MatchAssociative))
			{
				if (_matchTable.TryGetValue(massoc, out result))
					pool = pool.Where(e => e.AssociatesWith(result));
				break;
			}

			// Handle unique carriers
			foreach (string unique in carrier.GetComponentsOfType(CarrierComponentType.Unique))
			{
				HashSet<RantDictionaryEntry> usedSet;
				if (!_uniqueTable.TryGetValue(unique, out usedSet))
				{
					usedSet = _uniqueTable[unique] = new HashSet<RantDictionaryEntry>();
				}

				pool = pool.Except(usedSet);
			}

			// Handle match-unique carriers
			foreach (string munique in carrier.GetComponentsOfType(CarrierComponentType.Unique))
			{
				if (_matchTable.TryGetValue(munique, out result))
					pool = pool.Where(e => e != result);
			}

			// Handle relational carriers
			foreach (string relate in carrier.GetComponentsOfType(CarrierComponentType.Relational))
			{
				if (_assocTable.TryGetValue(relate, out result))
					pool = pool.Where(e => e.RelatesWith(result));
			}

			// Handle match-relational carriers
			foreach (string relate in carrier.GetComponentsOfType(CarrierComponentType.MatchRelational))
			{
				if (_matchTable.TryGetValue(relate, out result))
					pool = pool.Where(e => e.RelatesWith(result));
			}

			// Handle dissociative carriers
			foreach (string relate in carrier.GetComponentsOfType(CarrierComponentType.Dissociative))
			{
				if (_assocTable.TryGetValue(relate, out result))
					pool = pool.Where(e => !e.RelatesWith(result));
			}

			// Handle match-dissociative carriers
			foreach (string relate in carrier.GetComponentsOfType(CarrierComponentType.MatchDissociative))
			{
				if (_matchTable.TryGetValue(relate, out result))
					pool = pool.Where(e => !e.RelatesWith(result));
			}

			// Handle divergent carriers
			foreach (string diverge in carrier.GetComponentsOfType(CarrierComponentType.Divergent))
			{
				if (_assocTable.TryGetValue(diverge, out result))
					pool = pool.Where(e => e.DivergesFrom(result));
			}

			// Handle match-divergent carriers
			foreach (string diverge in carrier.GetComponentsOfType(CarrierComponentType.MatchDivergent))
			{
				if (_matchTable.TryGetValue(diverge, out result))
					pool = pool.Where(e => e.DivergesFrom(result));
			}

			result = pool.PickEntry(rng);

			// Handle rhyme carrier
			foreach (string rhyme in carrier.GetComponentsOfType(CarrierComponentType.Rhyme))
			{
				_<RantDictionaryTerm, HashSet<RantDictionaryEntry>> rhymeState;
				if (!_rhymeTable.TryGetValue(rhyme, out rhymeState))
				{
					result = pool
						.Where(e => !Util.IsNullOrWhiteSpace(e[subtypeIndex].Pronunciation))
						.PickEntry(rng);
					if (result == null) return null;
					_rhymeTable[rhyme] = _.Create(result[subtypeIndex], new HashSet<RantDictionaryEntry>(new[] { result }));
					break;
				}
				result =
					pool.Except(rhymeState.Item2)
						.Where(e =>
							!Util.IsNullOrWhiteSpace(e[subtypeIndex].Pronunciation) &&
							Rhymer.Rhyme(rhymeState.Item1, e[subtypeIndex]))
						.PickEntry(rng);

				if (result != null) rhymeState.Item2.Add(result);
				break; // Ignore any extra rhyme carriers
			}

			if (result == null) return null;

			foreach (string a in carrier.GetComponentsOfType(CarrierComponentType.Associative))
				if (!_assocTable.ContainsKey(a)) _assocTable[a] = result;

			foreach (string a in carrier.GetComponentsOfType(CarrierComponentType.Dissociative))
				if (!_assocTable.ContainsKey(a)) _assocTable[a] = result;

			foreach (string a in carrier.GetComponentsOfType(CarrierComponentType.Divergent))
				if (!_assocTable.ContainsKey(a)) _assocTable[a] = result;

			foreach (string a in carrier.GetComponentsOfType(CarrierComponentType.Relational))
				if (!_assocTable.ContainsKey(a)) _assocTable[a] = result;

			foreach (string unique in carrier.GetComponentsOfType(CarrierComponentType.Unique))
				_uniqueTable[unique].Add(result);

			foreach (string match in carrier.GetComponentsOfType(CarrierComponentType.Match))
			{
				_matchTable[match] = result;
				break;
			}

			return result;
		}

		public void RemoveType(CarrierComponentType type, string name)
		{
			if (type == CarrierComponentType.Rhyme)
				DeleteRhyme(name);
			else if (type == CarrierComponentType.Unique)
				DeleteUnique(name);
			else if (
				type == CarrierComponentType.Associative ||
				type == CarrierComponentType.Relational ||
				type == CarrierComponentType.Dissociative ||
				type == CarrierComponentType.Divergent)
				DeleteAssociation(name);
			else
				DeleteMatch(name);
		}
	}
}