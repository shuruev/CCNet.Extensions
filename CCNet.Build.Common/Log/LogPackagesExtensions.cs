using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CCNet.Build.Common;

namespace CCNet.Build.SetupPackages
{
	/// <summary>
	/// Some service methods which can be executed on a collection of referenced packages.
	/// </summary>
	public static class LogPackagesExtensions
	{
		/// <summary>
		/// Updates local references folder to represent the actual dependencies for all referenced packages.
		/// </summary>
		public static void SaveReferences(this LogPackages packages, string referencesPath)
		{
			referencesPath.CreateDirectoryIfNotExists();

			var after = packages.Values
				.Where(i => i.IsLocal)
				.ToDictionary(i => i.ProjectName);

			var before = Directory
				.GetFiles(referencesPath)
				.ToDictionary(Path.GetFileNameWithoutExtension);

			var toAdd = new List<string>();
			foreach (var reference in after)
			{
				if (before.ContainsKey(reference.Key))
					continue;

				var name = String.Format("{0}.txt", reference.Key);
				var path = Path.Combine(referencesPath, name);
				toAdd.Add(path);
			}

			var toRemove = new List<string>();
			foreach (var reference in before)
			{
				if (after.ContainsKey(reference.Key))
					continue;

				toRemove.Add(reference.Value);
			}

			foreach (var file in toRemove)
			{
				File.Delete(file);
			}

			foreach (var file in toAdd)
			{
				File.WriteAllText(file, String.Format("Created on {0}", DateTime.Now.ToDetailedString()));
			}
		}

		/// <summary>
		/// Saves the summary file with all referenced packages.
		/// </summary>
		public static void SaveSummary(this LogPackages packages, string tempPath)
		{
			tempPath.CreateDirectoryIfNotExists();

			var file = Path.Combine(tempPath, "packages.txt");
			File.WriteAllText(file, packages.Summary());
		}
	}
}
