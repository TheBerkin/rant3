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
using System.Threading;

using Rant.Core.IO;
using Rant.Core.IO.Bson;
using Rant.Core.IO.Compression;
using Rant.Core.Utilities;

using static Rant.Localization.Txtres;

namespace Rant.Resources
{
    /// <summary>
    /// Represents a collection of patterns and tables that can be exported to an archive file.
    /// </summary>
    public sealed class RantPackage
    {
        private const string MAGIC = "RPKG";
        internal const string EXTENSION = ".rantpkg";
        private const ushort PACKAGE_VERSION = 3;
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
                if (Util.IsNullOrWhiteSpace(value)) throw new ArgumentException("Title cannot be empty.");
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
                if (Util.IsNullOrWhiteSpace(value)) throw new ArgumentException("ID cannot be empty.");
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
        /// <param name="stringTableMode">Specifies string table behavior for the package.</param>
        public void Save(
            string path,
            bool compress = true,
            BsonStringTableMode stringTableMode = BsonStringTableMode.None)
        {
            if (string.IsNullOrEmpty(Path.GetExtension(path)))
                path += EXTENSION;

            using (var writer = new EasyWriter(File.Create(path)))
            {
                var doc = new BsonDocument(stringTableMode)
                {
                    Top =
                    {
                        ["info"] = new BsonItem
                        {
                            ["title"] = new BsonItem(_title),
                            ["id"] = new BsonItem(_id),
                            ["description"] = new BsonItem(Description),
                            ["tags"] = new BsonItem(Tags),
                            ["version"] = new BsonItem(Version.ToString()),
                            ["authors"] = new BsonItem(Authors),
                            ["dependencies"] = new BsonItem(_dependencies.Select(dep =>
                            {
                                var depObj = new BsonItem
                                {
                                    ["id"] = dep.ID,
                                    ["version"] = dep.Version.ToString(),
                                    ["allow-newer"] = dep.AllowNewer
                                };
                                return depObj;
                            }).ToArray())
                        },
                        ["resources"] = new BsonItem(_resources.Select(RantResource.SerializeResource).ToList())
                    }
                };

                var data = doc.ToByteArray(stringTableMode != BsonStringTableMode.None);
                if (compress)
                    data = EasyCompressor.Compress(data);
                writer.Write(Encoding.ASCII.GetBytes(MAGIC));
                writer.Write(PACKAGE_VERSION);
                writer.Write(compress);
                writer.Write(data.Length);
                writer.Write(data);
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
                var package = new RantPackage();
                ushort version = reader.ReadUInt16();
                if (version != PACKAGE_VERSION)
                    throw new InvalidDataException(GetString("err-invalid-package-version", version));
                bool compress = reader.ReadBoolean();
                int size = reader.ReadInt32();
                var data = reader.ReadBytes(size);
                if (compress) data = EasyCompressor.Decompress(data);
                var doc = BsonDocument.Read(data);

                var info = doc["info"];
                if (info == null)
                    throw new InvalidDataException(GetString("err-missing-package-meta"));

                package.Title = info["title"];
                package.ID = info["id"];
                package.Version = RantPackageVersion.Parse(info["version"]);
                package.Description = info["description"];
                package.Authors = (string[])info["authors"];
                package.Tags = (string[])info["tags"];

                var deps = info["dependencies"];
                if (deps != null && deps.IsArray)
                {
                    for (int i = 0; i < deps.Count; i++)
                    {
                        var dep = deps[i];
                        var depId = dep["id"].Value;
                        var depVersion = dep["version"].Value;
                        bool depAllowNewer = (bool)dep["allow-newer"].Value;
                        package.AddDependency(new RantPackageDependency(depId.ToString(), depVersion.ToString())
                        {
                            AllowNewer = depAllowNewer
                        });
                    }
                }
				
                package._resources.AddRange(doc["resources"].Values.Select(RantResource.DeserializeResource).Where(res => res != null));

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