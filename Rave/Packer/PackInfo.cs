using System.Collections.Generic;

using Newtonsoft.Json;

using Rant;
using Rant.Resources;

namespace Rave.Packer
{
	public class PackInfo
	{
		[JsonProperty("title")]
		public string Title { get; set; }

		[JsonProperty("id")]
		public string ID { get; set; }

		[JsonProperty("authors")]
		public string[] Authors { get; set; }

		[JsonProperty("version")]
		public string Version { get; set; }

		[JsonProperty("description")]
		public string Description { get; set; }

		[JsonProperty("tags")]
		public string[] Tags { get; set; }

		[JsonProperty("out")]
		public string OutputPath { get; set; }

		[JsonProperty("dependencies", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(DependencyListConverter))]
		public List<RantPackageDependency> Dependencies { get; set; } = new List<RantPackageDependency>();
	}
}