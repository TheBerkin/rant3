using Rant.Core.Framework;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Rant.Tools.Commands
{
	[CommandName("fdocs", UsesPath = false, Description = "Generates a Rant function reference document in Markdown format.")]
	[CommandParam("out", false, "Indicates the output path for the generated file.")]
	internal class FuncDocsCommand : Command
	{
		protected override void OnRun()
		{
			var argOut = CmdLine.Property("out", Path.Combine(Environment.CurrentDirectory, "functions.md"));

			using (var writer = new StreamWriter(argOut))
			{
				writer.Write("# Function reference\n\n");

				foreach (var group in RantUtils.GetFunctions().OrderBy(g => g.Name))
				{
					writer.Write($"## {group.Name}\n\n");

					var aliases = RantUtils.GetFunctionAliases(group.Name).Where(a => a != group.Name).ToArray();

					if (aliases.Length > 0)
					{
						writer.Write($"**Aliases:** {aliases.Select(a => $"`{a}`").Aggregate((c, n) => c + ", " + n)}<br/>");
					}
					writer.Write($"**Overloads:** {group.Overloads.Count()}\n\n");

					foreach (var func in group.Overloads.OrderBy(f => f.ParamCount))
					{

						writer.Write($"### {func}\n\n");
						writer.Write(func.Description);

						var fparams = func.GetParameters().ToArray();

						if (fparams.Length > 0)
						{
							writer.Write("\n\n**Parameters**\n\n|Name|Type|Description|\n|---|---|---|\n");

							foreach (var p in fparams)
							{

								if (p.RantType == RantFunctionParameterType.Mode || p.RantType == RantFunctionParameterType.Flags)
								{
									var sb = new StringBuilder();
									sb.Append("<ul>");
									foreach (var mode in p.GetEnumValues())
									{
										sb.Append($"<li><b>{mode.Name}</b><br/>{mode.Description}</li>");
									}
									sb.Append("</ul>");
									writer.Write($"|{p.Name + (p.IsParams ? "..." : "")}|{p.RantType}|{(String.IsNullOrEmpty(p.Description) ? sb.ToString() : p.Description + "<br/><br/>" + sb.ToString())}|\n");
								}
								else
								{
									writer.Write($"|{p.Name + (p.IsParams ? "..." : "")}|{p.RantType}|{(String.IsNullOrEmpty(p.Description) ? "*No description*" : p.Description)}|\n");
								}
							}
						}

						writer.Write("\n***\n");
					}
				}
			}
		}
	}
}
