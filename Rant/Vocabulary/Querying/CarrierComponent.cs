#region License

// https://github.com/TheBerkin/Rant
// 
// Copyright (c) 2017 Nicholas Fleck
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in the
// Software without restriction, including without limitation the rights to use, copy,
// modify, merge, publish, distribute, sublicense, and/or sell copies of the Software,
// and to permit persons to whom the Software is furnished to do so, subject to the
// following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
// PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE
// OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

namespace Rant.Vocabulary.Querying
{
    /// <summary>
    /// Defines carrier types for queries.
    /// </summary>
    public enum CarrierComponentType : byte
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