using System;

namespace CCNet.ObsoleteCleaner
{
	/// <summary>
	/// Utility class with helpful methods to define obsolete files inside "InternalReferences" folder.
	/// </summary>
	public static class InternalReferencesHelper
	{
		public const string LatestVersionFileName = "LatestLabel.txt";
		public const string LatestFolderName = "Latest";

		/// <summary>
		/// Shows the value.
		/// </summary>
		public static bool IsObsolete(
			DateTime date,
			int daysToLive)
		{
			var period = TimeSpan.FromDays(daysToLive);
			return date + period < DateTime.Now;
		}

		/// <summary>
		/// Converts version folder name to date.
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
	}
}
