using System;
using System.Collections.Generic;
using System.IO;
using CCNet.Common;
using CCNet.ObsoleteCleaner.Properties;

namespace CCNet.ObsoleteCleaner
{
	public class Program
	{
		/// <summary>
		/// Main program.
		/// </summary>
		public static int Main(string[] args)
		{
			/*xxxargs = new[]
			{
				@"InternalReferencesPath=D:\InternalReferences",
				@"DaysToLive=30",
			};*/

			if (args == null || args.Length == 0)
			{
				DisplayUsage();
				return 0;
			}

			try
			{
				Arguments.Default = ArgumentProperties.Parse(args);

				CleanInternalReferences(
					Arguments.InternalReferencesPath,
					Arguments.DaysToLive);
			}
			catch (Exception e)
			{
				return ErrorHandler.Runtime(e);
			}

			return 0;
		}

		/// <summary>
		/// Removes folders inside <paramref nam="internalReferencesPath"/> folder older than <paramref name="daysToLive"/> days.
		/// </summary>
		private static void CleanInternalReferences(
			string internalReferencesPath,
			int daysToLive)
		{
			var projects = Directory.EnumerateDirectories(internalReferencesPath);
			foreach (var project in projects)
			{
				CleanProjectFolder(project, daysToLive);
			}
		}

		/// <summary>
		/// Removes all folders inside <paramref name="projectFolder"/> older than <paramref name="daysToLive"/> days.
		/// </summary>
		private static void CleanProjectFolder(string projectFolder, int daysToLive)
		{
			string latestVersion = File.ReadAllText(
				Path.Combine(
					projectFolder,
					InternalReferencesHelper.LatestVersionFileName)).TrimEnd();

			IEnumerable<string> versionsToDelete = GetFoldersToDelete(
				projectFolder,
				latestVersion,
				daysToLive);

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
			string latestVersion,
			int daysToLive)
		{
			var foldersToDelete = new List<string>();

			var versionFolders = Directory.EnumerateDirectories(projectFolder);
			foreach (var versionFolder in versionFolders)
			{
				var versionFolderName = Path.GetFileName(versionFolder);

				if (versionFolderName == latestVersion
					|| versionFolderName == InternalReferencesHelper.LatestFolderName)
				{
					continue;
				}

				var date = InternalReferencesHelper.ConvertVersionToDate(versionFolderName);
				if (!date.HasValue)
				{
					continue;
				}

				if (!InternalReferencesHelper.IsObsolete(date.Value, daysToLive))
				{
					continue;
				}

				foldersToDelete.Add(versionFolder);
			}

			return foldersToDelete;
		}

		/// <summary>
		/// Displays usage text.
		/// </summary>
		private static void DisplayUsage()
		{
			Console.WriteLine();
			Console.WriteLine(Resources.UsageInfo);
			Console.WriteLine();
		}
	}
}
