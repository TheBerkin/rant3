namespace Rant.Vocabulary
{
    public sealed class Carrier
    {
        private CarrierSyncType _syncType;
        private CarrierSyncState _syncState;
        private string _syncId;
        private string _rhymeId;
        private int _syllablesMin, _syllablesMax;

        public CarrierSyncType SyncType
        {
            get { return _syncType; }
            set { _syncType = value; }
        }

        public CarrierSyncState SyncState
        {
            get { return _syncState; }
            set { _syncState = value; }
        }

        public string SyncId
        {
            get { return _syncId; }
            set { _syncId = value; }
        }

        public string RhymeId
        {
            get { return _rhymeId; }
            set { _rhymeId = value; }
        }

        public int SyllablesMin
        {
            get { return _syllablesMin; }
            set { _syllablesMin = value; }
        }

        public int SyllablesMax
        {
            get { return _syllablesMax; }
            set { _syllablesMax = value; }
        }

        public Carrier(CarrierSyncState syncState, CarrierSyncType syncType, string syncId, string rhymeId, int syllablesMin, int syllablesMax)
        {
            _syncState = syncState;
            _syncType = syncType;
            _syncId = syncId ?? "";
            _rhymeId = rhymeId ?? "";
            _syllablesMin = syllablesMin;
            _syllablesMax = syllablesMax;
        }
    }

    /// <summary>
    /// 
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
        Unique
    }
}