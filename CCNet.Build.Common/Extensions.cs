using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace CCNet.Build.Common
{
	/// <summary>
	/// Some extension methods.
	/// </summary>
	public static class Extensions
	{
		/// <summary>
		/// Gets detailed date/time description in two time zones.
		/// </summary>
		public static string ToDetailedString(this DateTime dateTime)
		{
			return String.Format(
				CultureInfo.InvariantCulture,
				"{0:yyyy-MM-dd}   {1:HH:mm:ss} MSK   {2:HH:mm:ss} PST",
				dateTime.ToUniversalTime(),
				TimeZoneInfo.ConvertTimeBySystemTimeZoneId(dateTime, "Russian Standard Time"),
				TimeZoneInfo.ConvertTimeBySystemTimeZoneId(dateTime, "Pacific Standard Time"));
		}

		/// <summary>
		/// Converts version to a form where all four components are specified.
		/// </summary>
		public static Version Normalize(this Version version)
		{
			if (version == null)
				return null;

			if (version.Build >= 0 && version.Revision >= 0)
				return version;

			return new Version(
				version.Major,
				version.Minor,
				Math.Max(version.Build, 0),
				Math.Max(version.Revision, 0));
		}

		/// <summary>
		/// Creates directory if it does not exist.
		/// </summary>
		public static void CreateDirectoryIfNotExists(this string path)
		{
			if (Directory.Exists(path))
				return;

			Directory.CreateDirectory(path);
		}

		/// <summary>
		/// Converts all whitespace characters to spaces and removes double whitespaces.
		/// </summary>
		public static string CleanWhitespaces(this string text)
		{
			var sb = new StringBuilder();
			bool whitespace = false;
			foreach (char c in text.Trim())
			{
				if (!Char.IsWhiteSpace(c))
				{
					whitespace = false;
					sb.Append(c);
					continue;
				}

				if (whitespace)
					continue;

				whitespace = true;
				sb.Append(' ');
			}

			return sb.ToString();
		}

		/// <summary>
		/// Filters out ASCII characters only, including some other specified characters.
		/// </summary>
		public static string AsciiOnly(this string text, params char[] includeAlso)
		{
			var map = new HashSet<char>(includeAlso);

			var sb = new StringBuilder();
			foreach (var c in text)
			{
				if ((c >= '0' && c <= '9')
					|| (c >= 'A' && c <= 'Z')
					|| (c >= 'a' && c <= 'z')
					|| map.Contains(c))
				{
					sb.Append(c);
				}
			}

			return sb.ToString();
		}

		/// <summary>
		/// Removes specified text from the beginning of the original string.
		/// </summary>
		public static string RemoveFromStart(this string originalText, string textToRemove)
		{
			if (!originalText.StartsWith(textToRemove))
				throw new InvalidOperationException(
					String.Format("The original text is expected to start with '{0}', but it doesn't.", textToRemove));

			return originalText.Substring(textToRemove.Length);
		}

		/// <summary>
		/// Removes specified text from the ending of the original string.
		/// </summary>
		public static string RemoveFromEnd(this string originalText, string textToRemove)
		{
			if (!originalText.EndsWith(textToRemove))
				throw new InvalidOperationException(
					String.Format("The original text is expected to end with '{0}', but it doesn't.", textToRemove));

			return originalText.Substring(0, originalText.Length - textToRemove.Length);
		}
	}
}
