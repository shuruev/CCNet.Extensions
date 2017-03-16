using CCNet.Build.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CCNet.Build.SetupPackages
{
	public class NuGetSubstitute
	{
		private readonly NuGetDb m_db;
		private List<NuGetPackage> m_thisBranchPackages;

		public NuGetSubstitute(string nugetDbConnection)
		{
			m_db = new NuGetDb(nugetDbConnection);

			m_thisBranchPackages = m_db.GetLatestVersions()
				.Where(p => p.Branch == Args.BranchName.ToLower())
				.ToList();
		}

		private List<BinaryReference> GetProjectBinaryReferences()
		{
			var project = new ProjectDocument(Args.ProjectFile);
			return project.GetBinaryReferences();
		}
		
		public void RestoreBranchedPackages()
		{
			Console.WriteLine("Restoring packages...");

			if (String.IsNullOrEmpty(Args.PackagesPath))
				throw new InvalidOperationException("Packages path is not set.");

			string mainNuGetUrl = Args.NuGetUrl;
			if (Args.BranchName != null)
			{
				string branchNuGetUrl = Args.NuGetUrl.Split(new[] { ';' })[0];
				var referenceNames = GetProjectBinaryReferences().Select(x => x.AssemblyName.Name).ToList();
				var branchedPackages = m_thisBranchPackages.Where(x => referenceNames.Contains(x.Id)).ToList();

				foreach (var package in branchedPackages)
				{
					InstallNuGutPackage(package, branchNuGetUrl);
				}
			}
		}

		private void InstallNuGutPackage(NuGetPackage package, string branchNuGetUrl)
		{
			var downloadUrl = $"{branchNuGetUrl}/package/__{package.Branch.ToLower()}__{package.Id}/{package.Version}";
			var packageName = $"{Args.PackagesPath}\\{package.Id}.{package.Version}\\{package.Id}.{package.Version}.nupkg";

			ClearPackage(packageName);
			(new WebClient()).DownloadFile(downloadUrl, packageName);
			UnZipPackage(packageName);
		}

		private void ClearPackage(string packageName)
		{
			var packagePath = Path.GetDirectoryName(packageName);

			if (Directory.Exists(packagePath))
			{
				Directory.Delete(packagePath, true);
			}

			Directory.CreateDirectory(packagePath);

			if (File.Exists(packageName))
			{
				File.Delete(packageName);
			}
		}

		private void UnZipPackage(string packageName)
		{
			using (FileStream zipToOpen = new FileStream(packageName, FileMode.Open))
			{
				using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
				{
					List<ZipArchiveEntry> entries = archive.Entries.Where(x => x.FullName.StartsWith("lib/")).ToList();
					foreach (var entry in entries)
					{
						var entryName = Path.Combine(
							Path.GetDirectoryName(packageName),
							entry.FullName);

						var entryPath = Path.GetDirectoryName(entryName);

						if (!Directory.Exists(entryPath))
						{
							Directory.CreateDirectory(entryPath);
						}

						using (var fileStream = new FileStream(entryName, FileMode.Create, FileAccess.Write))
						{
							entry.Open().CopyTo(fileStream);
						}
					}
				}
			}
		}
	}
}
