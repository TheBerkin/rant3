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

using System.Collections.Generic;

using Rant.Core.Compiler.Syntax;
using Rant.Core;
using Rant.Localization;
using System;

namespace Rant.Vocabulary.Querying
{
    /// <summary>
    /// Represents a set of search criteria for a Rant dictionary.
    /// </summary>
    internal sealed class Query
    {
        private readonly HashSet<Filter> _filters = new HashSet<Filter>();

        /// <summary>
        /// The carrier for the query.
        /// </summary>
        public Carrier Carrier { get; set; }

        /// <summary>
        /// The name of the table to search.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The subtype of the dictionary term to use.
        /// </summary>
        public string Subtype { get; set; }

        /// <summary>
        /// The plural subtype to use.
        /// </summary>
        public string PluralSubtype { get; set; }

        /// <summary>
        /// Specifies exclusivity of the class filter.
        /// </summary>
        public bool Exclusive { get; set; }

        /// <summary>
        /// Complement for phrasal verbs. Not yet available in public API.
        /// </summary>
        internal RST Complement { get; set; }

        public int FilterCount => _filters.Count;

        /// <summary>
        /// Returns whether the query is a "bare query" - should only return the table itself.
        /// </summary>
        public bool BareQuery => _filters.Count == 0 && !Exclusive && !HasCarrier;

        /// <summary>
        /// Returns whether the query has a carrier.
        /// </summary>
        public bool HasCarrier => Carrier != null && Carrier.GetTotalCount() != 0;

        public void AddFilter(Filter filter) => _filters.Add(filter);

        public IEnumerable<Filter> GetFilters()
        {
            foreach (var filter in _filters) yield return filter;
        }

		internal IEnumerator<RST> Run(Sandbox sb)
		{
			if (Name == null) yield break;
			if (sb.Engine.Dictionary == null)
			{
				sb.Print(Txtres.GetString("missing-table"));
				yield break;
			}

			var result = sb.Engine.Dictionary.Query(sb, this, sb.CarrierState);

			if (result == null)
			{
				sb.Print("[No Match]");
			}
			else
			{
				if (result.IsSplit)
				{
					if (this.Complement == null)
					{
						sb.Print(result.LeftSide);
						sb.Print(sb.Format.WritingSystem.Space);
						sb.Print(result.RightSide);
					}
					else if (result.ValueSplitIndex == 0) // Pushes complement to the left of the query result
					{
						long l = sb.SizeLimit.Value;
						yield return this.Complement;
						if (sb.SizeLimit.Value > l) sb.Print(sb.Format.WritingSystem.Space);
						sb.Print(result.Value);
					}
					else if (result.ValueSplitIndex == result.Value.Length)
					{
						sb.Print(result.Value);
						long l = sb.SizeLimit.Value;
						var t = sb.Output.InsertAnonymousTarget();
						yield return this.Complement;
						if (sb.SizeLimit.Value > l) sb.Output.PrintToTarget(t, sb.Format.WritingSystem.Space);
					}
					else // Complement goes inside phrase
					{
						sb.Print(result.LeftSide);
						sb.Print(sb.Format.WritingSystem.Space);
						long l = sb.SizeLimit.Value;
						yield return this.Complement;
						if (sb.SizeLimit.Value > l) sb.Print(sb.Format.WritingSystem.Space);
						sb.Print(result.RightSide);
					}
				}
				else
				{
					sb.Print(result.Value);
					if (this.Complement != null)
					{
						long l = sb.SizeLimit.Value;
						var t = sb.Output.InsertAnonymousTarget();
						yield return this.Complement;
						if (sb.SizeLimit.Value > l) sb.Output.PrintToTarget(t, sb.Format.WritingSystem.Space);
					}
				}
			}
		}
    }
}