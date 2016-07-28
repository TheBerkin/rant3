using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Rant.Localization
{
	internal static class Txtres
	{
		public const string LanguageResourceNamespace = "Rant.Localization";
		public const string FallbackLanguageCode = "en-US";

		private static readonly Dictionary<string, string> _stringTable = new Dictionary<string, string>();

		static Txtres()
		{
			try
			{
				var ass = Assembly.GetExecutingAssembly();
				using (var stream =
					ass.GetManifestResourceStream($"{LanguageResourceNamespace}.{CultureInfo.CurrentCulture.Name}.lang")
					?? ass.GetManifestResourceStream($"{LanguageResourceNamespace}.{FallbackLanguageCode}.lang"))
				{
					if (stream == null)
					{
#if DEBUG
						Console.WriteLine($"Txtres error: Missing language definition Localization/{CultureInfo.CurrentCulture.Name}.lang");
#endif
						return;
					}

					using (var reader = new StreamReader(stream))
					{
						loop:
						while (!reader.EndOfStream)
						{
							var line = reader.ReadLine();
							if (line == null || line.Length == 0) continue;
							var kv = line.Split(new[] { '=' }, 2);
							if (kv.Length != 2) continue;
							var key = kv[0].Trim();
							if (!key.All(c => Char.IsLetterOrDigit(c) || c == '-' || c == '_')) continue;
							var valueLiteral = kv[1].Trim();
							var sb = new StringBuilder();
							char esc;
							int i = 0;
							int len = valueLiteral.Length;
							while (i < len)
							{
								if ((i == 0 || i == valueLiteral.Length - 1))
								{
									if (valueLiteral[i] != '"') goto loop;
									i++;
									continue;
								}
								switch (valueLiteral[i])
								{
									case '\\':
										{
											if (i == valueLiteral.Length - 1) goto loop;
											switch (valueLiteral[i + 1])
											{
												case 'a':
													sb.Append('\a');
													break;
												case 'b':
													sb.Append('\b');
													break;
												case 'f':
													sb.Append('\f');
													break;
												case 'n':
													sb.Append('\n');
													break;
												case 'r':
													sb.Append('\r');
													break;
												case 't':
													sb.Append('\t');
													break;
												case 'v':
													sb.Append('\v');
													break;
												case 'u':
													{
														if (i + 5 >= valueLiteral.Length) goto loop;
														short code;
														if (!short.TryParse(valueLiteral.Substring(i + 1, 4),
															NumberStyles.AllowHexSpecifier,
															CultureInfo.InvariantCulture, out code)) goto loop;
														sb.Append((char)code);
														i += 6;
														continue;
													}
												default:
													sb.Append(valueLiteral[i + 1]);
													break;
											}
											i += 2;
											continue;
										}
									default:
										sb.Append(valueLiteral[i]);
										break;
								}
								i++;
							}
							_stringTable[key] = sb.ToString();
						}
					}
				}
#if DEBUG
				Console.WriteLine($"Loaded string resources for {CultureInfo.CurrentCulture.Name}");
#endif
			}
			catch (Exception ex)
			{
#if DEBUG
				Console.WriteLine($"Txtres error: {ex.Message}");
#endif
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		public static void ForceLoad()
		{
		}

		public static string GetString(string name)
		{
			string str;
			return _stringTable.TryGetValue(name, out str) ? str : name;
		}

		public static string GetString(string name, params object[] args)
		{
			string str;
			try
			{
				return _stringTable.TryGetValue(name, out str) ? String.Format(str, args) : name;
			}
			catch
			{
				return "<Format Error>";
			}
		}
	}
}