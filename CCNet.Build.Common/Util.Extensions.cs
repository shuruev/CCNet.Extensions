using System;
using System.Text.RegularExpressions;

namespace CCNet.Build.Common
{
	/// <summary>
	/// Some utility methods which are hard to be categorized yet.
	/// </summary>
	public static partial class Util
	{
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
		/// Performs case-insensitive string replacement.
		/// </summary>
		public static string ReplaceIgnoreCase(this string input, string search, string replacement)
		{
			// xxx TODO: put into Atom
			// xxx maybe this is faster http://stackoverflow.com/questions/6025560/how-to-ignore-case-in-string-replace
			return Regex.Replace(
				input,
				Regex.Escape(search),
				replacement.Replace("$", "$$"),
				RegexOptions.IgnoreCase
			);
		}
	}
}
