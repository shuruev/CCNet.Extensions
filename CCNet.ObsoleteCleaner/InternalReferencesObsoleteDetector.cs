using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CCNet.Common;

namespace CCNet.ObsoleteCleaner
{
	/// <summary>
	/// Contains the logic of cleaning up of "InternalReferences" folder.
	/// </summary>
	public class InternalReferencesObsoleteDetector : IObsoleteDetector
	{
		/// <summary>
		/// Gets list of obsolete subfolders.
		/// </summary>
		public IEnumerable<string> GetObsoleteSubfolders(string projectPath)
		{
			string projectFolder = Path.GetFileName(projectPath);

			string latestVersion = ReferenceFolder.GetLatestVersion(
				Arguments.InternalReferencesPath,
				projectFolder);

			string latestFolderName = Path.GetFileName(
				ReferenceFolder.GetLatestPath(
					Arguments.InternalReferencesPath,
					projectFolder));

			var obsoleteSubfolders = ObsoleteHelper.GetObsoletePaths(
				Directory.GetDirectories(projectPath),
				new List<string>
				{
					latestVersion,
					latestFolderName
				},
				Arguments.DaysToLive);

			return obsoleteSubfolders;
		}
	}
}
