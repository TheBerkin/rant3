using System;
using System.Collections.Generic;
using System.Linq;

namespace Rant.Vocabulary
{
    /// <summary>
    /// Maintains state information necessary for query synchronization (e.g. rhyming, uniqueness, matching).
    /// </summary>
    public sealed class CarrierSyncState
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
        private readonly Dictionary<string, Tuple<RantDictionaryTerm, HashSet<RantDictionaryEntry>>> _rhymeTable = new Dictionary<string, Tuple<RantDictionaryTerm, HashSet<RantDictionaryEntry>>>();

        /// <summary>
        /// Match table.
        /// </summary>
        private readonly Dictionary<string, RantDictionaryEntry> _matchTable = new Dictionary<string, RantDictionaryEntry>();

        /// <summary>
        /// Associative carrier table.
        /// </summary>
        private readonly Dictionary<string, RantDictionaryEntry> _assocTable = new Dictionary<string, RantDictionaryEntry>();

        internal RantDictionaryEntry GetEntry(Carrier carrier, int subtypeIndex, IEnumerable<RantDictionaryEntry> pool, RNG rng)
        {
            if (carrier == null) return pool.PickWeighted(rng, e => e.Weight);

            bool bMatch = !String.IsNullOrEmpty(carrier.Match);
            bool bDistinct = !String.IsNullOrEmpty(carrier.Distinct);
            bool bAssociate = !String.IsNullOrEmpty(carrier.Association);
            bool bRhyme = !String.IsNullOrEmpty(carrier.Rhyme);

            RantDictionaryEntry result = null;

            if (bMatch && _matchTable.TryGetValue(carrier.Match, out result))
            {
                return result;
            }

            if (bAssociate && 
                ((carrier.AssociateWithMatch && _matchTable.TryGetValue(carrier.Association, out result))
                    || _assocTable.TryGetValue(carrier.Association, out result)))
            {
                bool resultHasClasses = result.Classes.Any();
                pool = pool.Where(e => e.Classes.Any() == resultHasClasses && !e.Classes.Except(result.Classes).Any());
            }

            if (bDistinct)
            {
                HashSet<RantDictionaryEntry> usedSet;
                if (!_uniqueTable.TryGetValue(carrier.Distinct, out usedSet))
                {
                    usedSet = _uniqueTable[carrier.Distinct] = new HashSet<RantDictionaryEntry>();
                }

                pool = pool.Except(usedSet);
            }

            result = pool.PickWeighted(rng, e => e.Weight);

            if (bRhyme)
            {
                Tuple<RantDictionaryTerm, HashSet<RantDictionaryEntry>> rhymeState;
                if (!_rhymeTable.TryGetValue(carrier.Rhyme, out rhymeState))
                {
                    result = pool
                        .Where(e => !String.IsNullOrWhiteSpace(e.Terms[subtypeIndex].Pronunciation))
                        .PickWeighted(rng, e => e.Weight);
                    _rhymeTable[carrier.Rhyme] = Tuple.Create(result.Terms[subtypeIndex], new HashSet<RantDictionaryEntry>(new[] { result }));
                }
                result =
                    pool.Except(rhymeState.Item2)
                        .Where(e => !String.IsNullOrWhiteSpace(e.Terms[subtypeIndex].Pronunciation))
                                .PickWeighted(rng, e => e.Weight * VocabUtils.RhymeIndex(rhymeState.Item1, e.Terms[subtypeIndex]));

                if (result != null) rhymeState.Item2.Add(result);
            }

            if (bAssociate)
            {
                _assocTable[carrier.Association] = result;
            }

            if (bDistinct)
            {
                _uniqueTable[carrier.Distinct].Add(result);
            }

            if (bMatch)
            {
                _matchTable[carrier.Match] = result;
            }

            return result;
        }
    }
}