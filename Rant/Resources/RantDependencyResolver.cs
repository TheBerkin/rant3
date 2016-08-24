using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Rant.Resources
{
	/// <summary>
	/// Default class for package depdendency resolving.
	/// </summary>
	public class RantDependencyResolver
	{
		private static readonly Regex FallbackRegex = new Regex(@"[\s\-_.]*[vV]?(?<version>\d+(\.\d+){0,2})$",
			RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.Compiled);

		/// <summary>
		/// Attempts to resolve a depdendency to the appropriate package.
		/// </summary>
		/// <param name="depdendency">The depdendency to resolve.</param>
		/// <param name="package">The package loaded from the depdendency.</param>
		/// <returns></returns>
		public virtual bool TryResolvePackage(RantPackageDependency depdendency, out RantPackage package)
		{
			package = null;
			string path = Path.Combine(Environment.CurrentDirectory, $"{depdendency.ID}.rantpkg");
			if (!File.Exists(path))
			{
				RantPackageVersion version;
				// Fallback to name with version appended
#if UNITY
				path = Directory.GetFiles(Environment.CurrentDirectory, $"{depdendency.ID}*.rantpkg", SearchOption.AllDirectories).FirstOrDefault(p =>
#else
				path =
					Directory.EnumerateFiles(Environment.CurrentDirectory, "*.rantpkg", SearchOption.AllDirectories)
						.FirstOrDefault(p =>
#endif

						{
							var match = FallbackRegex.Match(Path.GetFileNameWithoutExtension(p));
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
				if ((depdendency.AllowNewer && pkg.Version >= depdendency.Version) || pkg.Version == depdendency.Version)
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