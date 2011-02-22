using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CCNet.Coverage2Xml
{
	/// <summary>
	/// Class for extensions.
	/// </summary>
	public static class StringExtensions
	{
		/// <summary>
		/// Shortens method name.
		/// </summary>
		public static string ShortenMethodName(this string str)
		{
			return str.ShortenInsideArguments('(');
		}

		/// <summary>
		/// Shortens arguments inside method/generic type.
		/// </summary>
		private static string ShortenInsideArguments(this string argument, char openBracket)
		{
			int bracketIndex = argument.IndexOf(openBracket);
			if (bracketIndex == -1)
				return argument;

			string arguments = argument.Substring(bracketIndex + 1, argument.Length - bracketIndex - 2);
			if (string.IsNullOrEmpty(arguments))
				return argument;

			string shortenedArguments = arguments.ShortenArgumentsString();

			return argument.Replace(arguments, shortenedArguments);
		}

		/// <summary>
		/// Shortens arguments in method names.
		/// </summary>
		private static string ShortenArgumentsString(this string arguments)
		{
			string[] args = arguments.GetArgumentsArray();
			if (args == null)
				return arguments;

			List<string> shortenedArgs = new List<string>();
			foreach (var arg in args)
			{
				int lastDotIndex = arg.GetLastDotIndex();
				if (lastDotIndex == -1)
				{
					shortenedArgs.Add(arg);
					continue;
				}

				string shortenededArg = arg.Remove(0, lastDotIndex + 1);
				shortenededArg = shortenededArg.ShortenInsideArguments('<');
				shortenedArgs.Add(shortenededArg);
			}

			return String.Join(", ", shortenedArgs);
		}

		/// <summary>
		/// Returns index of method's last dot.
		/// </summary>
		private static int GetLastDotIndex(this string argument)
		{
			int openBracketIndex = argument.IndexOf("<");
			if (openBracketIndex == -1)
				return argument.LastIndexOf(".");

			return argument.LastIndexOf(".", openBracketIndex);
		}

		/// <summary>
		/// Returns array of arguments.
		/// </summary>
		private static string[] GetArgumentsArray(this string arguments)
		{
			Regex regex = new Regex(@"[^,\<\>]*(\<.*\>)?[,]?");

			List<string> result = new List<string>();
			var matches = regex.Matches(arguments);
			foreach (Match match in matches)
			{
				string value = match.Value;
				if (string.IsNullOrEmpty(value))
					continue;

				result.Add(value.TrimEnd(','));
			}

			return result.ToArray();
		}

		/// <summary>
		/// Removes wrong substrings.
		/// </summary>
		public static string RemoveWrongSymbols(this string str)
		{
			string result = str;
			result = result.Replace("`1", string.Empty);
			result = result.Replace("`2", string.Empty);
			return result;
		}
	}
}
