using System;
using System.Collections.Generic;
using Rant.Stringes;

namespace Rant.Engine.Syntax.Expressions
{
	internal class REAPatternString : RantExpressionAction
	{
		private string _value;
		private RantPattern _pattern;

		public override int GetHashCode() => _value.GetHashCode();
        public RantPattern Pattern => _pattern;

        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                CreatePattern();
            }
        }

		public REAPatternString(string value, Stringe origin)
			: base(origin)
		{
			_value = value;
			Type = ActionValueType.Pattern;
		}

		public override object GetValue(Sandbox sb)
		{
			return this;
		}

		public RantAction Execute(Sandbox sb)
		{
            if (_pattern == null)
                CreatePattern();
			return new REAString(sb.EvalPattern(sb.LastTimeout, _pattern).Main, Range);
		}

        private void CreatePattern()
        {
            _pattern = new RantPattern("pattern string", RantPatternSource.String, Value);
        }

        public override IEnumerator<RantAction> Run(Sandbox sb)
		{
            if (_pattern == null)
                CreatePattern();
            yield break;
		}

		public override string ToString()
		{
			return Value;
		}
	}
}
