using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

namespace Manhood
{
    internal class Scanner
    {
        private static readonly Dictionary<char, char> _escapeChars = new Dictionary<char, char>
        {
            {'n', '\n'},
            {'r', '\r'},
            {'t', '\t'},
            {'b', '\b'},
            {'f', '\f'},
            {'v', '\v'},
            {'0', '\0'},
            {'s', ' '}
        };

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
            set { _position = value; }
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
            char c = _string[_position++];
            if (c != '\\') return c;

            // Escape sequence reading
            if (Next == -1) throw new EndOfStreamException("Incomplete escape sequence at end of string.");
            c = _string[_position++];

            if (c == 'u')
            {
                if (Remaining < 4)
                {
                    _position = _string.Length;
                    return '?';
                }
                var code = ReadTo(_position + 4).ToLower();
                if (!code.All(ch => "1234567890abcdef".Contains(ch))) return '?';
                return (char)Convert.ToInt16(code, 16);
            }

            char escaped;
            return _escapeChars.TryGetValue(c, out escaped) ? escaped : c;
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
            while (!EndOfString && "\r\n\t ".Contains((char)Next)) _position++;
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

        public bool ReadBlock(out BlockInfo block)
        {
            block = null;
            if (Next != '{') return false;
            if (_string.IndexOf('}', _position + 1) == -1) return false;
            int balance = 0;
            int elemStart = _position + 1;
            var ranges = new List<Range>();

            bool escapeNext = false;
            for (int i = _position; i < _string.Length; i++)
            {
                if (escapeNext)
                {
                    escapeNext = false;
                    continue;
                }
                switch (_string[i])
                {
                    case '\\':
                        escapeNext = true;
                        continue;
                    case '{':
                        balance--;
                        break;
                    case '|':
                        if (balance == -1)
                        {
                            ranges.Add(new Range(elemStart, i - elemStart));
                            elemStart = i + 1;
                        }
                        break;
                    case '}':
                        if (++balance == 0)
                        {
                            ranges.Add(new Range(elemStart, i - elemStart));
                            block = new BlockInfo(_position + 1, i + 1, ranges.ToArray());
                            _position = block.End;
                            return true;
                        }
                        break;
                }
            }
            return false;
        }

        public bool ReadTag(out TagInfo tag)
        {
            tag = null;
            if (Next != '[') return false;
            if (_string.IndexOf(']', _position + 1) == -1) return false;
            int balance = 0;

            bool escapeNext = false;
            for (int i = _position; i < _string.Length; i++)
            {
                if (escapeNext)
                {
                    escapeNext = false;
                    continue;
                }

                switch (_string[i])
                {
                    case '\\':
                        escapeNext = true;
                        continue;
                    case '[':
                        balance--;
                        break;
                    case ']':
                        if (++balance == 0)
                        {
                            tag = new TagInfo(_string.Substring(_position + 1, i - (_position + 1)));
                            _position = i + 1;
                            return true;
                        }
                        break;
                }
            }
            return false;
        }

        public bool ReadWordCall(out Query wc)
        {
            wc = null;
            if (Next != '<' || Escaped(_position + 1)) return false;
            int endPos;
            if ((endPos = IndexOf('>', _position + 1)) == -1) return false;
            wc = Query.Parse(ReadTo(endPos + 1));
            return wc != null;
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