namespace Rant
{
    /// <summary>
    /// Represents a collection of named, queryable dictionaries.
    /// </summary>
    public interface IVocabulary
    {
        /// <summary>
        /// Queries the vocabulary according to the specified criteria and returns a random match.
        /// </summary>
        /// <param name="rng">The random number generator to randomize the match with.</param>
        /// <param name="query">The search criteria to use.</param>
        /// <returns></returns>
        string Query(RNG rng, Query query);
    }
}