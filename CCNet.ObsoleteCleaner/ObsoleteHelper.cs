using System;
using System.Collections.Generic;
using System.IO;

namespace CCNet.ObsoleteCleaner
{
	/// <summary>
	/// Utility class with helpful methods for obsolescence detection.
	/// </summary>
	public static class ObsoleteHelper
	{
		/// <summary>
		/// Gets the value indicating whether <paramref name="date"/> is outdated.
		/// </summary>
		public static bool IsObsolete(
			DateTime date,
			int daysToLive)
		{
			var period = TimeSpan.FromDays(daysToLive);
			return date + period < DateTime.Now;
		}

		/// <summary>
		/// Converts <paramref name="version"/> string to the correspond date.
		/// </summary>
		public static DateTime? ConvertVersionToDate(string version)
		{
			var array = version.Split(new[] { '.' });
			if (array.Length != 4)
			{
				return null;
			}

			try
			{
				int year = int.Parse("20" + array[0]);
				int month = int.Parse(array[1]);
				int day = int.Parse(array[2]);

				return new DateTime(year, month, day);
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// Gets obsolete paths.
		/// </summary>
		public static List<string> GetObsoletePaths(
			IEnumerable<string> sourcePaths,
			IList<string> excludeVersions,
			int daysToLive)
		{
			var foldersToDelete = new List<string>();

			foreach (var versionPath in sourcePaths)
			{
				var versionFolder = Path.GetFileName(versionPath);

				if (excludeVersions.Contains(versionFolder))
				{
					continue;
				}

				var date = ConvertVersionToDate(versionFolder);
				if (!date.HasValue)
				{
					continue;
				}

				if (!IsObsolete(
					date.Value,
					daysToLive))
				{
					continue;
				}

				foldersToDelete.Add(versionPath);
			}

			return foldersToDelete;
		}
	}
}
