using System.Collections.Generic;
using System.Linq;

using Rant.Engine.Compiler;
using Rant.Stringes;

namespace Rant.Engine
{
    internal class TokenReader
    {
        private readonly string _sourceName;
        private readonly Token<R>[] _tokens;
        private int _pos;

        private readonly Stack<Token<R>> _stack = new Stack<Token<R>>(10); 

        public TokenReader(string sourceName, IEnumerable<Token<R>> tokens)
        {
            _sourceName = sourceName;
            _tokens = tokens.ToArray();
            _pos = 0;
        }

        public string SourceName => _sourceName;

	    public int Position
        {
            get { return _pos; }
            set { _pos = value; }
        }

        public bool End => _pos >= _tokens.Length;

	    public Token<R> PrevToken => _pos == 0 ? null : _tokens[_pos - 1];

		public Token<R> PrevLooseToken
		{
			get
			{
				if (_pos == 0) return null;
				int tempPos = _pos - 1;
				while (tempPos > 0 && _tokens[tempPos].ID == R.Whitespace)
					tempPos--;
				return _tokens[tempPos].ID != R.Whitespace ? _tokens[tempPos] : null;
			}
		}

	    public Token<R> ReadToken()
        {
            if (End) throw new RantCompilerException(_sourceName, null, "Unexpected end of file.");
            return _tokens[_pos++];
        }

        
        public Token<R> PeekToken()
        {
            if (End) throw new RantCompilerException(_sourceName, null, "Unexpected end of file.");
            return _tokens[_pos];
        }

		public Token<R> PeekLooseToken()
		{
			if (End) throw new RantCompilerException(_sourceName, null, "Unexpected end of file.");
			int pos = _pos;
			SkipSpace();
			var token = _tokens[_pos];
			_pos = pos;
			return token;
		}

	    public R PeekType() => End ? R.EOF : _tokens[_pos].ID;

	    public bool IsNext(R type)
        {
            return !End && _tokens[_pos].ID == type;
        }

	    public R LastNonSpaceType
	    {
		    get
		    {
			    if (_pos == 0) return R.EOF;
			    R id;
			    for (int i = _pos; i >= 0; i--)
			    {
				    if ((id = _tokens[i].ID) != R.Whitespace) return id;
			    }
			    return R.EOF;
		    }
	    }

        public bool Take(R type, bool allowEof = true)
        {
            if (End)
            {
                if (!allowEof)
                    throw new RantCompilerException(_sourceName, null, "Unexpected end-of-file.");
                return false;
            }
            if (_tokens[_pos].ID != type) return false;
            _pos++;
            return true;
        }

        public bool TakeLoose(R type, bool allowEof = true)
        {
            if (End)
            {
                if (!allowEof)
                    throw new RantCompilerException(_sourceName, null, "Unexpected end-of-file.");
                return false;
            }
            SkipSpace();
            if (_tokens[_pos].ID != type) return false;
            _pos++;
            SkipSpace();
            return true;
        }

        public bool TakeAny(params R[] types)
        {
            if (End) return false;
            foreach (var type in types)
            {
                if (_tokens[_pos].ID != type) continue;
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
                if (_tokens[_pos].ID != type) continue;
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
                if (_tokens[_pos].ID != type) continue;
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
                if (_tokens[_pos].ID != type) continue;
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
                    throw new RantCompilerException(_sourceName, null, "Unexpected end-of-file.");
                return false;
            }
            if (_tokens[_pos].ID != type) return false;
            do
            {
                _pos++;
            } while (!End && _tokens[_pos].ID == type);
            return true;
        }

        public Token<R> Read(R type, string expectedTokenName = null)
        {
            if (End) 
                throw new RantCompilerException(_sourceName, null, "Expected " + (expectedTokenName ?? "'" + RantLexer.Rules.GetSymbolForId(type) + "'") + ", but hit end of file.");
            if (_tokens[_pos].ID != type)
            {
                throw new RantCompilerException(_sourceName, _tokens[_pos], "Expected " + (expectedTokenName ?? "'" + RantLexer.Rules.GetSymbolForId(type) + "'"));
            }
            return _tokens[_pos++];
        }

        
        public Token<R> ReadLoose(R type, string expectedTokenName = null)
        {
            if (End)
                throw new RantCompilerException(_sourceName, null, "Expected " + (expectedTokenName ?? "'" + RantLexer.Rules.GetSymbolForId(type) + "'") + ", but hit end of file.");
            SkipSpace();
            if (_tokens[_pos].ID != type)
            {
                throw new RantCompilerException(_sourceName, _tokens[_pos], "Expected " + (expectedTokenName ?? "'" + RantLexer.Rules.GetSymbolForId(type) + "'"));
            }
            var t = _tokens[_pos++];
            SkipSpace();
            return t;
        }

		public Token<R> ReadLooseToken()
		{
			if (End)
				throw new RantCompilerException(_sourceName, null, "Expected token, but hit end of file.");
			SkipSpace();
			var token = _tokens[_pos++];
			SkipSpace();
			return token;
		}
        
        public bool TakeAllUntil(R takeType, R untilType, bool allowEof = true)
        {
            if (End)
            {
                if (!allowEof)
                    throw new RantCompilerException(_sourceName, null, "Unexpected end-of-file.");
                return false;
            }

            int i = _pos;
            R t;
            while (i < _tokens.Length)
            {
                t = _tokens[_pos].ID;
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

        
        public bool SkipSpace() => TakeAll(R.Whitespace);
    }
}