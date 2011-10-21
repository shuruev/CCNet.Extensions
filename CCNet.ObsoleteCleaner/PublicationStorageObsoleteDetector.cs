using System.Collections.Generic;
using System.IO;
using CCNet.Releaser.Client;

namespace CCNet.ObsoleteCleaner
{
	/// <summary>
	/// Contains the logic of cleaning up of "PublicationStorage" folder.
	/// </summary>
	public class PublicationStorageObsoleteDetector : IObsoleteDetector
	{
		private readonly IReleaserClient m_releaserClient;

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public PublicationStorageObsoleteDetector()
		{
			m_releaserClient = new ReleaserClient();
		}

		/// <summary>
		/// Gets the list of obsolete subfolders.
		/// Returns false if project path is unknown.
		/// </summary>
		public bool GetObsoleteSubfolders(string projectPath, out List<string> obsoleteSubfolders)
		{
			obsoleteSubfolders = new List<string>();

			var projectName = Path.GetFileName(projectPath);

			List<string> releases;
			bool ok = m_releaserClient.GetReleases(projectName, out releases);

			if (!ok)
				return false;

			obsoleteSubfolders = ObsoleteHelper.GetObsoletePaths(
				Directory.GetDirectories(projectPath),
				releases,
				Arguments.DaysToLive);

			return true;
		}
	}
}
