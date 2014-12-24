namespace Rant.Vocabulary
{
    /// <summary>
    /// Defines carrier types for queries.
    /// </summary>
    public enum CarrierComponent
    {
        /// <summary>
        /// Select the same entry every time.
        /// </summary>
        Match,
        /// <summary>
        /// Share no classes.
        /// </summary>
        Dissociative,
        /// <summary>
        /// Share no classes with a match carrier entry.
        /// </summary>
        MatchDissociative,
        /// <summary>
        /// Classes must exactly match.
        /// </summary>
        Associative,
        /// <summary>
        /// Classes must exactly match those of a match carrier entry.
        /// </summary>
        MatchAssociative,
        /// <summary>
        /// Have at least one different class.
        /// </summary>
        Divergent,
        /// <summary>
        /// Have at least one different class than a match carrier entry.
        /// </summary>
        MatchDivergent,
        /// <summary>
        /// Share at least one class.
        /// </summary>
        Relational,
        /// <summary>
        /// Share at least one class with a match carrier entry.
        /// </summary>
        MatchRelational,
        /// <summary>
        /// Never choose the same entry twice.
        /// </summary>
        Unique,
        /// <summary>
        /// Choose an entry that is different from a match carrier entry.
        /// </summary>
        MatchUnique,
        /// <summary>
        /// Choose terms that rhyme.
        /// </summary>
        Rhyme
    }
}
