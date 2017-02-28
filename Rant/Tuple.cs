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

using static Rant.Core.Utilities.Util;

namespace Rant
{
	/// <summary>
	/// Makes tuples for your pleasure.
	/// </summary>
	public static class _
	{
		/// <summary>
		/// Makes Item1 1-tuple.
		/// </summary>
		/// <typeparam name="A">First type.</typeparam>
		/// <param name="Item1">First value.</param>
		/// <returns></returns>
		public static _<A> Create<A>(A Item1) => new _<A>(Item1);

		/// <summary>
		/// Makes Item1 2-tuple.
		/// </summary>
		/// <typeparam name="A">First type.</typeparam>
		/// <typeparam name="B">Second type.</typeparam>
		/// <param name="Item1">First value.</param>
		/// <param name="Item2">Second value.</param>
		/// <returns></returns>
		public static _<A, B> Create<A, B>(A Item1, B Item2) => new _<A, B>(Item1, Item2);

		/// <summary>
		/// Makes Item1 3-tuple.
		/// </summary>
		/// <typeparam name="A">First type.</typeparam>
		/// <typeparam name="B">Second type.</typeparam>
		/// <typeparam name="C">Third type.</typeparam>
		/// <param name="Item1">First value.</param>
		/// <param name="Item2">Second value.</param>
		/// <param name="Item3">Third value.</param>
		/// <returns></returns>
		public static _<A, B, C> Create<A, B, C>(A Item1, B Item2, C Item3) => new _<A, B, C>(Item1, Item2, Item3);

		/// <summary>
		/// Makes Item1 4-tuple.
		/// </summary>
		/// <typeparam name="A">First type.</typeparam>
		/// <typeparam name="B">Second type.</typeparam>
		/// <typeparam name="C">Third type.</typeparam>
		/// <typeparam name="D">Fourth type.</typeparam>
		/// <param name="Item1">First value.</param>
		/// <param name="Item2">Second value.</param>
		/// <param name="Item3">Third value.</param>
		/// <param name="Item4">Fourth value.</param>
		/// <returns></returns>
		public static _<A, B, C, D> Create<A, B, C, D>(A Item1, B Item2, C Item3, D Item4)
			=> new _<A, B, C, D>(Item1, Item2, Item3, Item4);

		/// <summary>
		/// Makes Item1 5-tuple.
		/// </summary>
		/// <typeparam name="A">First type.</typeparam>
		/// <typeparam name="B">Second type.</typeparam>
		/// <typeparam name="C">Third type.</typeparam>
		/// <typeparam name="D">Fourth type.</typeparam>
		/// <typeparam name="E">Fifth type.</typeparam>
		/// <param name="Item1">First value.</param>
		/// <param name="Item2">Second value.</param>
		/// <param name="Item3">Third value.</param>
		/// <param name="Item4">Fourth value.</param>
		/// <param name="Item5">Fifth value.</param>
		/// <returns></returns>
		public static _<A, B, C, D, E> Create<A, B, C, D, E>(A Item1, B Item2, C Item3, D Item4, E Item5)
			=> new _<A, B, C, D, E>(Item1, Item2, Item3, Item4, Item5);

		/// <summary>
		/// Makes Item1 6-tuple.
		/// </summary>
		/// <typeparam name="A">First type.</typeparam>
		/// <typeparam name="B">Second type.</typeparam>
		/// <typeparam name="C">Third type.</typeparam>
		/// <typeparam name="D">Fourth type.</typeparam>
		/// <typeparam name="E">Fifth type.</typeparam>
		/// <typeparam name="F">Sixth type.</typeparam>
		/// <param name="Item1">First value.</param>
		/// <param name="Item2">Second value.</param>
		/// <param name="Item3">Third value.</param>
		/// <param name="Item4">Fourth value.</param>
		/// <param name="Item5">Fifth value.</param>
		/// <param name="Item6">Sixth value.</param>
		/// <returns></returns>
		public static _<A, B, C, D, E, F> Create<A, B, C, D, E, F>(A Item1, B Item2, C Item3, D Item4, E Item5, F Item6)
			=> new _<A, B, C, D, E, F>(Item1, Item2, Item3, Item4, Item5, Item6);

		/// <summary>
		/// Makes Item1 7-tuple.
		/// </summary>
		/// <typeparam name="A">First type.</typeparam>
		/// <typeparam name="B">Second type.</typeparam>
		/// <typeparam name="C">Third type.</typeparam>
		/// <typeparam name="D">Fourth type.</typeparam>
		/// <typeparam name="E">Fifth type.</typeparam>
		/// <typeparam name="F">Sixth type.</typeparam>
		/// <typeparam name="G">Seventh type.</typeparam>
		/// <param name="Item1">First value.</param>
		/// <param name="Item2">Second value.</param>
		/// <param name="Item3">Third value.</param>
		/// <param name="Item4">Fourth value.</param>
		/// <param name="Item5">Fifth value.</param>
		/// <param name="Item6">Sixth value.</param>
		/// <param name="Item7">Seventh value.</param>
		/// <returns></returns>
		public static _<A, B, C, D, E, F, G> Create<A, B, C, D, E, F, G>(A Item1, B Item2, C Item3, D Item4, E Item5, F Item6,
			G Item7) => new _<A, B, C, D, E, F, G>(Item1, Item2, Item3, Item4, Item5, Item6, Item7);

		/// <summary>
		/// Makes an 8-tuple.
		/// </summary>
		/// <typeparam name="A">First type.</typeparam>
		/// <typeparam name="B">Second type.</typeparam>
		/// <typeparam name="C">Third type.</typeparam>
		/// <typeparam name="D">Fourth type.</typeparam>
		/// <typeparam name="E">Fifth type.</typeparam>
		/// <typeparam name="F">Sixth type.</typeparam>
		/// <typeparam name="G">Seventh type.</typeparam>
		/// <typeparam name="H">Eighth type.</typeparam>
		/// <param name="Item1">First value.</param>
		/// <param name="Item2">Second value.</param>
		/// <param name="Item3">Third value.</param>
		/// <param name="Item4">Fourth value.</param>
		/// <param name="Item5">Fifth value.</param>
		/// <param name="Item6">Sixth value.</param>
		/// <param name="Item7">Seventh value.</param>
		/// <param name="Item8">Eighth value.</param>
		/// <returns></returns>
		public static _<A, B, C, D, E, F, G, H> Create<A, B, C, D, E, F, G, H>(A Item1, B Item2, C Item3, D Item4, E Item5,
			F Item6, G Item7, H Item8) => new _<A, B, C, D, E, F, G, H>(Item1, Item2, Item3, Item4, Item5, Item6, Item7, Item8);
	}

	/// <summary>
	/// 1-tuple.
	/// </summary>
	/// <typeparam name="A">First type.</typeparam>
	public sealed class _<A>
	{
		/// <summary>
		/// The first item.
		/// </summary>
		public readonly A Item1;

		internal _(A Item1)
		{
			this.Item1 = Item1;
		}

		/// <summary>
		/// Returns a string representation of the tuple's contents.
		/// </summary>
		/// <returns></returns>
		public override string ToString() => $"{{Item1: {Item1}}}";

		/// <summary>
		/// Returns a hash of the tuple's contents.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode() => Item1.GetHashCode();

		/// <summary>
		/// Determines whether the current instance is equal to the specified object.
		/// </summary>
		/// <param name="obj">The object to test.</param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			var t = obj as _<A>;
			if (t == null) return false;
			return Equals(t.Item1, Item1);
		}
	}

	/// <summary>
	/// 2-tuple.
	/// </summary>
	/// <typeparam name="A">First type.</typeparam>
	/// <typeparam name="B">Second type.</typeparam>
	public sealed class _<A, B>
	{
		/// <summary>
		/// The first item.
		/// </summary>
		public readonly A Item1;

		/// <summary>
		/// The second item.
		/// </summary>
		public readonly B Item2;

		internal _(A Item1, B Item2)
		{
			this.Item1 = Item1;
			this.Item2 = Item2;
		}

		/// <summary>
		/// Returns a string representation of the tuple's contents.
		/// </summary>
		/// <returns></returns>
		public override string ToString() => $"{{Item1: {Item1}, Item2: {Item2}}}";

		/// <summary>
		/// Returns a hash of the tuple's contents.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode() => HashOf(Item1, Item2);

		/// <summary>
		/// Determines whether the current instance is equal to the specified object.
		/// </summary>
		/// <param name="obj">The object to test.</param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			var t = obj as _<A, B>;
			if (t == null) return false;
			return Equals(t.Item1, Item1) && Equals(t.Item2, Item2);
		}
	}

	/// <summary>
	/// 3-tuple.
	/// </summary>
	/// <typeparam name="A">First type.</typeparam>
	/// <typeparam name="B">Second type.</typeparam>
	/// <typeparam name="C">Third type.</typeparam>
	public sealed class _<A, B, C>
	{
		/// <summary>
		/// The first item.
		/// </summary>
		public readonly A Item1;

		/// <summary>
		/// The second item.
		/// </summary>
		public readonly B Item2;

		/// <summary>
		/// The third item.
		/// </summary>
		public readonly C Item3;

		internal _(A Item1, B Item2, C Item3)
		{
			this.Item1 = Item1;
			this.Item2 = Item2;
			this.Item3 = Item3;
		}

		/// <summary>
		/// Returns a string representation of the tuple's contents.
		/// </summary>
		/// <returns></returns>
		public override string ToString() => $"{{Item1: {Item1}, Item2: {Item2}, Item3: {Item3}}}";

		/// <summary>
		/// Returns a hash of the tuple's contents.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode() => HashOf(Item1, Item2, Item3);

		/// <summary>
		/// Determines whether the current instance is equal to the specified object.
		/// </summary>
		/// <param name="obj">The object to test.</param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			var t = obj as _<A, B, C>;
			if (t == null) return false;
			return Equals(t.Item1, Item1) && Equals(t.Item2, Item2) && Equals(t.Item3, Item3);
		}
	}

	/// <summary>
	/// 4-tuple.
	/// </summary>
	/// <typeparam name="A">First type.</typeparam>
	/// <typeparam name="B">Second type.</typeparam>
	/// <typeparam name="C">Third type.</typeparam>
	/// <typeparam name="D">Fourth type.</typeparam>
	public sealed class _<A, B, C, D>
	{
		/// <summary>
		/// The first item.
		/// </summary>
		public readonly A Item1;

		/// <summary>
		/// The second item.
		/// </summary>
		public readonly B Item2;

		/// <summary>
		/// The third item.
		/// </summary>
		public readonly C Item3;

		/// <summary>
		/// The fourth item.
		/// </summary>
		public readonly D Item4;

		internal _(A Item1, B Item2, C Item3, D Item4)
		{
			this.Item1 = Item1;
			this.Item2 = Item2;
			this.Item3 = Item3;
			this.Item4 = Item4;
		}

		/// <summary>
		/// Returns a string representation of the tuple's contents.
		/// </summary>
		/// <returns></returns>
		public override string ToString() => $"{{Item1: {Item1}, Item2: {Item2}, Item3: {Item3}, Item4: {Item4}}}";

		/// <summary>
		/// Returns a hash of the tuple's contents.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode() => HashOf(Item1, Item2, Item3, Item4);

		/// <summary>
		/// Determines whether the current instance is equal to the specified object.
		/// </summary>
		/// <param name="obj">The object to test.</param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			var t = obj as _<A, B, C, D>;
			if (t == null) return false;
			return Equals(t.Item1, Item1) && Equals(t.Item2, Item2) && Equals(t.Item3, Item3) && Equals(t.Item4, Item4);
		}
	}

	/// <summary>
	/// 5-tuple.
	/// </summary>
	/// <typeparam name="A">First type.</typeparam>
	/// <typeparam name="B">Second type.</typeparam>
	/// <typeparam name="C">Third type.</typeparam>
	/// <typeparam name="D">Fourth type.</typeparam>
	/// <typeparam name="E">Fifth type.</typeparam>
	public sealed class _<A, B, C, D, E>
	{
		/// <summary>
		/// The first item.
		/// </summary>
		public readonly A Item1;

		/// <summary>
		/// The second item.
		/// </summary>
		public readonly B Item2;

		/// <summary>
		/// The third item.
		/// </summary>
		public readonly C Item3;

		/// <summary>
		/// The fourth item.
		/// </summary>
		public readonly D Item4;

		/// <summary>
		/// The fifth item.
		/// </summary>
		public readonly E Item5;

		internal _(A Item1, B Item2, C Item3, D Item4, E Item5)
		{
			this.Item1 = Item1;
			this.Item2 = Item2;
			this.Item3 = Item3;
			this.Item4 = Item4;
			this.Item5 = Item5;
		}

		/// <summary>
		/// Returns a string representation of the tuple's contents.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
			=> $"{{Item1: {Item1}, Item2: {Item2}, Item3: {Item3}, Item4: {Item4}, Item5: {Item5}}}";

		/// <summary>
		/// Returns a hash of the tuple's contents.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode() => HashOf(Item1, Item2, Item3, Item4, Item5);

		/// <summary>
		/// Determines whether the current instance is equal to the specified object.
		/// </summary>
		/// <param name="obj">The object to test.</param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			var t = obj as _<A, B, C, D, E>;
			if (t == null) return false;
			return Equals(t.Item1, Item1) && Equals(t.Item2, Item2) && Equals(t.Item3, Item3) && Equals(t.Item4, Item4) &&
			       Equals(t.Item5, Item5);
		}
	}

	/// <summary>
	/// 6-tuple.
	/// </summary>
	/// <typeparam name="A">First type.</typeparam>
	/// <typeparam name="B">Second type.</typeparam>
	/// <typeparam name="C">Third type.</typeparam>
	/// <typeparam name="D">Fourth type.</typeparam>
	/// <typeparam name="E">Fifth type.</typeparam>
	/// <typeparam name="F">Sixth type.</typeparam>
	public sealed class _<A, B, C, D, E, F>
	{
		/// <summary>
		/// The first item.
		/// </summary>
		public readonly A Item1;

		/// <summary>
		/// The second item.
		/// </summary>
		public readonly B Item2;

		/// <summary>
		/// The third item.
		/// </summary>
		public readonly C Item3;

		/// <summary>
		/// The fourth item.
		/// </summary>
		public readonly D Item4;

		/// <summary>
		/// The fifth item.
		/// </summary>
		public readonly E Item5;

		/// <summary>
		/// The sixth item.
		/// </summary>
		public readonly F Item6;

		internal _(A Item1, B Item2, C Item3, D Item4, E Item5, F Item6)
		{
			this.Item1 = Item1;
			this.Item2 = Item2;
			this.Item3 = Item3;
			this.Item4 = Item4;
			this.Item5 = Item5;
			this.Item6 = Item6;
		}

		/// <summary>
		/// Returns a string representation of the tuple's contents.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
			=> $"{{Item1: {Item1}, Item2: {Item2}, Item3: {Item3}, Item4: {Item4}, Item5: {Item5}, Item6: {Item6}}}";

		/// <summary>
		/// Returns a hash of the tuple's contents.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode() => HashOf(Item1, Item2, Item3, Item4, Item5, Item6);

		/// <summary>
		/// Determines whether the current instance is equal to the specified object.
		/// </summary>
		/// <param name="obj">The object to test.</param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			var t = obj as _<A, B, C, D, E, F>;
			if (t == null) return false;
			return Equals(t.Item1, Item1) && Equals(t.Item2, Item2) && Equals(t.Item3, Item3) && Equals(t.Item4, Item4) &&
			       Equals(t.Item5, Item5) && Equals(t.Item6, Item6);
		}
	}

	/// <summary>
	/// 7-tuple.
	/// </summary>
	/// <typeparam name="A">First type.</typeparam>
	/// <typeparam name="B">Second type.</typeparam>
	/// <typeparam name="C">Third type.</typeparam>
	/// <typeparam name="D">Fourth type.</typeparam>
	/// <typeparam name="E">Fifth type.</typeparam>
	/// <typeparam name="F">Sixth type.</typeparam>
	/// <typeparam name="G">Seventh type.</typeparam>
	public sealed class _<A, B, C, D, E, F, G>
	{
		/// <summary>
		/// The first item.
		/// </summary>
		public readonly A Item1;

		/// <summary>
		/// The second item.
		/// </summary>
		public readonly B Item2;

		/// <summary>
		/// The third item.
		/// </summary>
		public readonly C Item3;

		/// <summary>
		/// The fourth item.
		/// </summary>
		public readonly D Item4;

		/// <summary>
		/// The fifth item.
		/// </summary>
		public readonly E Item5;

		/// <summary>
		/// The sixth item.
		/// </summary>
		public readonly F Item6;

		/// <summary>
		/// The seventh item.
		/// </summary>
		public readonly G Item7;

		internal _(A Item1, B Item2, C Item3, D Item4, E Item5, F Item6, G Item7)
		{
			this.Item1 = Item1;
			this.Item2 = Item2;
			this.Item3 = Item3;
			this.Item4 = Item4;
			this.Item5 = Item5;
			this.Item6 = Item6;
			this.Item7 = Item7;
		}

		/// <summary>
		/// Returns a string representation of the tuple's contents.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
			=>
				$"{{Item1: {Item1}, Item2: {Item2}, Item3: {Item3}, Item4: {Item4}, Item5: {Item5}, Item6: {Item6}, Item7: {Item7}}}";

		/// <summary>
		/// Returns a hash of the tuple's contents.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode() => HashOf(Item1, Item2, Item3, Item4, Item5, Item6, Item7);

		/// <summary>
		/// Determines whether the current instance is equal to the specified object.
		/// </summary>
		/// <param name="obj">The object to test.</param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			var t = obj as _<A, B, C, D, E, F, G>;
			if (t == null) return false;
			return Equals(t.Item1, Item1) && Equals(t.Item2, Item2) && Equals(t.Item3, Item3) && Equals(t.Item4, Item4) &&
			       Equals(t.Item5, Item5) && Equals(t.Item6, Item6) && Equals(t.Item7, Item7);
		}
	}

	/// <summary>
	/// 8-tuple.
	/// </summary>
	/// <typeparam name="A">First type.</typeparam>
	/// <typeparam name="B">Second type.</typeparam>
	/// <typeparam name="C">Third type.</typeparam>
	/// <typeparam name="D">Fourth type.</typeparam>
	/// <typeparam name="E">Fifth type.</typeparam>
	/// <typeparam name="F">Sixth type.</typeparam>
	/// <typeparam name="G">Seventh type.</typeparam>
	/// <typeparam name="H">Eighth tuple.</typeparam>
	public sealed class _<A, B, C, D, E, F, G, H>
	{
		/// <summary>
		/// The first item.
		/// </summary>
		public readonly A Item1;

		/// <summary>
		/// The second item.
		/// </summary>
		public readonly B Item2;

		/// <summary>
		/// The third item.
		/// </summary>
		public readonly C Item3;

		/// <summary>
		/// The fourth item.
		/// </summary>
		public readonly D Item4;

		/// <summary>
		/// The fifth item.
		/// </summary>
		public readonly E Item5;

		/// <summary>
		/// The sixth item.
		/// </summary>
		public readonly F Item6;

		/// <summary>
		/// The seventh item.
		/// </summary>
		public readonly G Item7;

		/// <summary>
		/// The eighth item.
		/// </summary>
		public readonly H Item8;

		internal _(A Item1, B Item2, C Item3, D Item4, E Item5, F Item6, G Item7, H Item8)
		{
			this.Item1 = Item1;
			this.Item2 = Item2;
			this.Item3 = Item3;
			this.Item4 = Item4;
			this.Item5 = Item5;
			this.Item6 = Item6;
			this.Item7 = Item7;
			this.Item8 = Item8;
		}

		/// <summary>
		/// Returns a string representation of the tuple's contents.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
			=>
				$"{{Item1: {Item1}, Item2: {Item2}, Item3: {Item3}, Item4: {Item4}, Item5: {Item5}, Item6: {Item6}, Item7: {Item7}, Item8: {Item8}}}";

		/// <summary>
		/// Returns a hash of the tuple's contents.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode() => HashOf(Item1, Item2, Item3, Item4, Item5, Item6, Item7, Item8);

		/// <summary>
		/// Determines whether the current instance is equal to the specified object.
		/// </summary>
		/// <param name="obj">The object to test.</param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			var t = obj as _<A, B, C, D, E, F, G, H>;
			if (t == null) return false;
			return Equals(t.Item1, Item1) && Equals(t.Item2, Item2) && Equals(t.Item3, Item3) && Equals(t.Item4, Item4) &&
			       Equals(t.Item5, Item5) && Equals(t.Item6, Item6) && Equals(t.Item7, Item7) && Equals(t.Item8, Item8);
		}
	}
}