using Rant.Stringes;

namespace Rant.Engine.Syntax.Richard
{
	internal abstract class RantExpressionAction : RantAction
	{
		public RantExpressionAction(Stringe _origin)
			: base(_origin)
		{
		}

		public ActionValueType Type = ActionValueType.Null;

		public abstract object GetValue(Sandbox sb);

		internal enum ActionValueType
		{
			Null,
			Boolean,
			Number,
			String,
			List,
			Object,
			Pattern,
			Function
		};

        public bool Breakable = false;
        public bool Returnable = false;
	}
}
