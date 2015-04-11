using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Rant.Stringes
{
	/// <summary>
	/// Represents a string or a substring in relation to its parent. Provides line number, column, offset, and other useful data.
	/// </summary>
	internal class Stringe : IEnumerable<Chare>
    {
        private readonly Stref _stref;
        private readonly int _offset;
        private readonly int _length;
        private readonly int _line;
        private readonly int _column;
        private string _substring;

        // Used to cache requested metadata so that we don't have a bunch of unused fields
        private Dictionary<string, object> _meta = null;

        /// <summary>
        /// Returns an empty stringe based on the position of another stringe.
        /// </summary>
        /// <param name="basis">The basis stringe to get position info from.</param>
        /// <returns></returns>
        public static Stringe Empty(Stringe basis)
        {
            return new Stringe(basis, 0, 0);
        }

        /// <summary>
        /// Indicates whether the specified stringe is null or empty.
        /// </summary>
        /// <param name="stringe">The stringe to test.</param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(Stringe stringe)
        {
            return stringe == null || stringe.Length == 0;
        }

        /// <summary>
        /// Returns a stringe whose endpoints are the specified stringes. The stringes must both belong to the same parent string.
        /// </summary>
        /// <param name="a">The first stringe.</param>
        /// <param name="b">The second stringe.</param>
        /// <returns></returns>
        public static Stringe Range(Stringe a, Stringe b)
        {
            if (a == null) throw new ArgumentNullException("a");
            if (b == null) throw new ArgumentNullException("b");
            if (a._stref != b._stref)
                throw new ArgumentException("The stringes do not belong to the same parent.");

            if (a == b) return a;
            if (a.IsSubstringeOf(b)) return b;
            if (b.IsSubstringeOf(a)) return a;

            // Right side of A intersects left side of B.
            if (a._offset > b._offset && a._offset + a._length < b._offset + b._length)
            {
                return a.Substringe(0, b._offset + b._length - a._offset);
            }

            // Left side of A intersects right side of B.
            if (a._offset < b._offset + b._length && a._offset > b._offset)
            {
                return b.Substringe(0, a._offset + a._length - b._offset);
            }

            // A is to the left of B.
            if (a._offset + a._length <= b._offset)
            {
                return a.Substringe(0, b._offset + b._length - a._offset);
            }

            // B is to the left of A.
            if (b._offset + b._length <= a._offset)
            {
                return b.Substringe(0, a._offset + a._length - b._offset);
            }

            return null;
        }

        /// <summary>
        /// Returns a stringe comprised of all text between the two specified stringes. Returns null if the stringes are adjacent or intersected.
        /// </summary>
        /// <param name="a">The first stringe.</param>
        /// <param name="b">The second stringe.</param>
        /// <returns></returns>
        public static Stringe Between(Stringe a, Stringe b)
        {
            if (a == null) throw new ArgumentNullException("a");
            if (b == null) throw new ArgumentNullException("b");
            if (a._stref != b._stref)
                throw new ArgumentException("The stringes do not belong to the same parent.");

            if (a == b) return a;
            if (a.IsSubstringeOf(b)) return b;
            if (b.IsSubstringeOf(a)) return a;

            // Right side of A intersects left side of B.
            if (a._offset > b._offset && a._offset + a._length < b._offset + b._length)
            {
                return null;
            }

            // Left side of A intersects right side of B.
            if (a._offset < b._offset + b._length && a._offset > b._offset)
            {
                return null;
            }

            // A is to the left of B.
            if (a._offset + a._length <= b._offset)
            {
                return a.Substringe(a._length, b._offset - a._offset - a._length);
            }

            // B is to the left of A.
            if (b._offset + b._length <= a._offset)
            {
                return b.Substringe(b._length, a._offset - b._offset - b._length);
            }

            return null;
        }

        /// <summary>
        /// The offset of the stringe in the string.
        /// </summary>
        public int Offset => _offset;

        /// <summary>
        /// The length of the string represented by the stringe.
        /// </summary>
        public int Length => _length;

        /// <summary>
        /// The 1-based line number at which the stringe begins.
        /// </summary>
        public int Line => _line;

        /// <summary>
        /// The 1-based column at which the stringe begins.
        /// </summary>
        public int Column => _column;

        /// <summary>
        /// The index at which the stringe ends in the string.
        /// </summary>
        public int End => _offset + _length;

        /// <summary>
        /// Indicates if the stringe is a substring.
        /// </summary>
        public bool IsSubstring => _offset > 0 || _length < _stref.String.Length;

        /// <summary>
        /// Indicates if the stringe is empty.
        /// </summary>
        public bool IsEmpty => _length == 0;

        /// <summary>
        /// The substring value represented by the stringe. If the stringe is the parent, this will provide the original string.
        /// </summary>
        public string Value => _substring ?? (_substring = _stref.String.Substring(_offset, _length));

        /// <summary>
        /// Gets the original string from which the stringe was originally derived.
        /// </summary>
        public string ParentString => _stref.String;

        private Dictionary<string, object> Meta => _meta ?? (_meta = new Dictionary<string, object>());

        /// <summary>
        /// Creates a new stringe from the specified string.
        /// </summary>
        /// <param name="value">The string to turn into a stringe.</param>
        public Stringe(string value)
        {
            if (value == null) throw new ArgumentNullException("value");
            _stref = new Stref(value);
            _offset = 0;
            _length = value.Length;
            _line = 1;
            _column = 1;
            _substring = null;
        }

        /// <summary>
        /// The number of times the current string occurs in the parent string.
        /// </summary>
        /// <returns></returns>
        public int OccurrenceCount
        {
            get
            {
                const string name = "Occurrences";
                object obj;
                if (Meta.TryGetValue(name, out obj)) return (int)obj;

                int count = StringeUtils.GetMatchCount(_stref.String, Value);
                Meta[name] = count;
                return count;
            }
        }

        /// <summary>
        /// The next index in the parent string at which the current stringe value occurs.
        /// </summary>
        public int NextIndex
        {
            get
            {
                const string name = "NextIndex";
                object obj;
                if (Meta.TryGetValue(name, out obj)) return (int)obj;

                int nextIndex = _stref.String.IndexOf(Value, _offset + 1, StringComparison.InvariantCulture);
                Meta[name] = nextIndex;
                return nextIndex;
            }
        }

        internal Stringe(Stringe value)
        {
            _stref = value._stref;
            _offset = value._offset;
            _length = value._length;
            _line = value._line;
            _column = value._column;
            _offset = value._offset;
            _substring = value._substring;
        }

        private Stringe(Stringe parent, int relativeOffset, int length)
        {
            _stref = parent._stref;
            _offset = parent._offset + relativeOffset;
            _length = length;
            _substring = null;

            // Calculate line/col
            _line = parent._line;
            _column = parent._column;

            // If the offset is to the left, the line/col is already calculated. Fetch it from the Chare cache.
            if (relativeOffset < 0)
            {
                _line = _stref.Chares[_offset].Line;
                _column = _stref.Chares[_offset].Column;
                return;
            }

            if (relativeOffset == 0) return; // Do nothing if the offset is the same

            int aOffset;

            for (int i = 0; i < relativeOffset; i++)
            {
                aOffset = parent._offset + i;
                if (_stref.String[aOffset] == '\n')
                {
                    _line++;
                    _column = 1;
                }
                else if (_stref.Bases[i]) // Advance column only for non-combining characters
                {
                    _column++;
                }
                if (_stref.Chares[aOffset] == null)
                    _stref.Chares[aOffset] = new Chare(parent, _stref.String[aOffset], aOffset, _line, _column);
            }
        }

        /// <summary>
        /// Gets the charactere at the specified index in the stringe.
        /// </summary>
        /// <param name="index">The index of the charactere to retrieve.</param>
        /// <returns></returns>
        public Chare this[int index] => _stref.Chares[index] ?? (_stref.Chares[index] = new Chare(this, _stref.String[index], index + _offset));

        /// <summary>
        /// Determines whether the current stringe is a substringe of the specified parent stringe.
        /// </summary>
        /// <param name="parent">The parent stringe to compare to.</param>
        /// <returns></returns>
        public bool IsSubstringeOf(Stringe parent)
        {
            if (_stref != parent._stref) return false;
            return _offset >= parent._offset && _offset + _length <= parent._offset + parent._length;
        }

        /// <summary>
        /// Returns the zero-based index at which the specified string first occurs, relative to the substringe. The search starts at the specified index.
        /// </summary>
        /// <param name="input">The string to search for.</param>
        /// <param name="start">The index at which to begin the search.</param>
        /// <param name="comparisonType">The string comparison rules to apply to the search.</param>
        /// <returns></returns>
        public int IndexOf(string input, int start = 0, StringComparison comparisonType = StringComparison.Ordinal)
        {
            return Value.IndexOf(input, start, comparisonType);
        }

        /// <summary>
        /// Returns the zero-based index at which the specified string first occurs, relative to the parent string. The search starts at the specified index.
        /// </summary>
        /// <param name="input">The string to search for.</param>
        /// <param name="start">The index at which to begin the search.</param>
        /// <param name="comparisonType">The string comparison rules to apply to the search.</param>
        /// <returns></returns>
        public int IndexOfTotal(string input, int start = 0, StringComparison comparisonType = StringComparison.Ordinal)
        {
            int index = Value.IndexOf(input, start, comparisonType);
            return index == -1 ? index : index + _offset;
        }

        /// <summary>
        /// Returns the zero-based index at which the specified character first occurs, relative to the substringe. The search starts at the specified index.
        /// </summary>
        /// <param name="input">The character to search for.</param>
        /// <param name="start">The index at which to begin the search.</param>
        /// <returns></returns>
        public int IndexOf(char input, int start = 0)
        {
            return Value.IndexOf(input, start);
        }

        /// <summary>
        /// Returns the zero-based index at which the specified character first occurs, relative to the parent string. The search starts at the specified index.
        /// </summary>
        /// <param name="input">The character to search for.</param>
        /// <param name="start">The index at which to begin the search.</param>
        /// <returns></returns>
        public int IndexOfTotal(char input, int start = 0)
        {
            int index = Value.IndexOf(input, start);
            return index == -1 ? index : index + _offset;
        }

        /// <summary>
        /// Creates a substringe from the stringe, starting at the specified index and extending to the specified length.
        /// </summary>
        /// <param name="offset">The offset at which to begin the substringe.</param>
        /// <param name="length">The length of the substringe.</param>
        /// <returns></returns>
        public Stringe Substringe(int offset, int length)
        {
            return new Stringe(this, offset, length);
        }

        /// <summary>
        /// Create a substringe from the stringe, starting at the specified index and extending to the end.
        /// </summary>
        /// <param name="offset">The offset at which to begin the substringe.</param>
        /// <returns></returns>
        public Stringe Substringe(int offset)
        {
            return new Stringe(this, offset, Length - offset);
        }

        /// <summary>
        /// Returns a substringe that contains all characters between the two specified positions in the stringe.
        /// </summary>
        /// <param name="a">The left side of the slice.</param>
        /// <param name="b">The right side of the slice.</param>
        /// <returns></returns>
        public Stringe Slice(int a, int b)
        {
            if (b < a) throw new ArgumentException("'b' cannot be less tha 'a'.");
            if (b < 0 || a < 0) throw new ArgumentException("Indices cannot be negative.");
            if (a > _length || b > _length) throw new ArgumentException("Indices must be within stringe boundaries.");
            return new Stringe(this, a, b - a);
        }

        /// <summary>
        /// Returns a new substringe whose left and right boundaries are offset by the specified values.
        /// </summary>
        /// <param name="left">The amount, in characters, to offset the left boundary to the left.</param>
        /// <param name="right">The amount, in characters, to offset the right boundary to the right.</param>
        /// <returns></returns>
        public Stringe Dilate(int left, int right)
        {
            int exIndex = _offset - left;
            if (exIndex < 0) throw new ArgumentException("Expanded offset was negative.");
            int exLength = _length + right + left;
            if (exLength < 0) throw new ArgumentException("Expanded length was negative.");
            if (exIndex + exLength > _stref.String.Length) throw new ArgumentException("Expanded stringe tried to extend beyond the end of the string.");
            return new Stringe(this, -left, exLength);
        }


        /// <summary>
        /// Returns the stringe with all leading and trailing white space characters removed.
        /// </summary>
        /// <returns></returns>
        public Stringe Trim()
        {
            if (_length == 0) return this;
            int a = 0;
            int b = _length;
            do
            {
                if (Char.IsWhiteSpace(Value[a]))
                {
                    a++;
                }
                else if (Char.IsWhiteSpace(Value[b - 1]))
                {
                    b--;
                }
                else
                {
                    break;
                }
            } while (a < b && b > 0 && a < _length);

            return Substringe(a, b - a);
        }

        /// <summary>
        /// Returns the stringe with any occurrences of the specified characters stripped from the ends.
        /// </summary>
        /// <param name="trimChars">The characters to strip off the ends of the stringe.</param>
        /// <returns></returns>
        public Stringe Trim(params char[] trimChars)
        {
            if (_length == 0) return this;
            bool useDefault = trimChars.Length == 0;
            int a = 0;
            int b = _length;
            do
            {
                if (useDefault ? Char.IsWhiteSpace(Value[a]) : trimChars.Contains(Value[a]))
                {
                    a++;
                }
                else if (useDefault ? Char.IsWhiteSpace(Value[b - 1]) : trimChars.Contains(Value[b - 1]))
                {
                    b--;
                }
                else
                {
                    break;
                }
            } while (a < b && b > 0 && a < _length);

            return Substringe(a, b - a);
        }

        /// <summary>
        /// Returns a copy of the stringe with the specified characters removed from the start.
        /// </summary>
        /// <param name="trimChars">The characters to remove.</param>
        /// <returns></returns>
        public Stringe TrimStart(params char[] trimChars)
        {
            if (_length == 0) return this;
            bool useDefault = trimChars.Length == 0;
            int a = 0;
            while (a < _length)
            {
                if (useDefault ? Char.IsWhiteSpace(Value[a]) : trimChars.Contains(Value[a]))
                {
                    a++;
                }
                else
                {
                    break;
                }
            }
            return Substringe(a);
        }

        /// <summary>
        /// Returns a copy of the stringe with the specified characters removed from the end.
        /// </summary>
        /// <param name="trimChars">The characters to remove.</param>
        /// <returns></returns>
        public Stringe TrimEnd(params char[] trimChars)
        {
            if (_length == 0) return this;
            bool useDefault = trimChars.Length == 0;
            int b = _length;
            do
            {
                if (useDefault ? Char.IsWhiteSpace(Value[b - 1]) : trimChars.Contains(Value[b - 1]))
                {
                    b--;
                }
                else
                {
                    break;
                }
            } while (b > 0);
            return Substringe(0, b);
        }

        /// <summary>
        /// Indicates whether the left side of the line on which the stringe exists is composed entirely of white space.
        /// </summary>
        public bool LeftPadded
        {
            get
            {
                if (_offset == 0)
                {
                    for (int i = 0; i < _length; i++)
                    {
                        if (Char.IsWhiteSpace(_stref.String[i])) return true;
                    }
                    return false;
                }
                for (int i = _offset - 1; i >= 0; i--)
                {
                    if (!Char.IsWhiteSpace(_stref.String[i])) return false;
                    if (_stref.String[i] == '\n') return true;
                }
                return true;
            }
        }

        /// <summary>
        /// Indicates whether the line context to the right side of the stringe is composed on uninterrupted white space.
        /// </summary>
        public bool RightPadded
        {
            get
            {
                bool found = false;
                
                // The end of the stringe is at the end of the parent string.
                if (_offset + _length == _stref.String.Length) 
                {
                    for (int i = _stref.String.Length - 1; i >= _offset; i--)
                    {
                        if (!Char.IsWhiteSpace(_stref.String[i]))
                        {
                            return found;
                        }
                        found = true;
                    }
                    return false;
                }

                // The stringe sits in the middle of one or more lines.
                for (int i = _offset + _length; i < _stref.String.Length; i++)
                {
                    if (!Char.IsWhiteSpace(_stref.String[i])) return false;
                    if (_stref.String[i] == '\n') return true;
                }
                return true;
            }
        }

        /// <summary>
        /// Splits the stringe into multiple parts by the specified delimiters.
        /// </summary>
        /// <param name="separators">The delimiters by which to split the stringe.</param>
        /// <returns></returns>
        public IEnumerable<Stringe> Split(params string[] separators)
        {
            return Split(separators, StringSplitOptions.None);
        }

        /// <summary>
        /// Splits the stringe into multiple parts by the specified delimiters.
        /// </summary>
        /// <param name="separators">The delimiters by which to split the stringe.</param>
        /// <returns></returns>
        public IEnumerable<Stringe> Split(params char[] separators)
        {
            return Split(separators, StringSplitOptions.None);
        }

        /// <summary>
        /// Splits the stringe into multiple parts by the specified delimiters.
        /// </summary>
        /// <param name="separators">The delimiters by which to split the stringe.</param>
        /// <param name="options">Specifies whether empty substringes should be included in the return value.</param>
        /// <returns></returns>
        public IEnumerable<Stringe> Split(char[] separators, StringSplitOptions options)
        {
            int start = 0;
            for (int i = 0; i < _length; i++)
            {
                if (!separators.Contains(Value[i])) continue;
                if (options == StringSplitOptions.None || i - start > 0) yield return Substringe(start, i - start);
                start = i + 1;
            }
            if (start > _length) yield break;
            if (options == StringSplitOptions.None || _length - start > 0) yield return Substringe(start, _length - start);
        }

        /// <summary>
        /// Splits the stringe into multiple parts by the specified delimiters.
        /// </summary>
        /// <param name="separators">The delimiters by which to split the stringe.</param>
        /// <param name="options">Specifies whether empty substringes should be included in the return value.</param>
        /// <returns></returns>
        public IEnumerable<Stringe> Split(string[] separators, StringSplitOptions options)
        {
            int start = 0;
            for (int i = 0; i < _length; i++)
            {
                var hit = separators.FirstOrDefault(sep => IndexOf(sep) == i);
                if (hit == null) continue;
                if (options == StringSplitOptions.None || i - start > 0) yield return Substringe(start, i - start);
                start = i + hit.Length;
            }
            if (start > _length) yield break;
            if (options == StringSplitOptions.None || _length - start > 0) yield return Substringe(start, _length - start);
        }

        /// <summary>
        /// Splits the stringe into multiple parts by the specified delimiters.
        /// </summary>
        /// <param name="separators">The delimiters by which to split the stringe.</param>
        /// <param name="count">The maximum number of substringes to return. If the count exceeds this number, the last item will be the remainder of the stringe.</param>
        /// <param name="options">Specifies whether empty substringes should be included in the return value.</param>
        /// <returns></returns>
        public IEnumerable<Stringe> Split(char[] separators, int count, StringSplitOptions options = StringSplitOptions.None)
        {
            if (count == 0) yield break;
            if (count == 1)
            {
                yield return this;
                yield break;
            }

            int matches = 0;
            int start = 0;

            for (int i = 0; i < _length; i++)
            {
                if (separators.Contains(Value[i]))
                {
                    if (options == StringSplitOptions.None || i - start > 0) yield return Substringe(start, i - start);
                    start = i + 1;
                    matches++;
                }
                if (matches < count - 1) continue;
                if (start > _length) yield break;
                yield return Substringe(start, _length - start);
                yield break;
            }
            if (start > _length || matches >= count) yield break;
            if (options == StringSplitOptions.None || _length - start > 0) yield return Substringe(start, _length - start);
        }

        /// <summary>
        /// Splits the stringe into multiple parts by the specified delimiters.
        /// </summary>
        /// <param name="separators">The delimiters by which to split the stringe.</param>
        /// <param name="count">The maximum number of substringes to return. If the count exceeds this number, the last item will be the remainder of the stringe.</param>
        /// <param name="options">Specifies whether empty substringes should be included in the return value.</param>
        /// <returns></returns>
        public IEnumerable<Stringe> Split(string[] separators, int count, StringSplitOptions options = StringSplitOptions.None)
        {
            if (count == 0) yield break;
            if (count == 1)
            {
                yield return this;
                yield break;
            }

            int matches = 0;
            int start = 0;

            for (int i = 0; i < _length; i++)
            {
                var hit = separators.FirstOrDefault(sep => IndexOf(sep, i) == i);
                if (hit != null)
                {
                    if (options == StringSplitOptions.None || i - start > 0) yield return Substringe(start, i - start);
                    start = i + hit.Length;
                    matches++;
                }
                if (matches < count - 1) continue;
                if (start > _length) yield break;
                yield return Substringe(start, _length - start);
                yield break;
            }
            if (start > _length || matches >= count) yield break;
            if (options == StringSplitOptions.None || _length - start > 0) yield return Substringe(start, _length - start);
        }

        /// <summary>
        /// Converts a Stringe to its string value.
        /// </summary>
        /// <param name="stringe">The stringe to convert.</param>
        public static explicit operator string(Stringe stringe) => stringe.Value;

        /// <summary>
        /// Converts a string to a Stringe.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        public static implicit operator Stringe(string value) => new Stringe(value);

        /// <summary>
        /// Determines whether two stringes are equal.
        /// </summary>
        /// <param name="a">The first stringe.</param>
        /// <param name="b">The second stringe.</param>
        /// <returns></returns>
        public static bool operator ==(Stringe a, Stringe b)
        {
            if (Equals(a, null) && Equals(b, null)) return true;
            if (Equals(a, null) || Equals(b, null)) return false;
            return a._stref == b._stref && a.Value == b.Value;
        }

        /// <summary>
        /// Determines whether two stringes are not equal.
        /// </summary>
        /// <param name="a">The first stringe.</param>
        /// <param name="b">The second stringe.</param>
        /// <returns></returns>
        public static bool operator !=(Stringe a, Stringe b)
        {
            if (Equals(a, null) && Equals(b, null)) return false;
            if (Equals(a, null) || Equals(b, null)) return true;
            return a._stref != b._stref || a.Value != b.Value;
        }

        /// <summary>
        /// Determines whether the current stringe is equal to the specified object.
        /// </summary>
        /// <param name="obj">The object to compare with.</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var stre = obj as Stringe;
            if (stre == null) return false;
            return this == stre;
        }

        /// <summary>
        /// Returns the hash of the current stringe.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => StringeUtils.HashOf(_stref.String, _offset, _length);

        /// <summary>
        /// Returns the string value of the stringe.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Value;

        /// <summary>
        /// Stores cached character data for a Stringe.
        /// </summary>
        private class Stref
        {
            public readonly string String;
            public readonly Chare[] Chares;
            public readonly bool[] Bases;

            public Stref(string str)
            {
                String = str;
                Chares = new Chare[str.Length];
                Bases = new bool[str.Length];
                var elems = StringInfo.GetTextElementEnumerator(str);
                while (elems.MoveNext())
                {
                    Bases[elems.ElementIndex] = true;
                }
            }
        }

        private IEnumerable<Chare> _Chares()
        {
            for (int i = 0; i < _length; i++)
            {
                yield return this[i];
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the characteres in the stringe.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Chare> GetEnumerator()
        {
            return _Chares().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
