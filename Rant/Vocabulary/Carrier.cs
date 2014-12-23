namespace Rant.Vocabulary
{
    /// <summary>
    /// Represents information that can be used to synchronize query selections based on certain criteria.
    /// </summary>
    public sealed class Carrier
    {
        private string _match, _distinct, _assoc, _rhyme;

        public string Match => _match ?? "";
        public string Distinct => _distinct ?? "";
        public bool DistinctFromMatch { get; set; }
        public string Association => _assoc ?? "";
        public bool AssociateWithMatch { get; set; }
        public string Rhyme => _rhyme ?? "";

        /// <summary>
        /// Creates an empty carrier.
        /// </summary>
        public Carrier()
        {
        }

        /// <summary>
        /// Creates a carrier with the specified arguments.
        /// </summary>
        /// <param name="match">The match name.</param>
        /// <param name="distinct">The distinction name.</param>
        /// <param name="distinctFromMatch">Determines whether the distinction is made from a previously cached entry.</param>
        /// <param name="association">The association name.</param>
        /// <param name="associateWithMatch">Determines whether the association should be made with a previously cached entry.</param>
        /// <param name="rhyme">The rhyme name.</param>
        public Carrier(string match, string distinct, bool distinctFromMatch, string association, bool associateWithMatch, string rhyme)
        {
            _match = match;
            _distinct = distinct;
            DistinctFromMatch = distinctFromMatch;
            _assoc = association;
            AssociateWithMatch = associateWithMatch;
            _rhyme = rhyme;
        }
    }
}