using System;

namespace Rant
{
	/// <summary>
	/// Represents a dependency for a Rant package.
	/// </summary>
	public sealed class RantPackageDependency
	{
		private string _id;
		private string _version;

		/// <summary>
		/// The ID of the package.
		/// </summary>
		public string ID
		{
			get { return _id; }
			set
			{
				if (value == null) throw new ArgumentNullException(nameof(value));
				_id = value.Trim();
			}
		}

		/// <summary>
		/// The targeted version of the package.
		/// </summary>
		public string Version
		{
			get { return _version; }
			set
			{
				if (value == null) throw new ArgumentNullException(nameof(value));
				_version = value.Trim();
			}
		}

		/// <summary>
		/// Initializes a new RantPackageDependency object.
		/// </summary>
		/// <param name="id">The ID of the package.</param>
		/// <param name="version">The targeted version of the package.</param>
		public RantPackageDependency(string id, string version)
		{
			if (id == null) throw new ArgumentNullException(nameof(id));
			if (version == null) throw new ArgumentNullException(nameof(version));
			ID = id;
			Version = version;
		}

		/// <summary>
		/// Creates a dependency for the specified package.
		/// </summary>
		/// <param name="package">The package to create the dependency for.</param>
		/// <returns></returns>
		public static RantPackageDependency Create(RantPackage package)
		{
			if (String.IsNullOrWhiteSpace(package.ID))
				throw new ArgumentException("Package ID cannot be empty.");
			if (String.IsNullOrWhiteSpace(package.Version))
				throw new ArgumentException("Package version cannot be empty.");
			return new RantPackageDependency(package.ID, package.Version);
		}

		/// <summary>
		/// Returns a string representation of the current dependency.
		/// </summary>
		/// <returns></returns>
		public override string ToString() => $"{ID} {Version}";

		/// <summary>
		/// Gets the hash code for the instance.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode() => unchecked((ID.GetHashCode() + 11) * (Version.GetHashCode() + 12345));

		/// <summary>
		/// Determines whether the current RantPackageDependency is equal to the specified instance.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			var d = obj as RantPackageDependency;
			if (d == null) return false;
			return String.Equals(ID, d.ID, StringComparison.InvariantCulture) &&
			       String.Equals(Version, d.Version, StringComparison.InvariantCulture);
		}
	}
}