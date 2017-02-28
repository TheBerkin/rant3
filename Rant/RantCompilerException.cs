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
using System.Linq;
using System.Text;

using static Rant.Localization.Txtres;

namespace Rant
{
	/// <summary>
	/// Represents an error raised by Rant during pattern compilation.
	/// </summary>
	public sealed class RantCompilerException : Exception
	{
		private readonly List<RantCompilerMessage> _errorList;

		internal RantCompilerException(string sourceName, List<RantCompilerMessage> errorList)
			: base(GenerateErrorString(errorList))
		{
			_errorList = errorList;
			ErrorCount = _errorList.Count;
			SourceName = sourceName;
			InternalError = false;
		}

		internal RantCompilerException(string sourceName, List<RantCompilerMessage> errorList, Exception innerException)
			: base(GenerateErrorStringWithInnerEx(errorList, innerException), innerException)
		{
			_errorList = errorList;
			ErrorCount = _errorList.Count;
			SourceName = sourceName;
			InternalError = true;
		}

		/// <summary>
		/// The name of the source pattern on which the error occurred.
		/// </summary>
		public string SourceName { get; }

		/// <summary>
		/// Indicates whether the exception is the result of an internal engine error.
		/// </summary>
		public bool InternalError { get; }

		/// <summary>
		/// Gets the number of errors returned by the compiler.
		/// </summary>
		public int ErrorCount { get; }

		private static string GenerateErrorString(List<RantCompilerMessage> list)
		{
			var writer = new StringBuilder();
			if (list.Count > 1)
			{
				writer.AppendLine(GetString("compiler-errors-found", list.Count));
				for (int i = 0; i < list.Count; i++)
				{
					var error = list[i];
					writer.Append($"    {i + 1}. ");
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
					for (int i = 0; i < list.Count; i++)
					{
						var error = list[i];
						writer.Append($"    {i + 1}. ");
						writer.AppendLine(error.ToString());
					}
				}
				else
				{
					writer.AppendLine(GetString("compiler-error-also-found"));
					writer.Append("    ");
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