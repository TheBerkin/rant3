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
        /// Wherein the KEY is the UNIQUE ID, and the VALUE is the collection of used entries.
        /// </summary>
        private readonly Dictionary<string, HashSet<RantDictionaryEntry>> _uniqueTable = new Dictionary<string, HashSet<RantDictionaryEntry>>();

        /// <summary>
        /// Wherein the KEY is the RHYME ID and SUBTYPE, and the VALUE is the IPA CODE.
        /// </summary>
        private readonly Dictionary<Tuple<string, string>, string> _rhymeTable = new Dictionary<Tuple<string, string>, string>(); 

        public CarrierSyncState()
        {
            
        }

        internal RantDictionaryEntry GetUniqueEntry(string uniqueId, IEnumerable<RantDictionaryEntry> entryPool, RNG rng)
        {
            HashSet<RantDictionaryEntry> usedSet;
            if (!_uniqueTable.TryGetValue(uniqueId, out usedSet))
            {
                usedSet = _uniqueTable[uniqueId] = new HashSet<RantDictionaryEntry>();
            }
            var selection = entryPool.Except(usedSet).PickWeighted(rng, e => e.Weight);
            usedSet.Add(selection);
            return selection;
        }


    }
}