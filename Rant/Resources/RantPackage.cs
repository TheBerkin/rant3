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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Rant.Core.IO;
using Rant.Core.Utilities;

using static Rant.Localization.Txtres;
using System.IO.Compression;

namespace Rant.Resources
{
	/// <summary>
	/// Represents a collection of patterns and tables that can be exported to an archive file.
	/// </summary>
	public sealed class RantPackage
    {
        private const string MAGIC = "NFRP";
        internal const string EXTENSION = ".rantpkg";
        private const ushort PACKAGE_FORMAT_VERSION = 3;
        private readonly HashSet<RantPackageDependency> _dependencies = new HashSet<RantPackageDependency>();
        private readonly HashSet<RantResource> _resources = new HashSet<RantResource>();
        private string _id = GetString("default-package-id");
        private string _title = GetString("untitled-package");
        private RantPackageVersion _version = new RantPackageVersion(1, 0, 0);

        /// <summary>
        /// The display name of the package.
        /// </summary>
        public string Title
        {
            get { return _title; }
            set
            {
                if (Util.IsNullOrWhiteSpace(value)) throw new ArgumentException(GetString("err-empty-pkg-title"));
                _title = value;
            }
        }

        /// <summary>
        /// The ID of the package.
        /// </summary>
        public string ID
        {
            get { return _id; }
            set
            {
                if (Util.IsNullOrWhiteSpace(value)) throw new ArgumentException(GetString("err-empty-pkg-id"));
                _id = value;
            }
        }

        /// <summary>
        /// The description for the package.
        /// </summary>
        public string Description { get; set; } = "";

        /// <summary>
        /// The tags associated with the package.
        /// </summary>
        public string[] Tags { get; set; } = { };

        /// <summary>
        /// The package version.
        /// </summary>
        public RantPackageVersion Version
        {
            get { return _version; }
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                _version = value;
            }
        }

        /// <summary>
        /// The authors of the package.
        /// </summary>
        public string[] Authors { get; set; } = { };

        /// <summary>
        /// Adds the specified dependency to the package.
        /// </summary>
        /// <param name="dependency">The dependency to add.</param>
        public void AddDependency(RantPackageDependency dependency)
        {
            if (dependency == null) throw new ArgumentNullException(nameof(dependency));
            _dependencies.Add(dependency);
        }

        /// <summary>
        /// Adds the specified dependency to the package.
        /// </summary>
        /// <param name="id">The ID of the package.</param>
        /// <param name="version">The package version to target.</param>
        public void AddDependency(string id, string version)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            if (version == null) throw new ArgumentNullException(nameof(version));
            _dependencies.Add(new RantPackageDependency(id, version));
        }

        /// <summary>
        /// Determines whether the package depends on the specified package.
        /// </summary>
        /// <param name="id">The ID of the package to check for.</param>
        /// <param name="version">The version of the package to check for.</param>
        /// <returns></returns>
        public bool DependsOn(string id, string version)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            if (version == null) throw new ArgumentNullException(nameof(version));
            return _dependencies.Contains(new RantPackageDependency(id, version));
        }

        /// <summary>
        /// Determines whether the package has the specified dependency.
        /// </summary>
        /// <param name="dependency">The dependency to check for.</param>
        /// <returns></returns>
        public bool DependsOn(RantPackageDependency dependency)
        {
            if (dependency == null) throw new ArgumentNullException(nameof(dependency));
            return _dependencies.Contains(dependency);
        }

        /// <summary>
        /// Enumerates the package's dependencies.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<RantPackageDependency> GetDependencies() => _dependencies.AsEnumerable();

        /// <summary>
        /// Removes the specified dependency from the package.
        /// </summary>
        /// <param name="id">The ID of the dependency to remove.</param>
        /// <param name="version">The version of the dependency to remove.</param>
        /// <returns></returns>
        public bool RemoveDependency(string id, string version)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            if (version == null) throw new ArgumentNullException(nameof(version));
            return _dependencies.Remove(new RantPackageDependency(id, version));
        }

        /// <summary>
        /// Removes the specified dependency from the package.
        /// </summary>
        /// <param name="dependency">The dependency to remove.</param>
        /// <returns></returns>
        public bool RemoveDependency(RantPackageDependency dependency)
        {
            if (dependency == null) throw new ArgumentNullException(nameof(dependency));
            return _dependencies.Remove(dependency);
        }

        /// <summary>
        /// Removes all dependencies from the package.
        /// </summary>
        public void ClearDependencies() => _dependencies.Clear();

        /// <summary>
        /// Enumerates all resources in the package.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<RantResource> GetResources() => _resources.AsEnumerable();

        /// <summary>
        /// Enumerates all resources matching the specified resource type.
        /// </summary>
        /// <typeparam name="TResource">The resource type to search for.</typeparam>
        /// <returns></returns>
        public IEnumerable<TResource> GetResources<TResource>() where TResource : RantResource => _resources.OfType<TResource>();

        /// <summary>
        /// Adds the specified resource to the package.
        /// </summary>
        /// <param name="resource">The resource to add.</param>
        /// <returns></returns>
        public bool AddResource(RantResource resource)
        {
            if (resource == null) throw new ArgumentNullException(nameof(resource));
            return _resources.Add(resource);
        }

        /// <summary>
        /// Removes the specified resource from the package.
        /// </summary>
        /// <param name="resource">The resource to remove.</param>
        /// <returns></returns>
        public bool RemoveResource(RantResource resource)
        {
            if (resource == null) throw new ArgumentNullException(nameof(resource));
            return _resources.Remove(resource);
        }

        /// <summary>
        /// Determines whether the package contains the specified resource.
        /// </summary>
        /// <param name="resource">The resource to search for.</param>
        /// <returns></returns>
        public bool ContainsResource(RantResource resource)
        {
            if (resource == null) throw new ArgumentNullException(nameof(resource));
            return _resources.Contains(resource);
        }

        /// <summary>
        /// Saves the package to the specified file path.
        /// </summary>
        /// <param name="path">The path to the file to create.</param>
        /// <param name="compress">Specifies whether to compress the package contents.</param>
        public void Save(
            string path,
            bool compress = true)
        {
            if (string.IsNullOrEmpty(Path.GetExtension(path)))
                path += EXTENSION;
			
            using (var writer = new EasyWriter(File.Create(path)))
            {
				writer.Write(Encoding.ASCII.GetBytes(MAGIC));
				writer.Write(PACKAGE_FORMAT_VERSION);
				writer.Write(compress);
				writer.Write(_title);
				writer.Write(_id);
				writer.Write(Description);
				writer.Write(Tags);
				writer.Write(Authors);
				writer.Write(Version.Major);
				writer.Write(Version.Minor);
				writer.Write(Version.Revision);
				writer.Write(_dependencies.Count);
				foreach(var dep in _dependencies)
				{
					writer.Write(dep.ID);
					writer.Write(dep.Version.Major);
					writer.Write(dep.Version.Minor);
					writer.Write(dep.Version.Revision);
					writer.Write(dep.AllowNewer);
				}
				writer.Write(_resources.Count);

				if (compress)
				{
					using (var compressStream = new DeflateStream(writer.BaseStream, CompressionMode.Compress, true))
					{
						foreach (var res in _resources)
						{
							RantResource.SerializeResource(res, new EasyWriter(compressStream, leaveOpen: true));
						}
						compressStream.Flush();
					}	
				}
				else
				{
					foreach (var res in _resources)
					{
						RantResource.SerializeResource(res, writer);
					}
				}
				writer.BaseStream.Flush();
            }
        }

        /// <summary>
        /// Loads a package from the specified path and returns it as a RantPackage object.
        /// </summary>
        /// <param name="path">The path to the package file to load.</param>
        /// <returns></returns>
        public static RantPackage Load(string path)
        {
            if (string.IsNullOrEmpty(Path.GetExtension(path))) path += EXTENSION;
            return Load(File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read));
        }

        /// <summary>
        /// Loads a package from the specified stream and returns it as a RantPackage object.
        /// </summary>
        /// <param name="source">The stream to load the package data from.</param>
        /// <returns></returns>
        public static RantPackage Load(Stream source)
        {
            using (var reader = new EasyReader(source))
            {
                string magic = Encoding.ASCII.GetString(reader.ReadBytes(4));
                if (magic != MAGIC)
                    throw new InvalidDataException(GetString("err-file-corrupt"));
                ushort version = reader.ReadUInt16();
                if (version != PACKAGE_FORMAT_VERSION)
                    throw new InvalidDataException(GetString("err-invalid-package-version", version));
                bool compress = reader.ReadBoolean();

				var package = new RantPackage();

				package.Title = reader.ReadString();
				package.ID = reader.ReadString();
				package.Description = reader.ReadString();
				package.Tags = reader.ReadStringArray();
				package.Authors = reader.ReadStringArray();
				int vmaj = reader.ReadInt32();
				int vmin = reader.ReadInt32();
				int vrev = reader.ReadInt32();
				package.Version = new RantPackageVersion(vmaj, vmin, vrev);
				int depCount = reader.ReadInt32();
				for(int i = 0; i < depCount; i++)
				{
					var depId = reader.ReadString();
					int depVerMaj = reader.ReadInt32();
					int depVerMin = reader.ReadInt32();
					int depVerRev = reader.ReadInt32();
					bool depAllowNewer = reader.ReadBoolean();
					package.AddDependency(new RantPackageDependency(depId, new RantPackageVersion(depVerMaj, depVerMin, depVerRev))
					{
						AllowNewer = depAllowNewer
					});
				}

				int resCount = reader.ReadInt32();

				if (compress)
				{
					using (var decompressStream = new DeflateStream(reader.BaseStream, CompressionMode.Decompress, true))
					{
						for (int i = 0; i < resCount; i++)
						{
							package._resources.Add(RantResource.DeserializeResource(new EasyReader(decompressStream, true)));
						}
					}
				}
				else
				{
					for(int i = 0; i < resCount; i++)
					{
						package._resources.Add(RantResource.DeserializeResource(reader));
					}
				}

				return package;
            }
        }

        /// <summary>
        /// Returns a string containing the title and version of the package.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"{Title}, v{Version}";
    }
}