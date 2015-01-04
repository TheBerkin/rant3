using System;
using System.Text.RegularExpressions;

using Rant.Formats;

namespace Rant.Engine.Formatters
{
    internal class OutputFormatter
    {
        private const RegexOptions FmtRegexOptions = RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase;

        private static readonly Regex RegCapsTitleWord = new Regex(@"\b(?<!['""])[a-z]", FmtRegexOptions);
        private static readonly Regex RegCapsFirst = new Regex(@"(?<![a-z].*?)[a-z]", FmtRegexOptions);
        private static readonly Regex RegCapsSentenceA = new Regex(@"(?<=[.?!])\s+\w", FmtRegexOptions); // Handles letters after ending capitalization
        private static readonly Regex RegCapsSentenceB = new Regex(@"[.?!](?!\w)$", FmtRegexOptions); // Handles end cases (no letter after last ending)

        public Case Case;
        public char LastChar => _lastChar;
                
        private char _lastChar;
        private bool _sentence;

        public OutputFormatter()
        {
            Case = Case.None;
            _lastChar = '\0';
            _sentence = true;
        }

        public OutputFormatter Clone() => new OutputFormatter() { Case = this.Case, _lastChar = this._lastChar, _sentence = this._sentence };

        public string Format(string input, RantFormat formatStyle, OutputFormatterOptions options = OutputFormatterOptions.None)
        {
            if (String.IsNullOrEmpty(input)) return input;

            // Check for special symbols
            switch (input.ToLowerInvariant())
            {
                case Symbols.EnDash:
                    return "\u2013";
                case Symbols.EmDash:
                    return "\u2014";
                case Symbols.Copyright:
                    return "\u00a9";
                case Symbols.RegisteredTM:
                    return "\u00ae";
                case Symbols.Trademark:
                    return "\u2122";
            }

            switch (Case)
            {
                case Case.Lower:
                    input = input.ToLower();
                    break;
                case Case.Upper:
                    input = input.ToUpper();
                    break;
                case Case.First:
                    input = RegCapsFirst.Replace(input, m => m.Value.ToUpper());
                    Case = Case.None;
                    break;
                case Case.Title:
                    if ((options.HasFlag(OutputFormatterOptions.IsArticle) || formatStyle.Excludes(input)) && Char.IsWhiteSpace(_lastChar)) break;

                    input = RegCapsTitleWord.Replace(input, m => (
                        _lastChar == '\0'
                        || Char.IsSeparator(_lastChar)
                        || Char.IsWhiteSpace(_lastChar))
                                                                 || Char.IsPunctuation(_lastChar)
                        ? m.Value.ToUpper() : m.Value);
                    break;
                case Case.Sentence:
                    if (_sentence) input = Regex.Replace(input, @"^.*?\w", m => {
                                                                                    if (!options.HasFlag(OutputFormatterOptions.NoUpdate)) _sentence = false;
                                                                                    return m.Value.ToUpper();
                    });
                    input = RegCapsSentenceA.Replace(input, m => m.Value.ToUpper());                    
                    break;
                case Case.Word:
                    input = RegCapsTitleWord.Replace(input, m => m.Value.ToUpper());
                    break;
            }
            if (RegCapsSentenceB.IsMatch(input) && !options.HasFlag(OutputFormatterOptions.NoUpdate)) _sentence = true;
            if (!options.HasFlag(OutputFormatterOptions.NoUpdate)) _lastChar = input[input.Length - 1];
            return input;
        }
    }

    [Flags]
    internal enum OutputFormatterOptions
    {
        None =          0x00,
        NoUpdate =      0x01,
        IsArticle =     0x02
    }
}