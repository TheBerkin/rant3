using System.Collections.Generic;

using Rant.Core.Stringes;

namespace Rant.Core.Compiler.Syntax.Richard
{
	internal class RichPatternString : RichActionBase
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

		public RichPatternString(string value, Stringe origin)
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
			return new RichString(sb.EvalPattern(sb.LastTimeout, _pattern).Main, Range);
		}

        private void CreatePattern()
        {
            _pattern = new RantPattern("pattern string", RantPatternOrigin.String, Value);
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
