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
using System.Globalization;
using System.Linq;
using System.Text;

using Rant.Core.Compiler.Syntax;
using Rant.Core.Constructs;
using Rant.Core.Utilities;

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

		/// <summary>
		/// True
		/// </summary>
		public static readonly RantObject True = new RantObject(true);

		/// <summary>
		/// False
		/// </summary>
		public static readonly RantObject False = new RantObject(false);

		private object _value;

		/// <summary>
		/// Creates a null object.
		/// </summary>
		public RantObject()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RantObject"/> class from the specified object.
		/// </summary>
		/// <param name="o">The object to store in the <see cref="RantObject"/> instance.</param>
		public RantObject(object o)
		{

			switch (o)
			{
				case null:
					Type = RantObjectType.Null;
					break;
				case List<RantObject> list:
					_value = list;
					Type = RantObjectType.List;
					break;
				case bool b:
					_value = b;
					Type = RantObjectType.Boolean;
					break;
				case string str:
					_value = str;
					Type = RantObjectType.String;
					break;
				case byte _:
				case sbyte _:
				case short _:
				case ushort _:
				case int _:
				case uint _:
				case long _:
				case ulong _:
				case float _:
				case double _:
				case decimal _:
					_value = Convert.ChangeType(o, typeof(double));
					Type = RantObjectType.Number;
					break;
				case RST rst:
					_value = rst;
					Type = RantObjectType.Action;
					break;
				case Subroutine sub:
					_value = sub;
					Type = RantObjectType.Subroutine;
					break;
				case Array arr:
					_value = arr.OfType<object>().Select(obj => new RantObject(obj)).ToList();
					Type = RantObjectType.List;
					break;
				default:
					Type = RantObjectType.Undefined;
					break;
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
		public RantObjectType Type { get; private set; } = RantObjectType.Null;

		/// <summary>
		/// The value of the object.
		/// </summary>
		public object Value => _value;

		internal object PrintableValue
		{
			get
			{
				switch (Type)
				{
					case RantObjectType.Null:
						return null;
					case RantObjectType.Boolean:
					case RantObjectType.Number:
					case RantObjectType.String:
						return _value;
					default:
						return ToString();
				}
			}
		}

		/// <summary>
		/// Gets the length of the object. For strings, this is the character count. For lists, this is the item count. For all other types, -1 is returned.
		/// </summary>
		public int Length
		{
			get
			{
				switch (_value)
				{
					case List<RantObject> lst:
						return lst.Count;
					case string str:
						return str.Length;
					default:
						return -1;
				}
			}
		}

		/// <summary>
		/// Gets or sets the object at the specified index in the object.
		/// Only works with list objects.
		/// </summary>
		/// <param name="index">The index of the item to access.</param>
		/// <returns></returns>
		public RantObject this[int index]
		{
			get
			{
				if (Type != RantObjectType.List) return null;
				var list = _value as List<RantObject>;
				return index >= 0 && index < list.Count ? list[index] : null;
			}
			set
			{
				if (Type != RantObjectType.List) return;
				var list = _value as List<RantObject>;
				if (index >= 0 && index < list.Count) list[index] = value;
			}
		}

		/// <summary>
		/// Converts the current object to a RantObject of the specified type and returns it.
		/// </summary>
		/// <param name="targetType">The object type to convert to.</param>
		/// <returns></returns>
		public RantObject ConvertTo(RantObjectType targetType)
		{
			if (Type == targetType) return Clone();

			switch (targetType)
			{
				case RantObjectType.String:
					{
						switch (_value)
						{
							case bool b:
								return new RantObject(b.ToString());
							case double d:
								return new RantObject(d.ToString(CultureInfo.InvariantCulture));
							case List<RantObject> lst:
								{
									var sb = new StringBuilder();
									bool first = true;
									sb.Append("(");
									foreach (var rantObject in lst)
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
						switch (_value)
						{
							case bool b:
								return new RantObject(b ? 1 : 0);
							case string str:
								{
									return Util.ParseDouble(str, out double num) ? new RantObject(num) : Null;
								}
						}
						break;
					}
				case RantObjectType.Boolean:
					{
						switch (_value)
						{
							case string s:
								{
									switch (s)
									{
										case "True":
											return new RantObject(true);
										case "False":
											return new RantObject(false);
									}
									break;
								}
							case double d:
								// ReSharper disable once CompareOfFloatsByEqualityOperator
								return new RantObject(d != 0.0);
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
			object clonedValue;
			switch (_value)
			{
				case List<RantObject> list:
					clonedValue = list.ToList();
					break;
				default:
					clonedValue = _value;
					break;
			}

			return new RantObject
			{
				_value = clonedValue,
				Type = Type
			};
		}

		#region Operators

		/// <summary>
		/// Gets the boolean inverse of a RantObject.
		/// </summary>
		/// <param name="a">The object to invert from.</param>
		/// <returns></returns>
		public static RantObject operator !(RantObject a)
		{
			switch (a._value)
			{
				case bool b:
					return new RantObject(!b);
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
			switch (a._value) // TODO: Cover all cases
			{
				case double numA:
					{
						switch (b._value)
						{
							case double numB:
								return new RantObject(numA + numB);
						}
						break;
					}
				case string strA:
					{
						switch (b._value)
						{
							case double numB:
								return new RantObject(strA + numB.ToString(CultureInfo.InvariantCulture));
							case string strB:
								return new RantObject(strA + strB);
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
			switch (a._value)
			{
				case double numA:
					{
						switch (b._value)
						{
							case double numB:
								return new RantObject(numA - numB);
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
			switch (a._value)
			{
				case double numA:
					{
						switch (b._value)
						{
							case double numB:
								return new RantObject(numA * numB);
						}
						break;
					}
				case string strA:
					{
						switch (b._value)
						{
							case double numB:
								{
									var sb = new StringBuilder();
									int c = (int)numB;
									for (int i = 0; i < c; i++)
										sb.Append(strA);
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
			switch (a._value)
			{
				case double numA:
					{
						switch (b._value)
						{
							case double numB:
								return new RantObject(numA / numB);
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
			switch (a._value)
			{
				case double numA:
					{
						switch (b._value)
						{
							case double numB:
								return new RantObject(numA % numB);
						}
						break;
					}
			}

			return Null;
		}

		#endregion

		/// <summary>
		/// Returns a string representation of the current RantObject.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			if (Type == RantObjectType.Undefined) return "???";

			switch (_value)
			{
				case bool b:
					return b ? "true" : "false";
				case string str:
					return str;
				case null:
					return "null";
				case double d:
					return d.ToString(CultureInfo.InvariantCulture);
				case List<RantObject> list:
					{
						var sb = new StringBuilder();
						bool first = true;
						sb.Append("(");
						foreach (var rantObject in list)
						{
							if (!first) sb.Append(", ");
							first = false;
							sb.Append(rantObject);
						}
						sb.Append(")");
						return sb.ToString();
					}
				case Subroutine sub:
					return sub.Name;
			}

			return Value.ToString();
		}
	}
}