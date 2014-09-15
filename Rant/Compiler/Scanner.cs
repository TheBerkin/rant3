using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Rant.Compiler
{
    internal class Scanner
    {
        private int _position;
        private readonly string _string;

        public Scanner(string input)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }
            _string = input;
            _position = 0;
        }

        public int Next
        {
            get { return _position >= _string.Length ? -1 : _string[_position]; }
        }

        public int Prev
        {
            get { return _position <= 0 ? -1 : _string[_position - 1]; }
        }

        public bool EndOfString
        {
            get { return _position >= _string.Length; }
        }

        public int Position
        {
            get { return _position; }
        }

        public int Remaining
        {
            get { return _string.Length - _position; }
        }

        public char ReadChar()
        {
            if (_position >= _string.Length)
            {
                throw new EndOfStreamException("Tried to read past the end of the string.");
            }
            return _string[_position++];
        }

        public bool Eat(string next)
        {
            if (_string.IndexOf(next, _position, StringComparison.Ordinal) != _position) return false;
            _position += next.Length;
            return true;
        }

        public bool Eat(Regex regex)
        {
            var m = regex.Match(_string, _position);
            if (!m.Success) return false;
            _position += m.Length;
            return true;
        }

        public bool Eat(Regex regex, out string value)
        {
            value = "";
            var m = regex.Match(_string, _position);
            if (!m.Success) return false;
            _position += m.Length;
            value = m.Value;
            return true;
        }

        public void EatWhitespace()
        {
            while (!EndOfString && Char.IsWhiteSpace(_string[_position])) _position++;
        }

        public string ReadString(int length)
        {
            if (_position + length > _string.Length)
            {
                throw new EndOfStreamException("Tried to read past the end of the string.");
            }
            var output = _string.Substring(_position, length);
            _position += length;
            return output;
        }

        public string ReadTo(int position)
        {
            if (position < _position)
            {
                throw new ArgumentException("Destination position is less than the current position.");
            }
            if (position > _string.Length)
            {
                throw new EndOfStreamException("Tried to read past the end of the string.");
            }

            var output = _string.Substring(_position, position - _position);
            _position += position - _position;
            return output;
        }

        public int IndexOf(char c, int start = 0)
        {
            for (int i = start; i < _string.Length; i++)
            {
                if (c != '\\')
                {
                    if (_string[i] == c && !Escaped(i))
                    {
                        return i;
                    }
                }
                else if (Escaped(i) && _string[i] == c)
                {
                    return i;
                }
            }
            return -1;
        }

        public int Before(int position)
        {
            if (position < 1 || position >= _string.Length) return -1;
            return _string[position - 1];
        }

        public bool Escaped(int position)
        {
            if (position < 1 || position >= _string.Length) return false;
            return _string[position - 1] == '\\';
        }

        public bool EscapesNext
        {
            get { return Escaped(_position + 1); }
        }

        public string ReadLine()
        {
            if (_position >= _string.Length)
            {
                throw new EndOfStreamException("Tried to read past the end of the string.");
            }

            int i = _string.IndexOf('\n', _position);
            string output;

            if (i == -1)
            {
                output = _string.Substring(_position);
                _position = _string.Length;
                return output;
            }

            output = _string.Substring(_position, i - _position).Trim('\r');
            _position = i + 1;
            return output;
        }
    }
}