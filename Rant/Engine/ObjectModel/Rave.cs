using System.Collections.Generic;
using System.Linq;

using Rant.Engine.Compiler;
using Rant.Engine.ObjectModel.Parselets;
using Rant.Engine.ObjectModel.Metas;

namespace Rant.Engine.ObjectModel
{
	internal delegate RantObject RaveValueGen();

	/// <summary>
	/// Represents an instance of the Rave virtual machine.
	/// </summary>
	internal class Rave
	{	
		public readonly PatternReader Reader;
		public readonly Sandbox Rant;

		private readonly Stack<RaveState> _stateStack = new Stack<RaveState>(8);
		private readonly Stack<Meta> _valueStack = new Stack<Meta>(8);

		public static RaveValueGen Value(RantObject obj) => () => obj;

		public Rave(Sandbox vm, PatternReader reader)
		{
			Rant = vm;
			Reader = reader;
		}

		public void PushVal(RantObject obj) => _valueStack.Push(new ValueMeta(obj));

		public void PushName(string name) => _valueStack.Push(new NameMeta(name));

		public RantObject PopVal() => _valueStack.Any() ? _valueStack.Pop().Resolve(this) : null;

		public Meta Pop() => _valueStack.Any() ? _valueStack.Pop() : null;

		public void Run()
		{
			_stateStack.Push(new RaveState(this, Precedence.Never));
			
			moar:

			while (_stateStack.Any())
			{
				// Create a new state for each requested expression
				var state = _stateStack.Peek();
				while (state.Parser.MoveNext())
				{
					_stateStack.Push(new RaveState(this, state.Parser.Current));
					goto moar;
				}
				_stateStack.Pop();
			}

			Reader.SkipSpace();

			// Parse more if there is no script terminator
			if (!Reader.Take(R.At))
			{
				_stateStack.Push(new RaveState(this, Precedence.Never));
				goto moar;
			}
			
			var val = PopVal();
			switch (val.Type)
			{
				case RantObjectType.Number:
					Rant.Print(Rant.FormatNumber((double)val.Value));
					break;
				default:
					Rant.Print(val);
					break;
			}
		}

		public Precedence GetPrecedence()
		{
			if (Reader.End) return Precedence.Never;
			InfixParselet infix;
			RaveParse.PostParselets.TryGetValue(Reader.PeekToken().ID, out infix);
			return infix?.Precedence ?? Precedence.Never;
		}
	}
}