using System.Collections.Generic;
using System.IO;
using CCNet.Common;

namespace CCNet.ObsoleteCleaner
{
	/// <summary>
	/// Contains the logic of cleaning up of "InternalReferences" folder.
	/// </summary>
	public static class InternalReferences
	{
		/// <summary>
		/// Removes folders inside "Arguments.InternalReferencesPath" folder older than "Arguments.DaysToLive" days.
		/// </summary>
		public static void Clean()
		{
			var projectPaths = Directory.EnumerateDirectories(Arguments.InternalReferencesPath);
			foreach (var projectPath in projectPaths)
			{
				CleanProjectFolder(projectPath);
			}
		}

		/// <summary>
		/// Removes subfolders inside specified folder older than "Arguments.DaysToLive" days.
		/// </summary>
		private static void CleanProjectFolder(string projectPath)
		{
			IEnumerable<string> obsoleteSubfolders = GetObsoleteSubfolders(projectPath);

			foreach (var path in obsoleteSubfolders)
			{
				Directory.Delete(
					path,
					true);
			}
		}

		/// <summary>
		/// Gets list of obsolete subfolders.
		/// </summary>
		private static IEnumerable<string> GetObsoleteSubfolders(string projectPath)
		{
			string projectFolder = Path.GetFileName(projectPath);

			string latestVersion = ReferenceFolder.GetLatestVersion(
				Arguments.InternalReferencesPath,
				projectFolder);

			var latestPath = ReferenceFolder.GetLatestPath(
				Arguments.InternalReferencesPath,
				projectFolder);

			var foldersToDelete = new List<string>();

			var versionPaths = Directory.EnumerateDirectories(projectPath);
			foreach (var versionPath in versionPaths)
			{
				var versionFolder = Path.GetFileName(versionPath);

				if (versionFolder == latestVersion
					|| versionPath == latestPath)
				{
					continue;
				}

				var date = ObsoleteHelper.ConvertVersionToDate(versionFolder);
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

				foldersToDelete.Add(versionPath);
			}

			return foldersToDelete;
		}
	}
}
