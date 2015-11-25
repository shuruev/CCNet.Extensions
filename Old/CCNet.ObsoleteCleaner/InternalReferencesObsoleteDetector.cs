using System.Collections.Generic;
using System.IO;
using CCNet.Common;

namespace CCNet.ObsoleteCleaner
{
	/// <summary>
	/// Contains the logic of cleaning up of "InternalReferences" folder.
	/// </summary>
	public class InternalReferencesObsoleteDetector : IObsoleteDetector
	{
		/// <summary>
		/// Gets the list of obsolete subfolders.
		/// Returns false if project path is unknown.
		/// </summary>
		public bool GetObsoleteSubfolders(string projectPath, out List<string> obsoleteSubfolders)
		{
			string projectFolder = Path.GetFileName(projectPath);

			string latestVersion = ReferenceFolder.GetLatestVersion(
				Arguments.InternalReferencesPath,
				projectFolder);

			string latestFolderName = Path.GetFileName(
				ReferenceFolder.GetLatestPath(
					Arguments.InternalReferencesPath,
					projectFolder));

			obsoleteSubfolders = ObsoleteHelper.GetObsoletePaths(
				Directory.GetDirectories(projectPath),
				new List<string>
				{
					latestVersion,
					latestFolderName
				},
				Arguments.DaysToLive);

			return true;
		}
	}
}
