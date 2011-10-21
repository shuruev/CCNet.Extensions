using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CCNet.ObsoleteCleaner.Properties;

namespace CCNet.ObsoleteCleaner
{
	/// <summary>
	/// Folder to be cleaned up.
	/// </summary>
	public class CleaningFolder
	{
		private readonly IObsoleteDetector m_obsoleteDetector;
		private readonly string m_rootFolder;

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public CleaningFolder(
			string rootFolder,
			IObsoleteDetector obsoleteDetector)
		{
			m_rootFolder = rootFolder;
			m_obsoleteDetector = obsoleteDetector;
		}

		/// <summary>
		/// Removes obsolete subfolders inside root folder.
		/// </summary>
		public void Clean()
		{
			var projectPaths = Directory.EnumerateDirectories(m_rootFolder);
			Parallel.ForEach(
				projectPaths,
				CleanProjectFolder);
		}

		/// <summary>
		/// Removes obsolete subfolders inside specified folder.
		/// </summary>
		private void CleanProjectFolder(string projectPath)
		{
			List<string> obsoleteSubfolders;
			bool ok = m_obsoleteDetector.GetObsoleteSubfolders(
				projectPath,
				out obsoleteSubfolders);

			if (!ok)
			{
				Console.WriteLine(
					Resources.UnknownProjectWarning,
					Path.GetFileName(projectPath));
				return;
			}

			foreach (var path in obsoleteSubfolders)
			{
				Directory.Delete(
					path,
					true);
			}
		}
	}
}
