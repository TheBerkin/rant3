using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Rant.Core.Stringes
{
	/// <summary>
	/// Represents a set of rules for creating tokens from a stringe.
	/// </summary>
	/// <typeparam name="T">The identifier type to use in tokens created from the context.</typeparam>
	internal sealed class Lexer<T> : IEnumerable where T : struct
	{
		private const int DefaultPriority = 1;

		private readonly HashSet<char> _punctuation;
		private List<_<string, T, StringComparison>> _listNormal;
		private List<_<string, T, StringComparison>> _listHigh;
		private List<_<Regex, RuleMatchValueGenerator<T>, int>> _regexes;
		private List<_<Func<StringeReader, bool>, T, int>> _functions;
		private _<string, T> _endToken;
		private _<Func<Stringe, Stringe>, T> _undefToken;
		private readonly HashSet<T> _ignore;
		private bool _sorted;

		/// <summary>
		/// Creates a new LexerRules instance.
		/// </summary>
		public Lexer()
		{
			_endToken = null;
			_undefToken = null;
			_punctuation = new HashSet<char>();
			_listNormal = new List<_<string, T, StringComparison>>(8);
			_listHigh = new List<_<string, T, StringComparison>>(8);
			_regexes = new List<_<Regex, RuleMatchValueGenerator<T>, int>>(8);
			_functions = new List<_<Func<StringeReader, bool>, T, int>>(8);
			_ignore = new HashSet<T>();
			_sorted = false;
		}

		/// <summary>
		/// A list of token identifiers that should be ignored.
		/// </summary>
		public HashSet<T> IgnoreRules => _ignore;

		/// <summary>
		/// Adds the specified token types to the ignore list.
		/// </summary>
		/// <param name="types">The token types to ignore.</param>
		/// <returns></returns>
		public Lexer<T> Ignore(params T[] types)
		{
			foreach (var t in types) _ignore.Add(t);
			return this;
		}

		/// <summary>
		/// Returns the symbol that represents the specified identifier. If the identifier cannot be found, the method will return an empty string.
		/// </summary>
		/// <param name="id">The identifier to get the symbol for.</param>
		/// <returns></returns>
		public string GetSymbolForId(T id)
		{
			foreach (var rule in _listNormal.Concat(_listHigh).Where(rule => id.Equals(rule.Item2)))
			{
				return rule.Item1;
			}
			return "";
		}

		private bool Available(string symbol)
		{
			return _listNormal.All(t => t.Item1 != symbol) && _listHigh.All(t => t.Item1 != symbol);
		}

		/// <summary>
		/// Define a lexer rule that returns a token when the end of the input is reached.
		/// </summary>
		/// <param name="endTokenId">The token identifier to associate with this rule.</param>
		public void AddEndToken(T endTokenId)
		{
			if (_sorted) throw new InvalidOperationException("Cannot add more rules after they have been used.");
			_endToken = _.Create("EOF", endTokenId);
		}

		/// <summary>
		/// Define a lexer rule that returns a token when the end of the input is reached.
		/// </summary>
		/// <param name="endTokenId">The token identifier to associate with this rule.</param>
		/// <param name="endTokenValue">The value string to assign to the end token.</param>
		public void AddEndToken(T endTokenId, string endTokenValue)
		{
			if (endTokenValue == null) throw new ArgumentNullException(nameof(endTokenValue));
			if (_sorted) throw new InvalidOperationException("Cannot add more rules after they have been used.");
			_endToken = _.Create(endTokenValue, endTokenId);
		}

		/// <summary>
		/// Define a lexer rule that captures unrecognized characters as a token.
		/// </summary>
		/// <param name="tokenId">The token identifier to associate with this rule.</param>
		/// <param name="evalFunc">A function that processes the captured stringe.</param>
		public void AddUndefinedCaptureRule(T tokenId, Func<Stringe, Stringe> evalFunc)
		{
			if (_sorted) throw new InvalidOperationException("Cannot add more rules after they have been used.");
			_undefToken = _.Create(evalFunc, tokenId);
		}


		/// <summary>
		/// Adds a constant rule to the context. This will throw an InvalidOperationException if called after the context is used to create tokens.
		/// </summary>
		/// <param name="symbol">The symbol to test for.</param>
		/// <param name="value">The token identifier to associate with the symbol.</param>
		/// <param name="priority">Determines whether the symbol should be tested before any regex rules.</param>
		public void Add(string symbol, T value, SymbolPriority priority = SymbolPriority.Last)
		{
			if (_sorted) throw new InvalidOperationException("Cannot add more rules after they have been used.");
			if (String.IsNullOrEmpty(symbol)) throw new ArgumentException("Argument 'symbol' can neither be null nor empty.");
			if (!Available(symbol)) throw new InvalidOperationException("A rule with the symbol '" + symbol + "' already exists.");
			(priority == SymbolPriority.First ? _listHigh : _listNormal).Add(_.Create(symbol, value, StringComparison.InvariantCulture));
			_punctuation.Add(symbol[0]);
		}

		/// <summary>
		/// Adds a constant rule to the context that affects all symbols in the specified array. This will throw an InvalidOperationException if called after the context is used to create tokens.
		/// </summary>
		/// <param name="symbols">The symbols to test for.</param>
		/// <param name="value">The token identifier to associate with the symbols.</param>
		/// <param name="priority">Determines whether the symbol should be tested before any regex rules.</param>
		public void Add(string[] symbols, T value, SymbolPriority priority = SymbolPriority.Last)
		{
			if (_sorted) throw new InvalidOperationException("Cannot add more rules after they have been used.");
			if (symbols == null) throw new ArgumentNullException(nameof(symbols));
			if (symbols.Length == 0) throw new ArgumentException("Tried to use an empty symbol array.");
			foreach (var s in symbols)
			{
				if (String.IsNullOrEmpty(s)) throw new ArgumentException("One or more symbols in the provided array were empty or null.");
				if (!Available(s)) throw new InvalidOperationException("A rule with the symbol '" + s + "' already exists.");
				(priority == SymbolPriority.First ? _listHigh : _listNormal).Add(_.Create(s, value, StringComparison.InvariantCulture));
				_punctuation.Add(s[0]);
			}
		}

		/// <summary>
		/// Adds a constant rule to the context. This will throw an InvalidOperationException if called after the context is used to create tokens.
		/// </summary>
		/// <param name="symbol">The symbol to test for.</param>
		/// <param name="value">The token identifier to associate with the symbol.</param>
		/// <param name="ignoreCase">Specifies whether the rule should ignore capitalization.</param>
		/// <param name="priority">Determines whether the symbol should be tested before any regex rules.</param>
		public void Add(string symbol, T value, bool ignoreCase, SymbolPriority priority = SymbolPriority.Last)
		{
			if (_sorted) throw new InvalidOperationException("Cannot add more rules after they have been used.");
			if (String.IsNullOrEmpty(symbol)) throw new ArgumentException("Argument 'symbol' can neither be null nor empty.");
			if (!Available(symbol)) throw new InvalidOperationException("A rule with the symbol '" + symbol + "' already exists.");
			(priority == SymbolPriority.First ? _listHigh : _listNormal).Add(_.Create(symbol, value, ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture));
			_punctuation.Add(symbol[0]);
		}

		/// <summary>
		/// Adds a constant rule to the context that affects all symbols in the specified array. This will throw an InvalidOperationException if called after the context is used to create tokens.
		/// </summary>
		/// <param name="symbols">The symbols to test for.</param>
		/// <param name="value">The token identifier to associate with the symbols.</param>
		/// <param name="ignoreCase">Specifies whether the rule should ignore capitalization.</param>
		/// <param name="priority">Determines whether the symbol should be tested before any regex rules.</param>
		public void Add(string[] symbols, T value, bool ignoreCase, SymbolPriority priority = SymbolPriority.Last)
		{
			if (_sorted) throw new InvalidOperationException("Cannot add more rules after they have been used.");
			if (symbols == null) throw new ArgumentNullException(nameof(symbols));
			if (symbols.Length == 0) throw new ArgumentException("Tried to use an empty symbol array.");
			foreach (var s in symbols)
			{
				if (String.IsNullOrEmpty(s)) throw new ArgumentException("One or more symbols in the provided array were empty or null.");
				if (!Available(s)) throw new InvalidOperationException("A rule with the symbol '" + s + "' already exists.");
				(priority == SymbolPriority.First ? _listHigh : _listNormal).Add(_.Create(s, value, ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture));
				_punctuation.Add(s[0]);
			}
		}

		/// <summary>
		/// Adds a regex rule to the context. This will throw an InvalidOperationException if called after the context is used to create tokens.
		/// </summary>
		/// <param name="regex">The regex to test for.</param>
		/// <param name="value">The token identifier to associate with the symbol.</param>
		/// <param name="priority">The priority of the rule. Higher values are checked first.</param>
		public void Add(Regex regex, T value, int priority = DefaultPriority)
		{
			if (_sorted) throw new InvalidOperationException("Cannot add more rules after they have been used.");
			if (regex == null) throw new ArgumentNullException(nameof(regex));
			if (_regexes.Any(re => re.Item1 == regex)) throw new InvalidOperationException("A rule with this pattern already exists.");

			_regexes.Add(_.Create(regex, new RuleMatchValueGenerator<T>(value), priority));
		}

		/// <summary>
		/// Adds a regex rule to the context. This will throw an InvalidOperationException if called after the context is used to create tokens.
		/// </summary>
		/// <param name="regex">The regex to test for.</param>
		/// <param name="generator">A function that generates a token identifier from the match.</param>
		/// <param name="priority">The priority of the rule. Higher values are checked first.</param>
		public void Add(Regex regex, Func<Match, T> generator, int priority = DefaultPriority)
		{
			if (_sorted) throw new InvalidOperationException("Cannot add more rules after they have been used.");
			if (regex == null) throw new ArgumentNullException(nameof(regex));
			if (generator == null) throw new ArgumentNullException(nameof(generator));
			if (_regexes.Any(re => re.Item1 == regex)) throw new InvalidOperationException("A rule with this pattern already exists.");
			_regexes.Add(_.Create(regex, new RuleMatchValueGenerator<T>(generator), priority));
		}

		/// <summary>
		/// Adds a function rule to the context. This will throw an InvalidOperationException if called after the context is used to create tokens.
		/// </summary>
		/// <param name="func">The function to read the token with.</param>
		/// <param name="value">The token identifier to associate with the function.</param>
		/// <param name="priority">The priority of the rule. Higher values are checked first.</param>
		public void Add(Func<StringeReader, bool> func, T value, int priority = DefaultPriority)
		{
			if (_sorted) throw new InvalidOperationException("Cannot add more rules after they have been used.");
			if (func == null) throw new ArgumentNullException(nameof(func));
			_functions.Add(_.Create(func, value, priority));
		}

		/// <summary>
		/// Tokenizes the input stringe and enumerates the resulting tokens.
		/// </summary>
		/// <param name="input">The input to tokenize.</param>
		/// <returns></returns>
		public IEnumerable<Token<T>> Tokenize(Stringe input)
		{
			var reader = new StringeReader(input);

			while (!reader.EndOfStringe)
			{
				yield return reader.ReadToken(this);
			}
		}

		internal bool HasPunctuation(int c)
		{
			return c != -1 && _punctuation.Contains((char)c);
		}

		internal bool HasPunctuation(char c)
		{
			return _punctuation.Contains(c);
		}

		private void Sort()
		{
			if (_sorted) return;
			_listNormal = _listNormal.OrderByDescending(t => t.Item1.Length).ToList();
			_listHigh = _listHigh.OrderByDescending(t => t.Item1.Length).ToList();
			_regexes = _regexes.OrderByDescending(r => r.Item3).ToList();
			_functions = _functions.OrderByDescending(t => t.Item3).ToList();
			_sorted = true;
		}

		internal _<Func<Stringe, Stringe>, T> UndefinedCaptureRule => _undefToken;

		internal _<string, T> EndToken => _endToken;

		internal List<_<string, T, StringComparison>> NormalSymbols
		{
			get
			{
				Sort();
				return _listNormal;
			}
		}

		internal List<_<string, T, StringComparison>> HighSymbols
		{
			get
			{
				Sort();
				return _listHigh;
			}
		}

		internal List<_<Regex, RuleMatchValueGenerator<T>, int>> RegexList
		{
			get
			{
				Sort();
				return _regexes;
			}
		}

		internal List<_<Func<StringeReader, bool>, T, int>> FunctionList
		{
			get
			{
				Sort();
				return _functions;
			}
		}

		/// <summary>
		/// Definitely does not return the enumerator for the current instance.
		/// </summary>
		/// <returns></returns>
		public IEnumerator GetEnumerator()
		{
			throw new InvalidOperationException("Cannot enumerate a rule set.");
		}
	}

	/// <summary>
	/// Used to manipulate the order in which symbol (non-regex) rules are tested.
	/// </summary>
	public enum SymbolPriority
	{
		/// <summary>
		/// Test symbol after testing regex symbols. This is the default value for all symbols rules.
		/// </summary>
		Last = 0,

		/// <summary>
		/// Test symbol before testing any regex rules.
		/// </summary>
		First = 1
	}

	internal class RuleMatchValueGenerator<T>
	{
		private readonly T _value;
		private readonly Func<Match, T> _func;

		public RuleMatchValueGenerator(T value)
		{
			_value = value;
			_func = null;
		}

		public RuleMatchValueGenerator(Func<Match, T> generator)
		{
			_func = generator;
		}

		public T GetValue(Match m)
		{
			return _func == null ? _value : _func(m);
		}
	}
}