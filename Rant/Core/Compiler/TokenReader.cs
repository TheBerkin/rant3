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

using System;
using System.Collections.Generic;
using System.Linq;

using static Rant.Localization.Txtres;

namespace Rant.Core.Compiler
{
    internal class TokenReader
    {
        private readonly RantCompiler _compiler;
        private readonly Token[] _tokens;

        public TokenReader(string sourceName, IEnumerable<Token> tokens, RantCompiler compiler)
        {
            SourceName = sourceName;
            _compiler = compiler;
            _tokens = tokens.ToArray();
            Position = 0;
        }

        public string SourceName { get; }
        public int Position { get; set; }

        /// <summary>
        /// Determines whether the reader has reached the end of the token stream.
        /// </summary>
        public bool End => Position >= _tokens.Length;

        /// <summary>
        /// The last token that was read.
        /// </summary>
        public Token PrevToken => Position == 0 ? Token.None : _tokens[Position - 1];

        public LineCol BaseLocation => _tokens.Length > 0 ? _tokens[0].ToLocation() : new LineCol(1, 1, 0);

        /// <summary>
        /// The last non-whitespace token before the current reader position.
        /// </summary>
        public Token PrevLooseToken
        {
            get
            {
                if (Position == 0) return Token.None;
                int tempPos = Position - 1;
                while (tempPos > 0 && _tokens[tempPos].Type == R.Whitespace)
                    tempPos--;
                return _tokens[tempPos].Type != R.Whitespace ? _tokens[tempPos] : Token.None;
            }
        }

        /// <summary>
        /// The last non-whitespace token type.
        /// </summary>
        public R LastNonSpaceType
        {
            get
            {
                if (Position == 0) return R.EOF;
                R Type;
                for (int i = Position; i >= 0; i--)
                    if ((Type = _tokens[i].Type) != R.Whitespace) return Type;
                return R.EOF;
            }
        }

        public Token this[int pos] => _tokens[pos];

        /// <summary>
        /// Reads the next available token.
        /// </summary>
        /// <returns></returns>
        public Token ReadToken()
        {
            if (End)
            {
                _compiler.SyntaxError(Token.None, true, GetString("err-compiler-eof"));
                return Token.None;
            }
            return _tokens[Position++];
        }

        /// <summary>
        /// Returns the next available token, but does not consume it.
        /// </summary>
        /// <returns></returns>
        public Token PeekToken()
        {
            return End ? Token.None : _tokens[Position];
        }

        /// <summary>
        /// Returns the next available non-whitespace token, but does not consume it.
        /// </summary>
        /// <returns></returns>
        public Token PeekLooseToken()
        {
            if (End)
            {
                _compiler.SyntaxError(Token.None, true, "err-compiler-eof");
                return Token.None;
            }
            int pos = Position;
            SkipSpace();
            var token = _tokens[Position];
            Position = pos;
            return token;
        }

        /// <summary>
        /// Returns the type of the next available token.
        /// </summary>
        /// <returns></returns>
        public R PeekType() => End ? R.EOF : _tokens[Position].Type;

        /// <summary>
        /// Determines whether the next token is of the specified type.
        /// </summary>
        /// <param name="type">The type to check for.</param>
        /// <returns></returns>
        public bool IsNext(R type)
        {
            return !End && _tokens[Position].Type == type;
        }

        /// <summary>
        /// Consumes the next token if its type matches the specified type. Returns true if it matches.
        /// </summary>
        /// <param name="type">The type to consume.</param>
        /// <param name="allowEof">Allow end-of-file tokens. Specifying False will throw an exception instead.</param>
        /// <returns></returns>
        public bool Take(R type, bool allowEof = true)
        {
            if (End)
            {
                if (!allowEof)
                {
                    _compiler.SyntaxError(Token.None, true, "err-compiler-eof");
                    return false;
                }
                return false;
            }
            if (_tokens[Position].Type != type) return false;
            Position++;
            return true;
        }

        /// <summary>
        /// Consumes the next non-whitespace token if its type matches the specified type. Returns true if it matches.
        /// </summary>
        /// <param name="type">The type to consume.</param>
        /// <param name="allowEof">Allow end-of-file tokens. Specifying False will throw an exception instead.</param>
        /// <returns></returns>
        public bool TakeLoose(R type, bool allowEof = true)
        {
            if (End)
            {
                if (!allowEof)
                {
                    _compiler.SyntaxError(Token.None, true, "err-compiler-eof");
                    return false;
                }
                return false;
            }
            SkipSpace();
            if (_tokens[Position].Type != type) return false;
            Position++;
            SkipSpace();
            return true;
        }

        /// <summary>
        /// Consumes the next token if its type matches any of the specified types. Returns true if a match was found.
        /// </summary>
        /// <param name="types">The types to consume.</param>
        /// <returns></returns>
        public bool TakeAny(params R[] types)
        {
            if (End) return false;
            foreach (var type in types)
            {
                if (_tokens[Position].Type != type) continue;
                Position++;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Consumes the next token if its type matches any of the specified types, and outputs the matching type. Returns true if
        /// a match was found.
        /// </summary>
        /// <param name="result">The matched type.</param>
        /// <param name="types">The types to consume.</param>
        /// <returns></returns>
        public bool TakeAny(out R result, params R[] types)
        {
            result = default(R);
            if (End) return false;
            foreach (var type in types)
            {
                if (_tokens[Position].Type != type) continue;
                result = type;
                Position++;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Consumes the next non-whitespace token if its type matches any of the specified types. Returns true if a match was
        /// found.
        /// </summary>
        /// <param name="types">The types to consume.</param>
        /// <returns></returns>
        public bool TakeAnyLoose(params R[] types)
        {
            if (End) return false;
            SkipSpace();
            foreach (var type in types)
            {
                if (_tokens[Position].Type != type) continue;
                Position++;
                SkipSpace();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Consumes the next non-whitespace token if its type matches any of the specified types, and outputs the matching type.
        /// Returns true if a match was found.
        /// </summary>
        /// <param name="result">The matched type.</param>
        /// <param name="types">The types to consume.</param>
        /// <returns></returns>
        public bool TakeAnyLoose(out R result, params R[] types)
        {
            result = default(R);
            if (End) return false;
            SkipSpace();
            foreach (var type in types)
            {
                if (_tokens[Position].Type != type) continue;
                result = type;
                Position++;
                SkipSpace();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Consumes as many tokens as possible, as long as they match the specified type. Returns true if at least one was found.
        /// </summary>
        /// <param name="type">The type to consume.</param>
        /// <param name="allowEof">Allow end-of-file tokens. Specifying False will throw an exception instead.</param>
        /// <returns></returns>
        public bool TakeAll(R type, bool allowEof = true)
        {
            if (End)
            {
                if (!allowEof)
                {
                    _compiler.SyntaxError(Token.None, true, "err-compiler-eof");
                    return false;
                }
                return false;
            }
            if (_tokens[Position].Type != type) return false;
            do
            {
                Position++;
            } while (!End && _tokens[Position].Type == type);
            return true;
        }

        /// <summary>
        /// Consumes as many non-whitespace tokens as possible, as long as they match the specified type. Returns true if at least
        /// one was found.
        /// </summary>
        /// <param name="type">The type to consume.</param>
        /// <param name="allowEof">Allow end-of-file tokens. Specifying False will throw an exception instead.</param>
        /// <returns></returns>
        public bool TakeAllLoose(R type, bool allowEof = true)
        {
            if (End)
            {
                if (!allowEof)
                {
                    _compiler.SyntaxError(Token.None, true, "err-compiler-eof");
                    return false;
                }
                return false;
            }
            SkipSpace();
            if (_tokens[Position].Type != type) return false;
            do
            {
                SkipSpace();
                Position++;
            } while (!End && _tokens[Position].Type == type);
            return true;
        }

        /// <summary>
        /// Reads and returns the next token if its type matches the specified type.
        /// If it does not match, a RantCompilerException is thrown with the expected token name.
        /// </summary>
        /// <param name="type">The token type to read.</param>
        /// <param name="expectedTokenName">A display name describing what the token is for.</param>
        /// <returns></returns>
        public Token Read(R type, string expectedTokenName = null)
        {
            if (End)
            {
                _compiler.SyntaxError(Token.None, true, "err-compiler-missing-token-eof",
                    expectedTokenName != null ? GetString(expectedTokenName) : RantLexer.GetSymbolForType(type));
                return Token.None;
            }
            if (_tokens[Position].Type != type)
            {
                _compiler.SyntaxError(_tokens[Position], false, "err-compiler-missing-token",
                    expectedTokenName != null ? GetString(expectedTokenName) : RantLexer.GetSymbolForType(type));
                return Token.None;
            }
            return _tokens[Position++];
        }

        /// <summary>
        /// Reads and returns the next token if its type matches any of the given types
        /// If it does not match, a RantCompilerException is thrown with the expected token names.
        /// </summary>
        /// <param name="types">The token types accepted for the read token.</param>
        /// <returns></returns>
        public Token ReadAny(params R[] types)
        {
            if (End)
            {
                _compiler.SyntaxError(Token.None, true,
                    "err-compiler-missing-token-any-eof",
                    string.Join(" ", types.Select(RantLexer.GetSymbolForType).ToArray()));
                return Token.None;
            }

            if (!types.Contains(_tokens[Position].Type)) // NOTE: .Contains isn't too fast but does it matter in this case?
            {
                _compiler.SyntaxError(_tokens[Position], false, "err-compiler-missing-token-any",
                    string.Join(" ", types.Select(RantLexer.GetSymbolForType).ToArray()));
                return Token.None;
            }

            return _tokens[Position++];
        }

        /// <summary>
        /// Reads and returns the next non-whitespace token if its type matches the specified type.
        /// If it does not match, a RantCompilerException is thrown with the expected token name.
        /// </summary>
        /// <param name="type">The token type to read.</param>
        /// <param name="expectedTokenName">A display name describing what the token is for.</param>
        /// <returns></returns>
        public Token ReadLoose(R type, string expectedTokenName = null)
        {
            if (End)
            {
                _compiler.SyntaxError(Token.None, true, "err-compiler-missing-token-eof",
                    expectedTokenName != null ? GetString(expectedTokenName) : RantLexer.GetSymbolForType(type));
                return Token.None;
            }
            SkipSpace();
            if (_tokens[Position].Type != type)
            {
                _compiler.SyntaxError(_tokens[Position], false, "err-compiler-missing-token",
                    expectedTokenName != null ? GetString(expectedTokenName) : RantLexer.GetSymbolForType(type));
                return Token.None;
            }
            var t = _tokens[Position++];
            SkipSpace();
            return t;
        }

        /// <summary>
        /// Reads a series of tokens into a buffer as long as they match the types specified in an array, in the order they appear.
        /// Returns True if reading was successful.
        /// </summary>
        /// <param name="buffer">The buffer to read into.</param>
        /// <param name="offset">The offset at which to begin writing tokens into the buffer.</param>
        /// <param name="types">The required types.</param>
        /// <returns></returns>
        public bool TakeSeries(Token[] buffer, int offset, params R[] types)
        {
            if (Position >= _tokens.Length) return types.Length == 0;
            if (_tokens.Length - Position < types.Length || buffer.Length - offset < types.Length)
                return false;
            for (int i = 0; i < types.Length; i++)
            {
                if (_tokens[Position + i].Type != types[i]) return false;
                buffer[i + offset] = _tokens[Position + i];
            }
            Position += types.Length;
            return true;
        }

        /// <summary>
        /// Reads a series of non-whitespace tokens into a buffer as long as they match the types specified in an array, in the
        /// order they appear. Returns True if reading was successful.
        /// </summary>
        /// <param name="buffer">The buffer to read into.</param>
        /// <param name="offset">The offset at which to begin writing tokens into the buffer.</param>
        /// <param name="types">The required types.</param>
        /// <returns></returns>
        public bool TakeSeriesLoose(Token[] buffer, int offset, params R[] types)
        {
            if (Position >= _tokens.Length) return types.Length == 0;
            if (_tokens.Length - Position < types.Length || buffer.Length - offset < types.Length)
                return false;
            int i = 0;
            int j = 0;
            while (i < types.Length)
            {
                if (Position + j >= _tokens.Length) return false;
                while (_tokens[Position + j].Type == R.Whitespace) j++;
                if (Position + j >= _tokens.Length) return false;

                if (_tokens[Position + j].Type != types[i]) return false;
                buffer[i + offset] = _tokens[Position + i];
                j++;
                i++;
            }
            Position += j;
            return true;
        }

        /// <summary>
        /// Reads and returns the next non-whitespace token.
        /// </summary>
        /// <returns></returns>
        public Token ReadLooseToken()
        {
            if (End)
            {
                _compiler.SyntaxError(Token.None, true, "err-compiler-eof");
                return Token.None;
            }
            SkipSpace();
            var token = _tokens[Position++];
            //SkipSpace();
            return token;
        }

        /// <summary>
        /// Consumes as many token as possible while they satisfy the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate to test tokens with.</param>
        /// <param name="allowEof">Allow end-of-file tokens. Specifying False will throw an exception instead.</param>
        /// <returns></returns>
        public bool TakeAllWhile(Func<Token, bool> predicate, bool allowEof = true)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            if (End)
            {
                if (!allowEof)
                    _compiler.SyntaxError(Token.None, true, "err-compiler-eof");
                return false;
            }

            int i = Position;
            Token t;
            while (Position < _tokens.Length)
            {
                t = _tokens[Position];
                if (!predicate(t))
                    return Position > i;
                Position++;
            }
            return true;
        }

        public bool SkipSpace() => TakeAll(R.Whitespace);
    }
}