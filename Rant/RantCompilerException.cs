using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Rant.Core.Stringes;

using static Rant.Localization.Txtres;

namespace Rant
{
	/// <summary>
	/// Represents an error raised by Rant during pattern compilation.
	/// </summary>
	public sealed class RantCompilerException : Exception
	{
		private readonly List<RantCompilerMessage> _errorList;

		/// <summary>
		/// The name of the source pattern on which the error occurred.
		/// </summary>
		public string SourceName { get; }

		/// <summary>
		/// Indicates whether the exception is the result of an internal engine error.
		/// </summary>
		public bool InternalError { get; }

		internal RantCompilerException(string sourceName, List<RantCompilerMessage> errorList)
			: base(GenerateErrorString(errorList))
		{
			_errorList = errorList;
			SourceName = sourceName;
			InternalError = false;
		}

		internal RantCompilerException(string sourceName, List<RantCompilerMessage> errorList, Exception innerException)
			: base(GenerateErrorStringWithInnerEx(errorList, innerException), innerException)
		{
			_errorList = errorList;
			SourceName = sourceName;
			InternalError = true;
		}

		private static string GenerateErrorString(List<RantCompilerMessage> list)
		{
			var writer = new StringBuilder();
			if (list.Count > 1)
			{
				writer.AppendLine(GetString("compiler-errors-found", list.Count));
				foreach (var error in list)
				{
					writer.Append('\t');
					writer.AppendLine(error.ToString());
				}
			}
			else
			{
				writer.Append(list.First());
			}
			return writer.ToString();
		}

		private static string GenerateErrorStringWithInnerEx(List<RantCompilerMessage> list, Exception inner)
		{
			var writer = new StringBuilder();
			writer.AppendLine(GetString("err-compiler-internal", inner.GetType().Name, inner.Message));
			
			if (list != null && list.Any())
			{
				writer.AppendLine();
				if (list.Count > 1)
				{
					writer.AppendLine(GetString("compiler-errors-also-found", list.Count));
					foreach (var error in list)
					{
						writer.Append('\t');
						writer.AppendLine(error.ToString());
					}
				}
				else
				{
					writer.AppendLine(GetString("compiler-error-also-found"));
					writer.Append('\t');
					writer.AppendLine(list.First().ToString());
				}
			}
			return writer.ToString();
		}

		/// <summary>
		/// Enumerates the errors collected from the compiler.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<RantCompilerMessage> GetErrors()
		{
			if (_errorList == null) yield break;
			foreach (var error in _errorList) yield return error;
		}
	}
}