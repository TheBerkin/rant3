using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using Rant.Engine.Compiler;
using Rant.Engine.Stringes.Tokens;

namespace Rant.Engine.Blueprints
{
    /// <summary>
    /// To be used on the replacer tag state as a pre-blueprint.
    /// </summary>
    internal class ReplacerBlueprint : Blueprint
    {
        private readonly Regex _regex;
        private readonly IEnumerable<Token<R>> _evaluator;
        private MatchCollection _matches;
        private string _input;
        private bool _inputCollected;

        public ReplacerBlueprint(VM interpreter, Regex regex, IEnumerable<Token<R>> evaluator) : base(interpreter)
        {
            _evaluator = evaluator;
            _regex = regex;
            _inputCollected = false;
            _input = null;
            _matches = null;
        }

        public override bool Use()
        {
            if (!_inputCollected)
            {
                // Get the replacer's input string
                _input = I.PopResultString();
                _inputCollected = true;

                // Queue this blueprint for after the matches are evaluated
                I.CurrentState.Pre(this);

                // Get the matches for the pattern in the input
                _matches = _regex.Matches(_input);
                
                // Queue a copy of the match evaluator for each match
                foreach (Match match in _matches)
                {
                    if (!match.Success) continue;
                    var state = VM.State.CreateSub(I.CurrentState.Reader.Source, _evaluator, I);

                    Match match1 = match;

                    state.Pre(new DelegateBlueprint(I, _ =>
                    {
                        _.PushMatch(match1);
                        return false;
                    }));

                    state.Post(new DelegateBlueprint(I, _ =>
                    {
                        _.PopMatch();
                        return false;
                    }));

                    I.PushState(state);
                }

                return true;
            }
            else
            {
                int start = 0;
                var sb = new StringBuilder();

                // Replace stuff
                foreach (Match match in _matches)
                {
                    sb.Append(_input.Substring(start, match.Index - start));
                    sb.Append(I.PopResultString());
                    start = match.Index + match.Length;
                }

                // Toss on any leftover string
                sb.Append(_input.Substring(start, _input.Length - start));

                // Print the result
                I.Print(sb.ToString());
            }

            return false;
        }
    }
}