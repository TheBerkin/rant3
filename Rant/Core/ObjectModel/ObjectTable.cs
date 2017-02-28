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

using Rant.Core.Utilities;

namespace Rant.Core.ObjectModel
{
	/// <summary>
	/// Stores global and local variables for a single engine instance.
	/// </summary>
	internal class ObjectTable
	{
		internal readonly Dictionary<string, RantObject> Globals = new Dictionary<string, RantObject>();

		public RantObject this[string name]
		{
			get
			{
				if (!Util.ValidateName(name)) return null;
				RantObject obj;
				return Globals.TryGetValue(name, out obj) ? obj : null;
			}
			set
			{
				if (!Util.ValidateName(name)) return;
				if (value == null) Globals.Remove(name);
				Globals[name] = value;
			}
		}

		public ObjectStack CreateLocalStack() => new ObjectStack(this);
	}
}