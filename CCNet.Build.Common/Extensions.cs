using System;
using System.Globalization;
using System.IO;

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
	}
}
