using System;
using System.Collections.Generic;
using System.Linq;

using Rant.Engine;

namespace Rant.Vocabulary
{
    /// <summary>
    /// Maintains state information necessary for query synchronization (e.g. rhyming, uniqueness, matching).
    /// </summary>
    public sealed class QueryState
    {
        /// <summary>
        /// Distinct carrier table.
        /// Wherein the KEY is the UNIQUE ID, and the VALUE is the collection of used entries.
        /// </summary>
        private readonly Dictionary<string, HashSet<RantDictionaryEntry>> _uniqueTable = new Dictionary<string, HashSet<RantDictionaryEntry>>();

        /// <summary>
        /// Rhyme carrier table.
        /// Wherein the KEY is the RHYME ID and SUBTYPE, and the VALUE is the ROOT TERM and USED TERMS.
        /// </summary>
        private readonly Dictionary<string, _<RantDictionaryTerm, HashSet<RantDictionaryEntry>>> _rhymeTable = new Dictionary<string, _<RantDictionaryTerm, HashSet<RantDictionaryEntry>>>();

        /// <summary>
        /// Match table.
        /// </summary>
        private readonly Dictionary<string, RantDictionaryEntry> _matchTable = new Dictionary<string, RantDictionaryEntry>();

        /// <summary>
        /// Associative/Disassociative/Relational/Divergent carrier table.
        /// </summary>
        private readonly Dictionary<string, RantDictionaryEntry> _assocTable = new Dictionary<string, RantDictionaryEntry>();

        private readonly Rhymer _rhymer = new Rhymer();

        internal void DeleteUnique(string name) => _uniqueTable.Remove(name);

        internal void DeleteRhyme(string name) => _rhymeTable.Remove(name);

        internal void DeleteMatch(string name) => _matchTable.Remove(name);

        internal void DeleteAssociation(string name) => _assocTable.Remove(name);

        internal Rhymer Rhymer => _rhymer;

        internal RantDictionaryEntry GetEntry(Carrier carrier, int subtypeIndex, IEnumerable<RantDictionaryEntry> pool, RNG rng)
        {
            if (carrier == null) return pool.PickWeighted(rng, e => e.Weight);
            
            RantDictionaryEntry result = null;

            // Handle match carriers
            foreach(var match in carrier.GetCarriers(CarrierComponent.Match))
                if (_matchTable.TryGetValue(match, out result)) return result;

            // Handle associative carriers
            foreach(var assoc in carrier.GetCarriers(CarrierComponent.Associative))
            {
                if (_assocTable.TryGetValue(assoc, out result))
                    pool = pool.Where(e => e.AssociatesWith(result));
                break;
            }

            // Handle match-associative carriers
            foreach(var massoc in carrier.GetCarriers(CarrierComponent.MatchAssociative))
            {
                if (_matchTable.TryGetValue(massoc, out result))
                    pool = pool.Where(e => e.AssociatesWith(result));
                break;
            }

            // Handle unique carriers
            foreach(var unique in carrier.GetCarriers(CarrierComponent.Unique))
            {
                HashSet<RantDictionaryEntry> usedSet;
                if (!_uniqueTable.TryGetValue(unique, out usedSet))
                {
                    usedSet = _uniqueTable[unique] = new HashSet<RantDictionaryEntry>();
                }

                pool = pool.Except(usedSet);
            }

            // Handle match-unique carriers
            foreach (var munique in carrier.GetCarriers(CarrierComponent.Unique))
            {
                if (_matchTable.TryGetValue(munique, out result))
                    pool = pool.Where(e => e != result);
            }

            // Handle relational carriers
            foreach(var relate in carrier.GetCarriers(CarrierComponent.Relational))
            {
                if (_assocTable.TryGetValue(relate, out result))
                    pool = pool.Where(e => e.RelatesWith(result));
            }

            // Handle match-relational carriers
            foreach (var relate in carrier.GetCarriers(CarrierComponent.MatchRelational))
            {
                if (_matchTable.TryGetValue(relate, out result))
                    pool = pool.Where(e => e.RelatesWith(result));
            }

            // Handle dissociative carriers
            foreach (var relate in carrier.GetCarriers(CarrierComponent.Dissociative))
            {
                if (_assocTable.TryGetValue(relate, out result))
                    pool = pool.Where(e => !e.RelatesWith(result));
            }

            // Handle match-dissociative carriers
            foreach (var relate in carrier.GetCarriers(CarrierComponent.MatchDissociative))
            {
                if (_matchTable.TryGetValue(relate, out result))
                    pool = pool.Where(e => !e.RelatesWith(result));
            }

            // Handle divergent carriers
            foreach (var diverge in carrier.GetCarriers(CarrierComponent.Divergent))
            {
                if (_assocTable.TryGetValue(diverge, out result))
                    pool = pool.Where(e => e.DivergesFrom(result));
            }

            // Handle match-divergent carriers
            foreach (var diverge in carrier.GetCarriers(CarrierComponent.MatchDivergent))
            {
                if (_matchTable.TryGetValue(diverge, out result))
                    pool = pool.Where(e => e.DivergesFrom(result));
            }

            result = pool.PickWeighted(rng, e => e.Weight);

            // Handle rhyme carrier
            foreach(var rhyme in carrier.GetCarriers(CarrierComponent.Rhyme))
            {
                _<RantDictionaryTerm, HashSet<RantDictionaryEntry>> rhymeState;
                if (!_rhymeTable.TryGetValue(rhyme, out rhymeState))
                {
                    result = pool
                        .Where(e => !Util.IsNullOrWhiteSpace(e.Terms[subtypeIndex].Pronunciation))
                        .PickWeighted(rng, e => e.Weight);
                    _rhymeTable[rhyme] = _.Create(result.Terms[subtypeIndex], new HashSet<RantDictionaryEntry>(new[] { result }));
                    break;
                }
                result =
                    pool.Except(rhymeState.Item2)
                        .Where(e => !Util.IsNullOrWhiteSpace(e.Terms[subtypeIndex].Pronunciation))
                                .PickWeighted(rng, e => e.Weight * (_rhymer.Rhyme(rhymeState.Item1, e.Terms[subtypeIndex]) ? rhymeState.Item1.SyllableCount : 0));

                if (result != null) rhymeState.Item2.Add(result);
                break; // Ignore any extra rhyme carriers
            }

            if (result == null) return result;

            foreach (var a in carrier.GetCarriers(CarrierComponent.Associative))
                if (!_assocTable.ContainsKey(a)) _assocTable[a] = result;

            foreach (var a in carrier.GetCarriers(CarrierComponent.Dissociative))
                if (!_assocTable.ContainsKey(a)) _assocTable[a] = result;

            foreach (var a in carrier.GetCarriers(CarrierComponent.Divergent))
                if (!_assocTable.ContainsKey(a)) _assocTable[a] = result;

            foreach (var a in carrier.GetCarriers(CarrierComponent.Relational))
                if (!_assocTable.ContainsKey(a)) _assocTable[a] = result;

            foreach (var unique in carrier.GetCarriers(CarrierComponent.Unique))
                _uniqueTable[unique].Add(result);

            foreach (var match in carrier.GetCarriers(CarrierComponent.Match))
            {
                _matchTable[match] = result;
                break;
            }

            return result;
        }

		public void RemoveType(CarrierComponent type, string name)
		{
			if (type == CarrierComponent.Rhyme)
				DeleteRhyme(name);
			else if (type == CarrierComponent.Unique)
				DeleteUnique(name);
			else if (
				type == CarrierComponent.Associative ||
				type == CarrierComponent.Relational ||
				type == CarrierComponent.Dissociative ||
				type == CarrierComponent.Divergent)
				DeleteAssociation(name);
			else
				DeleteMatch(name);
		}
    }
}