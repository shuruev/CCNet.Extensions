using System.IO;

namespace CCNet.Common
{
	/// <summary>
	/// Common methods for working with files.
	/// </summary>
	public static class FileHelper
	{
		/// <summary>
		/// Safely copies file, creating all necessary directories and overwriting destination.
		/// </summary>
		public static void CopyFile(string sourceFile, string destFile)
		{
			string destDir = Path.GetDirectoryName(destFile);
			if (!Directory.Exists(destDir))
				Directory.CreateDirectory(destDir);

			if (File.Exists(destFile))
			{
				FileInfo fi = new FileInfo(destFile);
				fi.Attributes = FileAttributes.Normal;
			}

			File.Copy(sourceFile, destFile, true);
		}

		/// <summary>
		/// Safely copies all content from one directory to another.
		/// </summary>
		public static void CopyDirectory(string sourceDir, string destDir)
		{
			foreach (string sourceFile in Directory.GetFiles(
				sourceDir,
				"*",
				SearchOption.AllDirectories))
			{
				string destFile = sourceFile.Replace(sourceDir, destDir);
				CopyFile(sourceFile, destFile);
			}
		}
	}
}
