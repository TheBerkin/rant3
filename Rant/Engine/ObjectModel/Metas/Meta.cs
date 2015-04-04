namespace Rant.Engine.ObjectModel.Metas
{
	/// <summary>
	/// Represents a value stack item that can be resolved to a Rant object.
	/// </summary>
	internal abstract class Meta
	{
		public abstract RantObject Resolve(Rave rave);
	}
}