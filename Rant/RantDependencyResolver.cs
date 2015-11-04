using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Rant
{
	/// <summary>
	/// Default class for package depdendency resolving.
	/// </summary>
	public class RantDependencyResolver
	{
		/// <summary>
		/// Attempts to resolve a depdendency to the appropriate package.
		/// </summary>
		/// <param name="depdendency">The depdendency to resolve.</param>
		/// <param name="package">The package loaded from the depdendency.</param>
		/// <returns></returns>
		public virtual bool TryResolvePackage(RantPackageDependency depdendency, out RantPackage package)
		{
			package = null;
			var path = $"{depdendency.ID}.rantpkg";
			if (!File.Exists(path))
			{
				RantPackageVersion version;
				// Fallback to name with version appended
				path = Directory.GetFiles(Environment.CurrentDirectory, $"{depdendency.ID}*.rantpkg").FirstOrDefault(p =>
				{
					var match = Regex.Match(Path.GetFileNameWithoutExtension(p), depdendency.ID + @"[\s\-_.]+v?(?<version>\d+(\.\d+){1,2})");
					if (!match.Success) return false;
					version = RantPackageVersion.Parse(match.Groups["version"].Value);
					return (depdendency.AllowNewer && version >= depdendency.Version) || depdendency.Version == version;
				});
				if (path == null) return false;
			}
			try
			{
				var pkg = RantPackage.Load(path);
				if (pkg.ID != depdendency.ID) return false;
				if ((depdendency.AllowNewer && package.Version >= depdendency.Version) || package.Version == depdendency.Version)
				{
					package = pkg;
					return true;
				}
				return false;
			}
			catch
			{
				return false;
			}
		}
	}
}