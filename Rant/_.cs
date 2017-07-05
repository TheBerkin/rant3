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
}