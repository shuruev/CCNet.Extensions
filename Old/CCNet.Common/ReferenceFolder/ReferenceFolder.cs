using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CCNet.Common
{
	/// <summary>
	/// Common methods for working with reference folders.
	/// </summary>
	public static class ReferenceFolder
	{
		/// <summary>
		/// Gets latest reference version.
		/// </summary>
		public static string GetLatestVersion(string referenceFolder, string referenceProject)
		{
			string path = Path.Combine(referenceFolder, referenceProject);
			path = Path.Combine(path, "LatestLabel.txt");
			return File.ReadAllText(path).Trim();
		}

		/// <summary>
		/// Gets path for latest reference version.
		/// </summary>
		public static string GetLatestPath(string referenceFolder, string referenceProject)
		{
			string subPath = Path.Combine(referenceFolder, referenceProject);
			return Path.Combine(subPath, "Latest");
		}

		/// <summary>
		/// Gets names for all binary files in reference folder.
		/// </summary>
		public static List<ReferenceFile> GetAllFiles(string referenceFolder)
		{
			List<ReferenceFile> files = new List<ReferenceFile>();

			foreach (string dir in Directory.GetDirectories(referenceFolder))
			{
				string latestVersion = GetLatestVersion(referenceFolder, dir);
				string latestPath = GetLatestPath(referenceFolder, dir);
				foreach (string file in Directory.GetFiles(latestPath, "*.dll")
					.Union(Directory.GetFiles(latestPath, "*.exe")))
				{
					files.Add(new ReferenceFile
					{
						ProjectName = Path.GetFileName(dir),
						FilePath = file,
						FileName = Path.GetFileName(file),
						AssemblyName = Path.GetFileNameWithoutExtension(file),
						Version = latestVersion
					});
				}
			}

			return files;
		}
	}
}
