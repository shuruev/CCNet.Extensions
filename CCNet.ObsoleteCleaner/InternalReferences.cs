using System.Collections.Generic;
using System.IO;

namespace CCNet.ObsoleteCleaner
{
	/// <summary>
	/// Contains the logic of cleaning up of "InternalReferences" folder.
	/// </summary>
	public static class InternalReferences
	{
		private const string c_latestVersionFileName = "LatestLabel.txt";
		private const string c_latestFolderName = "Latest";

		/// <summary>
		/// Removes folders inside "Arguments.InternalReferencesPath" folder older than "Arguments.DaysToLive" days.
		/// </summary>
		public static void Clean()
		{
			var projects = Directory.EnumerateDirectories(Arguments.InternalReferencesPath);
			foreach (var project in projects)
			{
				CleanProjectFolder(project);
			}
		}

		/// <summary>
		/// Removes all folders inside <paramref name="projectFolder"/> folder older than "Arguments.DaysToLive" days.
		/// </summary>
		private static void CleanProjectFolder(string projectFolder)
		{
			string latestVersion = File.ReadAllText(
				Path.Combine(
					projectFolder,
					c_latestVersionFileName)).TrimEnd();

			IEnumerable<string> versionsToDelete = GetFoldersToDelete(
				projectFolder,
				latestVersion);

			foreach (var version in versionsToDelete)
			{
				Directory.Delete(version, true);
			}
		}

		/// <summary>
		/// Gets list folders to remove.
		/// </summary>
		private static IEnumerable<string> GetFoldersToDelete(
			string projectFolder,
			string latestVersion)
		{
			var foldersToDelete = new List<string>();

			var versionFolders = Directory.EnumerateDirectories(projectFolder);
			foreach (var versionFolder in versionFolders)
			{
				var versionFolderName = Path.GetFileName(versionFolder);

				if (versionFolderName == latestVersion
					|| versionFolderName == c_latestFolderName)
				{
					continue;
				}

				var date = ObsoleteHelper.ConvertVersionToDate(versionFolderName);
				if (!date.HasValue)
				{
					continue;
				}

				if (!ObsoleteHelper.IsObsolete(
					date.Value,
					Arguments.DaysToLive))
				{
					continue;
				}

				foldersToDelete.Add(versionFolder);
			}

			return foldersToDelete;
		}
	}
}
