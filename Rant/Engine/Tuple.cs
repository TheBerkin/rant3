using System;

using Rant.Engine.Util;

namespace Rant.Engine
{
    public static class Tuple
    {
        /// <summary>
        /// Makes Item1 1-tuple.
        /// </summary>
        /// <typeparam name="A">First type.</typeparam>
        /// <param name="Item1">First value.</param>
        /// <returns></returns>
        public static Tuple<A> Create<A>(A Item1) => new Tuple<A>(Item1);

        /// <summary>
        /// Makes Item1 2-tuple.
        /// </summary>
        /// <typeparam name="A">First type.</typeparam>
        /// <typeparam name="B">Second type.</typeparam>
        /// <param name="Item1">First value.</param>
        /// <param name="Item2">Second value.</param>
        /// <returns></returns>
        public static Tuple<A, B> Create<A, B>(A Item1, B Item2) => new Tuple<A, B>(Item1, Item2);

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
        public static Tuple<A, B, C> Create<A, B, C>(A Item1, B Item2, C Item3) => new Tuple<A, B, C>(Item1, Item2, Item3);

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
        public static Tuple<A, B, C, D> Create<A, B, C, D>(A Item1, B Item2, C Item3, D Item4) => new Tuple<A, B, C, D>(Item1, Item2, Item3, Item4);

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
        public static Tuple<A, B, C, D, E> Create<A, B, C, D, E>(A Item1, B Item2, C Item3, D Item4, E Item5) => new Tuple<A, B, C, D, E>(Item1, Item2, Item3, Item4, Item5);

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
        public static Tuple<A, B, C, D, E, F> Create<A, B, C, D, E, F>(A Item1, B Item2, C Item3, D Item4, E Item5, F Item6) => new Tuple<A, B, C, D, E, F>(Item1, Item2, Item3, Item4, Item5, Item6);

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
        public static Tuple<A, B, C, D, E, F, G> Create<A, B, C, D, E, F, G>(A Item1, B Item2, C Item3, D Item4, E Item5, F Item6, G Item7) => new Tuple<A, B, C, D, E, F, G>(Item1, Item2, Item3, Item4, Item5, Item6, Item7);

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
        public static Tuple<A, B, C, D, E, F, G, H> Create<A, B, C, D, E, F, G, H>(A Item1, B Item2, C Item3, D Item4, E Item5, F Item6, G Item7, H Item8) => new Tuple<A, B, C, D, E, F, G, H>(Item1, Item2, Item3, Item4, Item5, Item6, Item7, Item8);
    }

    /// <summary>
    /// 1-tuple.
    /// </summary>
    /// <typeparam name="A">First type.</typeparam>
    public sealed class Tuple<A>
    {
        public readonly A Item1;

        internal Tuple(A Item1)
        {
            this.Item1 = Item1;
        }

        public override string ToString() => "{Item1: \{Item1}}";

        public override int GetHashCode() => Item1.GetHashCode();

        public override bool Equals(object obj)
        {
            var t = obj as Tuple<A>;
            if (t == null) return false;
            return Equals(t.Item1, Item1);
        }
    }

    /// <summary>
    /// 2-tuple.
    /// </summary>
    /// <typeparam name="A">First type.</typeparam>
    /// <typeparam name="B">Second type.</typeparam>
    public sealed class Tuple<A, B>
    {
        public readonly A Item1;
        public readonly B Item2;

        internal Tuple(A Item1, B Item2)
        {
            this.Item1 = Item1;
            this.Item2 = Item2;
        }

        public override string ToString() => "{Item1: \{Item1}, Item2: \{Item2}}";

        public override int GetHashCode() => HashOf(Item1, Item2);

        public override bool Equals(object obj)
        {
            var t = obj as Tuple<A, B>;
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
    public sealed class Tuple<A, B, C>
    {
        public readonly A Item1;
        public readonly B Item2;
        public readonly C Item3;

        internal Tuple(A Item1, B Item2, C Item3)
        {
            this.Item1 = Item1;
            this.Item2 = Item2;
            this.Item3 = Item3;
        }

        public override string ToString() => "{Item1: \{Item1}, Item2: \{Item2}, Item3: \{Item3}}";

        public override int GetHashCode() => HashOf(Item1, Item2, Item3);

        public override bool Equals(object obj)
        {
            var t = obj as Tuple<A, B, C>;
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
    public sealed class Tuple<A, B, C, D>
    {
        public readonly A Item1;
        public readonly B Item2;
        public readonly C Item3;
        public readonly D Item4;

        internal Tuple(A Item1, B Item2, C Item3, D Item4)
        {
            this.Item1 = Item1;
            this.Item2 = Item2;
            this.Item3 = Item3;
            this.Item4 = Item4;
        }

        public override string ToString() => "{Item1: \{Item1}, Item2: \{Item2}, Item3: \{Item3}, Item4: \{Item4}}";

        public override int GetHashCode() => HashOf(Item1, Item2, Item3, Item4);

        public override bool Equals(object obj)
        {
            var t = obj as Tuple<A, B, C, D>;
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
    public sealed class Tuple<A, B, C, D, E>
    {
        public readonly A Item1;
        public readonly B Item2;
        public readonly C Item3;
        public readonly D Item4;
        public readonly E Item5;

        internal Tuple(A Item1, B Item2, C Item3, D Item4, E Item5)
        {
            this.Item1 = Item1;
            this.Item2 = Item2;
            this.Item3 = Item3;
            this.Item4 = Item4;
            this.Item5 = Item5;
        }

        public override string ToString() => "{Item1: \{Item1}, Item2: \{Item2}, Item3: \{Item3}, Item4: \{Item4}, Item5: \{Item5}}";

        public override int GetHashCode() => HashOf(Item1, Item2, Item3, Item4, Item5);

        public override bool Equals(object obj)
        {
            var t = obj as Tuple<A, B, C, D, E>;
            if (t == null) return false;
            return Equals(t.Item1, Item1) && Equals(t.Item2, Item2) && Equals(t.Item3, Item3) && Equals(t.Item4, Item4) && Equals(t.Item5, Item5);
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
    public sealed class Tuple<A, B, C, D, E, F>
    {
        public readonly A Item1;
        public readonly B Item2;
        public readonly C Item3;
        public readonly D Item4;
        public readonly E Item5;
        public readonly F Item6;

        internal Tuple(A Item1, B Item2, C Item3, D Item4, E Item5, F Item6)
        {
            this.Item1 = Item1;
            this.Item2 = Item2;
            this.Item3 = Item3;
            this.Item4 = Item4;
            this.Item5 = Item5;
            this.Item6 = Item6;
        }

        public override string ToString() => "{Item1: \{Item1}, Item2: \{Item2}, Item3: \{Item3}, Item4: \{Item4}, Item5: \{Item5}, Item6: \{Item6}}";

        public override int GetHashCode() => HashOf(Item1, Item2, Item3, Item4, Item5, Item6);

        public override bool Equals(object obj)
        {
            var t = obj as Tuple<A, B, C, D, E, F>;
            if (t == null) return false;
            return Equals(t.Item1, Item1) && Equals(t.Item2, Item2) && Equals(t.Item3, Item3) && Equals(t.Item4, Item4) && Equals(t.Item5, Item5) && Equals(t.Item6, Item6);
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
    public sealed class Tuple<A, B, C, D, E, F, G>
    {
        public readonly A Item1;
        public readonly B Item2;
        public readonly C Item3;
        public readonly D Item4;
        public readonly E Item5;
        public readonly F Item6;
        public readonly G Item7;

        internal Tuple(A Item1, B Item2, C Item3, D Item4, E Item5, F Item6, G Item7)
        {
            this.Item1 = Item1;
            this.Item2 = Item2;
            this.Item3 = Item3;
            this.Item4 = Item4;
            this.Item5 = Item5;
            this.Item6 = Item6;
            this.Item7 = Item7;
        }

        public override string ToString() => "{Item1: \{Item1}, Item2: \{Item2}, Item3: \{Item3}, Item4: \{Item4}, Item5: \{Item5}, Item6: \{Item6}, Item7: \{Item7}}";

        public override int GetHashCode() => HashOf(Item1, Item2, Item3, Item4, Item5, Item6, Item7);

        public override bool Equals(object obj)
        {
            var t = obj as Tuple<A, B, C, D, E, F, G>;
            if (t == null) return false;
            return Equals(t.Item1, Item1) && Equals(t.Item2, Item2) && Equals(t.Item3, Item3) && Equals(t.Item4, Item4) && Equals(t.Item5, Item5) && Equals(t.Item6, Item6) && Equals(t.Item7, Item7);
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
    public sealed class Tuple<A, B, C, D, E, F, G, H>
    {
        public readonly A Item1;
        public readonly B Item2;
        public readonly C Item3;
        public readonly D Item4;
        public readonly E Item5;
        public readonly F Item6;
        public readonly G Item7;
        public readonly H Item8;

        internal Tuple(A Item1, B Item2, C Item3, D Item4, E Item5, F Item6, G Item7, H Item8)
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

        public override string ToString() => "{Item1: \{Item1}, Item2: \{Item2}, Item3: \{Item3}, Item4: \{Item4}, Item5: \{Item5}, Item6: \{Item6}, Item7: \{Item7}, Item8: \{Item8}}";

        public override int GetHashCode() => HashOf(Item1, Item2, Item3, Item4, Item5, Item6, Item7, Item8);

        public override bool Equals(object obj)
        {
            var t = obj as Tuple<A, B, C, D, E, F, G, H>;
            if (t == null) return false;
            return Equals(t.Item1, Item1) && Equals(t.Item2, Item2) && Equals(t.Item3, Item3) && Equals(t.Item4, Item4) && Equals(t.Item5, Item5) && Equals(t.Item6, Item6) && Equals(t.Item7, Item7) && Equals(t.Item8, Item8);
        }
    }
}