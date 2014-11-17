namespace Rant.Vocabulary
{
    /// <summary>
    /// Represents information that can be used to synchronize query selections based on certain criteria.
    /// </summary>
    public sealed class Carrier
    {
        /// <summary>
        /// The type of synchronization to perform with the carrier.
        /// </summary>
        public CarrierSyncType SyncType { get; set; }

        /// <summary>
        /// The ID assigned to the carrier.
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// (Unused) The minimum number of syllables the selected term should have.
        /// </summary>
        public int SyllablesMin { get; set; }

        /// <summary>
        /// (Unused) The maximum number of syllables the selected term should have.
        /// </summary>
        public int SyllablesMax { get; set; }

        /// <summary>
        /// Creates a new Carrier instance with the specified parameters.
        /// </summary>
        /// <param name="syncType">The type of synchronization to perform with the carrier.</param>
        /// <param name="id">The ID assigned to the carrier.</param>
        /// <param name="syllablesMin">The minimum number of syllables the selected term should have.</param>
        /// <param name="syllablesMax">The maximum number of syllables the selected term should have.</param>
        public Carrier(CarrierSyncType syncType, string id, int syllablesMin, int syllablesMax)
        {
            SyncType = syncType;
            ID = id ?? "";
            SyllablesMin = syllablesMin;
            SyllablesMax = syllablesMax;
        }
    }

    /// <summary>
    /// Defines synchronization types for query carriers.
    /// </summary>
    public enum CarrierSyncType
    {
        /// <summary>
        /// Perform no synchronization.
        /// </summary>
        None,
        /// <summary>
        /// Match with other carriers using the same ID.
        /// </summary>
        Match,
        /// <summary>
        /// Be unique from other carriers using the same ID.
        /// </summary>
        Unique,
        /// <summary>
        /// Rhyme with previous carrier selections using the same ID.
        /// </summary>
        Rhyme
    }
}