using System.Collections.Generic;

using Rant.Core.IO;
using Rant.Core.Stringes;

namespace Rant.Core.Compiler.Syntax
{
	/// <summary>
	/// Represents a Rant Syntax Tree (RST) node for a Rant pattern. This is the base class for all Rant actions.
	/// </summary>
	internal abstract class RST
	{
		internal TokenLocation Location;

		protected RST(Stringe location)
		{
			Location = TokenLocation.FromStringe(location);
		}

		protected RST(TokenLocation location)
		{
			Location = location;
		}

		/// <summary>
		/// Performs the operations defined in the RST, given a specific sandbox to operate upon.
		/// </summary>
		/// <param name="sb">The sandbox on which to operate.</param>
		/// <returns></returns>
		public abstract IEnumerator<RST> Run(Sandbox sb);

		public void SerializeObject(EasyWriter output)
		{
			// TODO: Write type code
			//Serialize(output);
		}

		public void DeserializeObject(EasyReader input)
		{
			// Type code has already been read
			//Deserialize(input);
		}
	}
}