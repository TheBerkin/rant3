namespace Manhood
{
    /// <summary>
    /// The delegate used by Manhood to request custom string values via the [string] tag.
    /// </summary>
    /// <param name="rng">The random number generator in use by the interpreter.</param>
    /// <param name="requestName">A string describing the value to be retrieved.</param>
    /// <returns></returns>
    public delegate string StringRequestCallback(RNG rng, string requestName);
}