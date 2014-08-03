using System;
using System.Text.RegularExpressions;

namespace Manhood
{
    /// <summary>
    /// Stores preprocessed instances of code for interpreter use.
    /// </summary>
    public sealed class Pattern
    {
        private const string PatComments = @"(^|[^\\])``(?:.|[\r\n])*?[^\\]``";
        private const string PatTrim = @"([\s\t]+$|^[\s\t]+)";

        private static readonly Regex RegComments = new Regex(PatComments);
        private static readonly Regex RegTrim = new Regex(PatTrim, RegexOptions.Multiline);

        private readonly string _code;

        /// <summary>
        /// Creates a new pattern from the specified code.
        /// </summary>
        /// <param name="code">The code to process.</param>
        public Pattern(string code)
        {
            _code = RegTrim.Replace(RegComments.Replace(code, ""), "");
        }

        internal string Code
        {
            get { return _code; }
        }
    }
}