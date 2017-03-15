using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rant.Formats
{
	public abstract class NumberVerbalizer
	{
		public abstract string Verbalize(long number);
	}
}
