using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;

using Rant;
using Rant.Vocabulary;

using static Rant.Common.CmdLine;

namespace Rant.Benchmark
{
	class Program
	{
		public static readonly string LEGACY_DIC_PATH = Property("ldict");
		public static readonly string PKG_PATH = Property("package");
		public static readonly int ITERATIONS;

		static Program()
		{
			if(!int.TryParse(Property("iterations"), out ITERATIONS))
				ITERATIONS = 5;
		}

		static void Main(string[] args)
		{
			var rant = new RantEngine();
			if(!string.IsNullOrEmpty(LEGACY_DIC_PATH))
			{
				var tables =
					Directory
						.GetFiles(LEGACY_DIC_PATH)
						.Where(f => Path.GetExtension(f) == ".dic")
						.Select(f => RantDictionaryTable.FromLegacyFile(f));
				rant.Dictionary = new RantDictionary(tables);
			}

			if(!string.IsNullOrEmpty(PKG_PATH))
			{
				rant.LoadPackage(PKG_PATH);
			}
			else
			{
				foreach(string pkg in Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.rantpkg", SearchOption.AllDirectories))
				{
					rant.LoadPackage(pkg);
				}
			}

			var stopwatch = new Stopwatch();

			// find classes
			var classes = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.GetMethods().Any(m => m.GetCustomAttributes<Test>().Any()));

			PrintWithColor($"Displaying averages of {ITERATIONS} runs.", ConsoleColor.Cyan);
			
			foreach(var type in classes)
			{
				var suiteObj = Activator.CreateInstance(type);
				var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
				PrintWithColor(type.ToString() + ":", ConsoleColor.Yellow);

				foreach(var method in methods)
				{
					var test = method.GetCustomAttribute<Test>();
					Console.Write("\t" + test.Name + ": ");

					var totalSpan = new TimeSpan(0, 0, 0, 0, 0);
					
					for(var i = 0; i < ITERATIONS; i++)
					{
						stopwatch.Reset();
						stopwatch.Start();
						
						var result = method.Invoke(suiteObj, new object[] { rant });

						stopwatch.Stop();

						totalSpan += stopwatch.Elapsed;
					}

					var avgSpan = TimeSpan.FromMilliseconds(totalSpan.TotalMilliseconds / ITERATIONS);
					PrintWithColor(avgSpan.ToString("c"), ConsoleColor.Magenta);
				}

				PrintWithColor("Done", ConsoleColor.Green);
			}
		}

		private static void PrintWithColor(string str, ConsoleColor color)
		{
			var oldColor = Console.ForegroundColor;
			Console.ForegroundColor = color;
			Console.WriteLine(str);
			Console.ForegroundColor = oldColor;
		}
	}
}