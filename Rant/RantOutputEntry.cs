using Rant.Core.Output;

namespace Rant
{
	/// <summary>
	/// Represents the output of a single channel.
	/// </summary>
	public sealed class RantOutputEntry
	{
		internal RantOutputEntry(string name, string value, ChannelVisibility visiblity)
		{
			Name = name;
			Value = value;
			Visiblity = visiblity;
		}

		/// <summary>
		/// Gets the name of the channel.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Gets the value of the channel.
		/// </summary>
		public string Value { get; }

		/// <summary>
		/// The visibility of the channel that created the output entry.
		/// </summary>
		public ChannelVisibility Visiblity { get; }
	}
}