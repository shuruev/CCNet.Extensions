using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CCNet.Common;
using CCNet.Releaser.Remoting;

namespace CCNet.ObsoleteCleaner
{
	/// <summary>
	/// Contains the logic of cleaning up of "PublicationStorage" folder.
	/// </summary>
	public static class PublicationStorage
	{
		/// <summary>
		/// Removes folders inside "Arguments.PublicationStorage" folder older than "Arguments.DaysToLive" days.
		/// </summary>
		public static void Clean(IReleaserClient releaserClient)
		{
			//var projectPaths = Directory.EnumerateDirectories(Arguments.);
			//foreach (var projectPath in projectPaths)
			//{
			//   CleanProjectFolder(releaserClient, projectPath);
			//}
		}

		/// <summary>
		/// Removes subfolders inside specified folder older than "Arguments.DaysToLive" days.
		/// </summary>
		private static void CleanProjectFolder(IReleaserClient releaserClient, string projectPath)
		{
			var releases = releaserClient.GetReleases("XX");

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

			var foldersToDelete = new List<string>();

			Directory.EnumerateDirectories(projectPath);

			return null;
			//return ObsoleteHelper.GetObsoletePaths(projectPath, latestVersion);
		}


	}
}
