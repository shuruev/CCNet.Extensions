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
		/// Gets list of obsolete subfolders.
		/// </summary>
		public IEnumerable<string> GetObsoleteSubfolders(string projectPath)
		{
			var projectName = Path.GetFileName(projectPath);
			var releases = m_releaserClient.GetReleases(projectName);

			var obsoleteSubfolders = ObsoleteHelper.GetObsoletePaths(
				Directory.GetDirectories(projectPath),
				releases,
				Arguments.DaysToLive);

			return obsoleteSubfolders;
		}
	}
}
