using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Rant.Engine.Syntax;
using Rant.Stringes;

namespace Rant.Engine.Compiler.Parselets
{
    internal class LeftSquare : Parselet
    {
        public override R[] Identifiers
        {
            get
            {
                return new[] { R.LeftSquare };
            }
        }

        public LeftSquare()
        {
        }

        public override IEnumerator<Parselet> Parse(NewRantCompiler compiler, TokenReader reader, Token<R> fromToken)
        {
            var tagToken = reader.ReadToken();
            switch (tagToken.ID)
            {
                default:
                    compiler.SyntaxError(tagToken, "Expected function name, subroutine, regex or a script.");
                    break;

                case R.Text:
                    foreach (var parselet in Text(compiler, reader, tagToken)) // TODO: all this needs some serious cleanup
                        yield return parselet;
                    break;

                case R.Regex:
                    
                    break;

                case R.At:
                    break;

                case R.Dollar:
                    break;
            }
        }

        // TODO: all these "mini-parselets" are bad and DRY
        IEnumerable<Parselet> Regex(NewRantCompiler compiler, TokenReader reader, Token<R> token)
        {
            Regex regex = null;
            try
            {
                regex = Util.ParseRegex(token.Value);
            }
            catch (Exception e)
            {
                compiler.SyntaxError(token, e);
            }

            reader.ReadLoose(R.Colon);

            foreach (var parselet in ReplacerArgs(compiler, reader, token, regex))
                yield return parselet;
        }

        IEnumerable<Parselet> Text(NewRantCompiler compiler, TokenReader reader, Token<R> token)
        {
            var name = token.Value;
            var group = RantFunctions.GetFunctionGroup(name);

            if (group == null)
                compiler.SyntaxError(token, $"Unknown function: '{name}");

            if (reader.TakeLoose(R.Colon))
            {
                foreach (var parselet in FuncArgs(compiler, reader, token, group))
                    yield return parselet;
            }
            else
            {
                var end = reader.Read(R.RightSquare);
                AddToOutput(new RAFunction(Stringe.Range(Token, end), compiler.GetFunctionInfo(group, 0, Token, end), new List<RantAction>()));
            }
        }

        // TODO: all these "mini-compilers" are bad and DRY
        IEnumerable<Parselet> ReplacerArgs(NewRantCompiler compiler, TokenReader reader, Token<R> token, Regex regex)
        {
            Token<R> funcToken = null;
            var actions = new List<RantAction>();
            var sequences = new List<RantAction>();

            while (!reader.End)
            {
                funcToken = reader.ReadToken();

                if (funcToken.ID == R.Semicolon)
                {
                    // add action to args and continue
                    sequences.Add(actions.Count == 1 ? actions[0] : new RASequence(actions, funcToken));
                    actions.Clear();
                    reader.SkipSpace();
                    continue;
                }
                else if (funcToken.ID == R.RightSquare)
                {
                    // add action to args and return
                    sequences.Add(actions.Count == 1 ? actions[0] : new RASequence(actions, funcToken));

                    if (sequences.Count != 2)
                        compiler.SyntaxError(Stringe.Between(token, Token), "Replacer must have two arguments");

                    AddToOutput(new RAReplacer(Stringe.Range(Token, token), regex, sequences[0], sequences[1]));
                    yield break;
                }

                yield return Parselet.GetWithToken(compiler, funcToken, actions.Add);
            }

            compiler.SyntaxError(Token, "Unterminated function: unexpected end of file");
        }

        IEnumerable<Parselet> FuncArgs(NewRantCompiler compiler, TokenReader reader, Token<R> token, RantFunctionGroup group)
        { 
            Token<R> funcToken = null;
            var actions = new List<RantAction>();
            var sequences = new List<RantAction>();

            while (!reader.End)
            {
                funcToken = reader.ReadToken();

                if (funcToken.ID == R.Semicolon)
                {
                    // add action to args and continue
                    sequences.Add(actions.Count == 1 ? actions[0] : new RASequence(actions, funcToken));
                    actions.Clear();
                    reader.SkipSpace();
                    continue;
                }
                else if (funcToken.ID == R.RightSquare)
                {
                    // add action to args and return
                    sequences.Add(actions.Count == 1 ? actions[0] : new RASequence(actions, funcToken));
                    AddToOutput(new RAFunction(Stringe.Range(Token, token),
                        compiler.GetFunctionInfo(group, sequences.Count, Token, token), sequences));
                    yield break;
                }

                yield return Parselet.GetWithToken(compiler, funcToken, actions.Add);
            }

            compiler.SyntaxError(Token, "Unterminated function: unexpected end of file");
        }
    }
}
