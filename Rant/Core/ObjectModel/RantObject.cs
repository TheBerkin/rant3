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

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using Rant.Core.Compiler.Syntax;

namespace Rant.Core.ObjectModel
{
    /// <summary>
    /// Represents a Rant variable.
    /// </summary>
    public sealed class RantObject
    {
        /// <summary>
        /// Null
        /// </summary>
        public static readonly RantObject Null = new RantObject();

        private bool _boolean = false;
        private List<RantObject> _list = null;
        private double _number = 0;
        private RantProgram _pattern = null;
        private RST _rst = null;
        private string _string = null;

        /// <summary>
        /// Creates a No object.
        /// </summary>
        public RantObject()
        {
        }

        /// <summary>
        /// Creates a new RantObject instance with a list value.
        /// </summary>
        /// <param name="list">The list to assign to the object.</param>
        public RantObject(List<RantObject> list)
        {
            if (list == null) return;
            Type = RantObjectType.List;
            _list = list;
        }

        /// <summary>
        /// Creates a new RantObject instance with a boolean value.
        /// </summary>
        /// <param name="boolean">The boolean value to assign to the object.</param>
        public RantObject(bool boolean)
        {
            Type = RantObjectType.Boolean;
            _boolean = boolean;
        }

        /// <summary>
        /// Creates a new RantObject instance with a string value.
        /// </summary>
        /// <param name="str">The string to assign to the object.</param>
        public RantObject(string str)
        {
            if (str == null) return;
            Type = RantObjectType.String;
            _string = str;
        }

        /// <summary>
        /// Creates a new RantObject instance with a decimal number value.
        /// </summary>
        /// <param name="num">The number to assign to the object.</param>
        public RantObject(double num)
        {
            Type = RantObjectType.Number;
            _number = num;
        }

        internal RantObject(RST rst)
        {
            Type = RantObjectType.Action;
            _rst = rst;
        }

        /// <summary>
        /// Creates a new RantObject instance from the specified object.
        /// </summary>
        /// <param name="obj">The value to assign to the object.</param>
        public RantObject(object obj)
        {
            if (obj == null) return;

            if (obj is string)
            {
                _string = obj.ToString();
                Type = RantObjectType.String;
            }
            else if (obj is bool)
            {
                _boolean = (bool)obj;
                Type = RantObjectType.Boolean;
            }
            else if (IsNumber(obj))
            {
                _number = (double)obj;
                Type = RantObjectType.Number;
            }
            else if (obj is List<RantObject>)
            {
                _list = (List<RantObject>)obj;
                Type = RantObjectType.List;
            }
            else if (obj.GetType().IsArray)
            {
                _list = ((object[])obj).Select(o => new RantObject(o)).ToList();
                Type = RantObjectType.List;
            }
            else if (obj is RantProgram)
                _pattern = (RantProgram)obj;
            else if (obj is RST)
            {
                _rst = (RST)obj;
                Type = RantObjectType.Action;
            }
        }

        /// <summary>
        /// Creates a new RantObject with the specified object type and a default value.
        /// </summary>
        /// <param name="type">The type of object to create.</param>
        public RantObject(RantObjectType type)
        {
            Type = type;
        }

        /// <summary>
        /// The type of the object.
        /// </summary>
        public RantObjectType Type { get; internal set; } = RantObjectType.Null;

        /// <summary>
        /// The value of the object.
        /// </summary>
        public object Value
        {
            get
            {
                switch (Type)
                {
                    case RantObjectType.Null:
                        return null;
                    case RantObjectType.Boolean:
                        return _boolean;
                    case RantObjectType.Number:
                        return _number;
                    case RantObjectType.Pattern:
                        return _pattern;
                    case RantObjectType.String:
                        return _string;
                    case RantObjectType.List:
                        return _list;
                    case RantObjectType.Action:
                        return _rst;
                }
                return null;
            }
        }

        /// <summary>
        /// Converts the current object to a RantObject of the specified type and returns it.
        /// </summary>
        /// <param name="type">The object type to convert to.</param>
        /// <returns></returns>
        public RantObject ConvertTo(RantObjectType type)
        {
            if (Type == type) return Clone();

            switch (type)
            {
                case RantObjectType.String:
                {
                    switch (Type)
                    {
                        case RantObjectType.Boolean:
                            return new RantObject(_boolean.ToString());
                        case RantObjectType.Number:
                            return new RantObject(_number.ToString(CultureInfo.InvariantCulture));
                        case RantObjectType.Pattern:
                            return new RantObject(_pattern.Code);
                        case RantObjectType.List:
                        {
                            var sb = new StringBuilder();
                            bool first = true;
                            sb.Append("(");
                            foreach (var rantObject in _list)
                            {
                                if (first)
                                {
                                    first = false;
                                    sb.Append(", ");
                                }

                                sb.Append(rantObject);
                            }
                            sb.Append(")");
                            return new RantObject(sb.ToString());
                        }
                    }
                    break;
                }
                case RantObjectType.Number:
                {
                    switch (Type)
                    {
                        case RantObjectType.Boolean:
                            return new RantObject(_boolean ? 1 : 0);
                        case RantObjectType.String:
                        {
							return double.TryParse(_string, out double num) ? new RantObject(num) : Null;
						}
                    }
                    break;
                }
                case RantObjectType.Boolean:
                {
                    switch (Type)
                    {
                        case RantObjectType.Number:
                            return new RantObject(_number != 0);
                        case RantObjectType.String:
                        {
                            string bstr = _string.ToLower().Trim();
                            switch (bstr)
                            {
                                case "true":
                                    return new RantObject(true);
                                case "false":
                                    return new RantObject(false);
                            }
                            break;
                        }
                    }
                    break;
                }
                case RantObjectType.List:
                {
                    return new RantObject(new List<RantObject> { this });
                }
            }

            return Null;
        }

        /// <summary>
        /// Returns another RantObject instance with the exact same value as the current instance.
        /// </summary>
        /// <returns></returns>
        public RantObject Clone()
        {
            return new RantObject
            {
                _boolean = _boolean,
                _list = _list?.ToList(), // Create a copy of the list
                _number = _number,
                _pattern = _pattern,
                _string = _string,
                _rst = _rst,
                Type = Type
            };
        }

        /// <summary>
        /// Gets the boolean inverse of a RantObject.
        /// </summary>
        /// <param name="a">The object to invert from.</param>
        /// <returns></returns>
        public static RantObject operator !(RantObject a)
        {
            switch (a.Type)
            {
                case RantObjectType.Number:
                    {
                        switch (a.Type)
                        {
                            case RantObjectType.Boolean:
                                return new RantObject(!a._boolean);
                        }
                        break;
                    }
            }

            return Null;
        }

        /// <summary>
        /// Returns the sum of two RantObjects.
        /// </summary>
        /// <param name="a">The first object.</param>
        /// <param name="b">The second object.</param>
        /// <returns></returns>
        public static RantObject operator +(RantObject a, RantObject b)
        {
            switch (a.Type) // TODO: Cover all cases
            {
                case RantObjectType.Number:
                {
                    switch (b.Type)
                    {
                        case RantObjectType.Number:
                            return new RantObject(a._number + b._number);
                    }
                    break;
                }
                case RantObjectType.String:
                {
                    switch (b.Type)
                    {
                        case RantObjectType.Number:
                            return new RantObject(a._string + b._number);
                        case RantObjectType.String:
                            return new RantObject(a._string + b._string);
                    }
                    break;
                }
            }

            return Null;
        }

        /// <summary>
        /// Subtracts a RantObject from another.
        /// </summary>
        /// <param name="a">The object to subtract from.</param>
        /// <param name="b">The object to subtract.</param>
        /// <returns></returns>
        public static RantObject operator -(RantObject a, RantObject b)
        {
            switch (a.Type)
            {
                case RantObjectType.Number:
                {
                    switch (b.Type)
                    {
                        case RantObjectType.Number:
                            return new RantObject(a._number - b._number);
                    }
                    break;
                }
            }

            return Null;
        }

        /// <summary>
        /// Returns the product of two RantObjects.
        /// </summary>
        /// <param name="a">The first object.</param>
        /// <param name="b">The second object.</param>
        /// <returns></returns>
        public static RantObject operator *(RantObject a, RantObject b)
        {
            switch (a.Type)
            {
                case RantObjectType.Number:
                {
                    switch (b.Type)
                    {
                        case RantObjectType.Number:
                            return new RantObject(a._number * b._number);
                    }
                    break;
                }
                case RantObjectType.String:
                {
                    switch (b.Type)
                    {
                        case RantObjectType.Number:
                        {
                            var sb = new StringBuilder();
                            int c = (int)b._number;
                            for (int i = 0; i < c; i++)
                                sb.Append(a._string);
                            return new RantObject(sb.ToString());
                        }
                    }
                    break;
                }
            }

            return Null;
        }

        /// <summary>
        /// Divides one RantObject by another.
        /// </summary>
        /// <param name="a">The object to divide.</param>
        /// <param name="b">The object to divide by.</param>
        /// <returns></returns>
        public static RantObject operator /(RantObject a, RantObject b)
        {
            switch (a.Type)
            {
                case RantObjectType.Number:
                {
                    switch (b.Type)
                    {
                        case RantObjectType.Number:
                            return new RantObject(a._number / b._number);
                    }
                    break;
                }
            }

            return Null;
        }

        /// <summary>
        /// Mods one RantObject by another.
        /// </summary>
        /// <param name="a">The object to mod.</param>
        /// <param name="b">The object to mod by.</param>
        /// <returns></returns>
        public static RantObject operator %(RantObject a, RantObject b)
        {
            switch (a.Type)
            {
                case RantObjectType.Number:
                    {
                        switch (b.Type)
                        {
                            case RantObjectType.Number:
                                return new RantObject(a._number % b._number);
                        }
                        break;
                    }
            }

            return Null;
        }

        /// <summary>
        /// Returns a string representation of the current RantObject.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            switch (Type)
            {
                case RantObjectType.Boolean:
                    return _boolean ? "true" : "false";
                case RantObjectType.String:
                    return _string;
                case RantObjectType.Null:
                    return "no";
                case RantObjectType.Undefined:
                    return "???";
                case RantObjectType.Number:
                    return _number.ToString(CultureInfo.InvariantCulture);
                case RantObjectType.Pattern:
                    return $"$\"{_pattern.Code}\"";
                case RantObjectType.List:
                {
                    var sb = new StringBuilder();
                    bool first = true;
                    sb.Append("(");
                    foreach (var rantObject in _list)
                    {
                        if (!first) sb.Append(", ");
                        first = false;
                        sb.Append(rantObject);
                    }
                    sb.Append(")");
                    return sb.ToString();
                }
            }
            return Value.ToString();
        }

        private static bool IsNumber(object value)
        {
            return value is sbyte
                   || value is byte
                   || value is short
                   || value is ushort
                   || value is int
                   || value is uint
                   || value is long
                   || value is ulong
                   || value is float
                   || value is double
                   || value is decimal;
        }
    }
}