using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

using Rant.Compiler;
using Rant.Stringes.Tokens;

namespace Rant
{
    internal class PatternReader
    {
        private readonly RantPattern _source;
        private readonly Token<R>[] _tokens;
        private int _pos;

        private readonly Stack<Token<R>> _stack = new Stack<Token<R>>(10); 

        public PatternReader(RantPattern source)
        {
            _source = source;
            _tokens = source.Tokens.ToArray();
            _pos = 0;
        }

        public RantPattern Source
        {
            get { return _source; }
        }

        public int Position
        {
            get { return _pos; }
            set { _pos = value; }
        }

        public bool End
        {
            get { return _pos >= _tokens.Length; }
        }

        public Token<R> PrevToken
        {
            get { return _pos == 0 ? null : _tokens[_pos - 1]; }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Token<R> ReadToken()
        {
            if (End) throw new RantException(_source, null, "Unexpected end of file.");
            return _tokens[_pos++];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Token<R> PeekToken()
        {
            if (End) throw new RantException(_source, null, "Unexpected end of file.");
            return _tokens[_pos];
        }

        public bool IsNext(R type)
        {
            return !End && _tokens[_pos].Identifier == type;
        }

        public bool Take(R type, bool allowEof = true)
        {
            if (End)
            {
                if (!allowEof)
                    throw new RantException(_source, null, "Unexpected end-of-file.");
                return false;
            }
            if (_tokens[_pos].Identifier != type) return false;
            _pos++;
            return true;
        }

        public bool TakeLoose(R type, bool allowEof = true)
        {
            if (End)
            {
                if (!allowEof)
                    throw new RantException(_source, null, "Unexpected end-of-file.");
                return false;
            }
            SkipSpace();
            if (_tokens[_pos].Identifier != type) return false;
            _pos++;
            SkipSpace();
            return true;
        }

        public bool TakeAny(params R[] types)
        {
            if (End) return false;
            foreach (var type in types)
            {
                if (_tokens[_pos].Identifier != type) continue;
                _pos++;
                return true;
            }
            return false;
        }

        public bool TakeAny(out R result, params R[] types)
        {
            result = default(R);
            if (End) return false;
            foreach (var type in types)
            {
                if (_tokens[_pos].Identifier != type) continue;
                result = type;
                _pos++;
                return true;
            }
            return false;
        }

        public bool TakeAnyLoose(params R[] types)
        {
            if (End) return false;
            SkipSpace();
            foreach (var type in types)
            {
                if (_tokens[_pos].Identifier != type) continue;
                _pos++;
                SkipSpace();
                return true;
            }
            return false;
        }

        public bool TakeAnyLoose(out R result, params R[] types)
        {
            result = default(R);
            if (End) return false;
            SkipSpace();
            foreach (var type in types)
            {
                if (_tokens[_pos].Identifier != type) continue;
                result = type;
                _pos++;
                SkipSpace();
                return true;
            }
            return false;
        }

        public bool TakeAll(R type, bool allowEof = true)
        {
            if (End)
            {
                if (!allowEof)
                    throw new RantException(_source, null, "Unexpected end-of-file.");
                return false;
            }
            if (_tokens[_pos].Identifier != type) return false;
            do
            {
                _pos++;
            } while (!End && _tokens[_pos].Identifier == type);
            return true;
        }

        public Token<R> Read(R type, string expectedTokenName = null)
        {
            if (End) 
                throw new RantException(_source, null, "Expected " + (expectedTokenName ?? "'" + RantLexer.Rules.GetSymbolForId(type) + "'") + ", but hit end of file.");
            if (_tokens[_pos].Identifier != type)
            {
                throw new RantException(_source, _tokens[_pos], "Expected " + (expectedTokenName ?? "'" + RantLexer.Rules.GetSymbolForId(type) + "'"));
            }
            return _tokens[_pos++];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Token<R> ReadLoose(R type, string expectedTokenName = null)
        {
            if (End)
                throw new RantException(_source, null, "Expected " + (expectedTokenName ?? "'" + RantLexer.Rules.GetSymbolForId(type) + "'") + ", but hit end of file.");
            SkipSpace();
            if (_tokens[_pos].Identifier != type)
            {
                throw new RantException(_source, _tokens[_pos], "Expected " + (expectedTokenName ?? "'" + RantLexer.Rules.GetSymbolForId(type) + "'"));
            }
            var t = _tokens[_pos++];
            SkipSpace();
            return t;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TakeAllUntil(R takeType, R untilType, bool allowEof = true)
        {
            if (End)
            {
                if (!allowEof)
                    throw new RantException(_source, null, "Unexpected end-of-file.");
                return false;
            }

            int i = _pos;
            R t;
            while (i < _tokens.Length)
            {
                t = _tokens[_pos].Identifier;
                if (t == takeType)
                {
                    i++;
                }
                else if (t == untilType)
                {
                    _pos = i;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool SkipSpace()
        {
            return TakeAll(R.Whitespace);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<IEnumerable<Token<R>>> ReadMultiItemScope(R open, R close, R separator, Brackets bracketPairs)
        {
            SkipSpace();
            _stack.Clear();

            // Assumes first bracket was already read.
            _stack.Push(new Token<R>(open, RantLexer.Rules.GetSymbolForId(open)));
            
            int start = _pos;
            Token<R> token = null;

            while (!End)
            {
                // Peek but don't consume - this saves some calculations later on.
                token = PeekToken();

                // Opening bracket
                if (bracketPairs.ContainsOpening(token.Identifier) || open == token.Identifier) // Previous bracket allows nesting
                {
                    _stack.Push(token);
                }

                // Closing bracket
                else if (bracketPairs.ContainsClosing(token.Identifier) || close == token.Identifier) // Previous bracket allows nesting
                {
                    var lastOpening = _stack.Pop();

                    // Handle invalid closures
                    if (!bracketPairs.Contains(lastOpening.Identifier, token.Identifier)) // Not in main pair
                    {
                        throw new RantException(_source, token, 
                            "Invalid closure '"
                            + lastOpening.Value
                            + " ... " 
                            + token.Value 
                            + "' - expected '" 
                            + RantLexer.Rules.GetSymbolForId(bracketPairs.GetClosing(lastOpening.Identifier)) 
                            + "'");
                    }

                    // If the _stack is empty, this is the last item. Stop the iterator.
                    if (!_stack.Any())
                    {
                        yield return _tokens
                        .SkipWhile((t, i) => i < start) // Cut to start of section
                        .TakeWhile((t, i) => i < _pos - start) // Cut to end of section
                        .SkipWhile(t => t.Identifier == R.Whitespace) // Remove leading whitespace
                        .Reverse() // Reverse to trim end
                        .SkipWhile(t => t.Identifier == R.Whitespace) // Remove trailing whitespace
                        .Reverse() // Reverse back
                        .ToArray();
                        _pos++;
                        yield break;
                    }
                }

                // Separator
                else if (token.Identifier == separator && _stack.Count == 1)
                {
                    yield return _tokens
                        .SkipWhile((t, i) => i < start) // Cut to start of section
                        .TakeWhile((t, i) => i < _pos - start) // Cut to end of section
                        .SkipWhile(t => t.Identifier == R.Whitespace) // Remove leading whitespace
                        .Reverse() // Reverse to trim end
                        .SkipWhile(t => t.Identifier == R.Whitespace) // Remove trailing whitespace
                        .Reverse() // Reverse back
                        .ToArray();
                    _pos++;
                    start = _pos;
                    continue;
                }

                // Move to next position
                _pos++;
            }
            throw new RantException(_source, null, "Unexpected end of file - expected '" + RantLexer.Rules.GetSymbolForId(close) + "'.");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<Token<R>> ReadToScopeClose(R open, R close, Brackets bracketPairs)
        {
            SkipSpace();
            _stack.Clear();
            Token<R> token = null;
            while (!End)
            {
                token = ReadToken();
                if (bracketPairs.ContainsOpening(token.Identifier) || open == token.Identifier) // Allows nesting
                {
                    _stack.Push(token);
                }
                else if (bracketPairs.ContainsClosing(token.Identifier) || token.Identifier == close) // Allows nesting
                {
                    // Since this method assumes that the first opening bracket was already read, an empty _stack indicates main scope closure.
                    if (!_stack.Any() && token.Identifier == close)
                    {
                        yield break;
                    }

                    var lastOpening = _stack.Pop();
                    
                    if (!bracketPairs.Contains(lastOpening.Identifier, token.Identifier))
                    {
                        throw new RantException(_source, token,
                            "Invalid closure '"
                            + lastOpening.Value
                            + " ... "
                            + token.Value
                            + "' - expected '"
                            + RantLexer.Rules.GetSymbolForId(bracketPairs.GetClosing(lastOpening.Identifier))
                            + "'");
                    }
                }
                yield return token;
            }
            throw new RantException(_source, null, "Unexpected end of file - expected '" + RantLexer.Rules.GetSymbolForId(close) + "'.");
        }
    }
}