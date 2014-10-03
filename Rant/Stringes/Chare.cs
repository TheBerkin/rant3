using System.Globalization;

namespace Stringes
{
    /// <summary>
    /// Provides information about a character in a stringe.
    /// </summary>
    public sealed class Chare
    {
        private readonly Stringe _src;
        private readonly char _character;
        private readonly int _offset;
        private int _line;
        private int _column;

        public Stringe Source
        {
            get { return _src; }
        }

        public char Character
        {
            get { return _character; }
        }

        public int Offset
        {
            get { return _offset; }
        }

        public int Line
        {
            get
            {
                if (_line == 0) SetLineCol();
                return _line;
            }
        }

        public int Column
        {
            get
            {
                if (_column == 0) SetLineCol();
                return _column;
            }
        }

        private void SetLineCol()
        {
            _line = _src.Line;
            _column = _src.Column;
            if (_offset <= 0) return;
            for (int i = 0; i < _offset; i++)
            {
                if (_src.ParentString[_offset] == '\n')
                {
                    _line++;
                    _column = 1;
                }
                else
                {
                    _column++;
                }
            }
        }

        internal Chare(Stringe source, char c, int offset)
        {
            _src = source;
            _character = c;
            _offset = offset;
            _line = _column = 0;
        }

        internal Chare(Stringe source, char c, int offset, int line, int col)
        {
            _src = source;
            _character = c;
            _offset = offset;
            _line = line;
            _column = col;
        }

        public override string ToString()
        {
            return _character.ToString(CultureInfo.InvariantCulture);
        }

        public static implicit operator char(Chare chare)
        {
            return chare._character;
        }
    }
}