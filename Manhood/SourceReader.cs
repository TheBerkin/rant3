using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

using Manhood.Compiler;

using Stringes.Tokens;

namespace Manhood
{
    internal class SourceReader
    {
        private readonly Source _source;
        private readonly Token<TokenType>[] _tokens;
        private int _pos;

        public SourceReader(Source source)
        {
            _source = source;
            _tokens = source.Tokens.ToArray();
            _pos = 0;
        }

        public Source Source
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

        public Token<TokenType> PrevToken
        {
            get { return _pos == 0 ? null : _tokens[_pos - 1]; }
        }
            
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Token<TokenType> ReadToken()
        {
            if (End) throw new ManhoodException(_source, null, "Unexpected end of file.");
            return _tokens[_pos++];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Token<TokenType> PeekToken()
        {
            if (End) throw new ManhoodException(_source, null, "Unexpected end of file.");
            return _tokens[_pos];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsNext(TokenType type)
        {
            return !End && _tokens[_pos].Identifier == type;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Take(TokenType type)
        {
            if (End) return false;
            if (_tokens[_pos].Identifier != type) return false;
            _pos++;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TakeAll(TokenType type)
        {
            if (End || _tokens[_pos].Identifier != type) return false;
            do
            {
                _pos++;
            } while (!End && _tokens[_pos].Identifier == type);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool SkipSpace()
        {
            return TakeAll(TokenType.SymbolSpacing);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<IEnumerable<Token<TokenType>>> ReadMultiItemScope(TokenType open, TokenType close, TokenType separator, BracketPairs bracketPairs)
        {
            // This exception should not happen when running patterns - only if I mess something up in the code.
            if (!IsNext(open)) throw new InvalidOperationException("The specified opening token does not occur at the reader position.");

            var stack = new Stack<Token<TokenType>>(2);
            int start = _pos + 1;
            Token<TokenType> token = null;

            while (!End)
            {
                // Peek but don't consume - this saves some calculations later on.
                token = PeekToken();

                // Opening bracket
                if (bracketPairs.ContainsOpening(token.Identifier) || open == token.Identifier) // Previous bracket allows nesting
                {
                    stack.Push(token);
                }

                // Closing bracket
                else if (bracketPairs.ContainsClosing(token.Identifier) || close == token.Identifier) // Previous bracket allows nesting
                {
                    var lastOpening = stack.Pop();

                    // Handle invalid closures
                    if (!bracketPairs.Contains(lastOpening.Identifier, token.Identifier)) // Not in main pair
                    {
                        throw new ManhoodException(_source, token, 
                            "Invalid closure '"
                            + lastOpening.Value
                            + " ... " 
                            + token.Value 
                            + "' - expected '" 
                            + Lexer.Rules.GetSymbolForId(bracketPairs.GetClosing(lastOpening.Identifier)) 
                            + "'");
                    }

                    // If the stack is empty, this is the last item. Stop the iterator.
                    if (!stack.Any())
                    {
                        yield return _tokens.SkipWhile((t, i) => i < start).TakeWhile((t, i) => i < _pos - start).ToArray();
                        _pos++;
                        yield break;
                    }
                }

                // Separator
                else if (token.Identifier == separator && stack.Count == 1)
                {
                    yield return _tokens.SkipWhile((t, i) => i < start).TakeWhile((t, i) => i < _pos - start).ToArray();
                    start = _pos + 1;
                }

                // Move to next position
                _pos++;
            }
            throw new ManhoodException(_source, null, "Unexpected end of file - expected '" + Lexer.Rules.GetSymbolForId(close) + "'.");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<IEnumerable<Token<TokenType>>> ReadItemsToClosureTrimmed(TokenType open, TokenType close, TokenType separator, BracketPairs bracketPairs)
        {
            var stack = new Stack<Token<TokenType>>(2);
            Token<TokenType> token = null;

            TakeAll(TokenType.SymbolSpacing);

            int start = _pos;
            while (!End)
            {
                token = PeekToken();
                if (bracketPairs.ContainsOpening(token.Identifier) || open == token.Identifier) // Allows nesting
                {
                    stack.Push(token);
                }
                else if (bracketPairs.ContainsClosing(token.Identifier) || token.Identifier == close) // Allows nesting
                {
                    // Since this method assumes that the first opening bracket was already read, an empty stack indicates main scope closure.
                    if (!stack.Any() && token.Identifier == close)
                    {
                        yield return _tokens.SkipWhile((t, i) => i < start).TakeWhile((t, i) => i < _pos - start).ToArray();
                        _pos++;
                        yield break;
                    }

                    var lastOpening = stack.Pop();
                    if (!bracketPairs.Contains(lastOpening.Identifier, token.Identifier))
                    {
                        throw new ManhoodException(_source, token,
                            "Invalid closure '"
                            + lastOpening.Value
                            + " ... "
                            + token.Value
                            + "' - expected '"
                            + Lexer.Rules.GetSymbolForId(bracketPairs.GetClosing(lastOpening.Identifier))
                            + "'");
                    }
                }
                else if (token.Identifier == separator && !stack.Any())
                {
                    yield return _tokens.SkipWhile((t, i) => i < start).TakeWhile((t, i) => i < _pos - start).ToArray();
                    _pos++;
                    TakeAll(TokenType.SymbolSpacing);
                    start = _pos;
                    continue;
                }

                _pos++;
            }
            throw new ManhoodException(_source, null, "Unexpected end of file - expected '" + Lexer.Rules.GetSymbolForId(close) + "'.");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<Token<TokenType>> ReadToScopeClose(TokenType open, TokenType close, BracketPairs bracketPairs)
        {
            var stack = new Stack<Token<TokenType>>(2);
            Token<TokenType> token = null;
            while (!End)
            {
                token = ReadToken();
                if (bracketPairs.ContainsOpening(token.Identifier) || open == token.Identifier) // Allows nesting
                {
                    stack.Push(token);
                }
                else if (bracketPairs.ContainsClosing(token.Identifier) || token.Identifier == close) // Allows nesting
                {
                    // Since this method assumes that the first opening bracket was already read, an empty stack indicates main scope closure.
                    if (!stack.Any() && token.Identifier == close)
                    {
                        yield break;
                    }

                    var lastOpening = stack.Pop();
                    if (!bracketPairs.Contains(lastOpening.Identifier, token.Identifier))
                    {
                        throw new ManhoodException(_source, token,
                            "Invalid closure '"
                            + lastOpening.Value
                            + " ... "
                            + token.Value
                            + "' - expected '"
                            + Lexer.Rules.GetSymbolForId(bracketPairs.GetClosing(lastOpening.Identifier))
                            + "'");
                    }
                }
                yield return token;
            }
            throw new ManhoodException(_source, null, "Unexpected end of file - expected '" + Lexer.Rules.GetSymbolForId(close) + "'.");
        }
    }
}