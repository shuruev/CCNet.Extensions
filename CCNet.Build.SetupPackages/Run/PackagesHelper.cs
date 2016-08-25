using System;
using System.IO;
using System.Linq;
using CCNet.Build.Common;

namespace CCNet.Build.SetupPackages
{
	public class PackagesHelper
	{
		private readonly PackageChecker m_checker;
		private readonly LogPackages m_log;

		public PackagesHelper(PackageChecker checker, LogPackages log)
		{
			if (checker == null)
				throw new ArgumentNullException(nameof(checker));

			if (log == null)
				throw new ArgumentNullException(nameof(log));

			m_checker = checker;
			m_log = log;
		}

		public void Adjust()
		{
			if (!File.Exists(Paths.PackagesConfig))
				return;

			var config = new PackagesConfig(Paths.PackagesConfig);

			AnalyzePackages(config);
			UpdatePackageVersions(config);

			config.Save();
		}

		private void AnalyzePackages(PackagesConfig config)
		{
			Console.Write("Analyzing packages... ");

			foreach (var package in config.AllPackages().Select(e => e.AsPackage()))
			{
				var log = new LogPackage
				{
					PackageId = package.Id,
					ProjectName = m_checker.ProjectName(package.Id),
					IsLocal = m_checker.IsLocal(package.Id),
					IsStatic = m_checker.IsStatic(package.Id),
					SourceVersion = package.Version,
					PinnedToCurrent = m_checker.IsPinnedToCurrentVersion(package.Id),
					PinnedToSpecific = m_checker.IsPinnedToSpecificVersion(package.Id),
					IsDependency = m_checker.IsDependency(package.Id),
					IsBundle = m_checker.IsBundle(package.Id)
				};

				SetupProjectUrl(log);

				m_log.Add(package.Id, log);
			}

			Console.WriteLine("OK");
		}

		private void SetupProjectUrl(LogPackage package)
		{
			if (!package.IsLocal)
			{
				package.ProjectUrl = $"https://www.nuget.org/packages/{package.PackageId}/";
				return;
			}

			if (package.IsStatic)
			{
				package.ProjectUrl = $"https://rufc-devbuild.cneu.cnwk/nuget/packages/{package.PackageId}/";
				return;
			}

			package.ProjectUrl = $"http://rufc-devbuild.cneu.cnwk/ccnet/server/Library/project/{package.ProjectName}/ViewProjectReport.aspx";
		}

		private void UpdatePackageVersions(PackagesConfig config)
		{
			Console.Write("Updating package versions... ");

			foreach (var element in config.AllPackages())
			{
				var package = element.AsPackage();

				// skip remote packages
				if (!m_checker.IsLocal(package.Id))
					continue;

				// skip static packages
				if (m_checker.IsStatic(package.Id))
					continue;

				// package should be pinned to its current version
				if (m_checker.IsPinnedToCurrentVersion(package.Id))
					continue;

				// get version to use for local package
				var versionToUse = m_checker.VersionToUse(package.Id);

				// update is not required
				if (versionToUse.Normalize() == package.Version.Normalize())
					continue;

				// update package version within packages configuration
				element.Attribute("version").Value = versionToUse.ToString();
			}

			Console.WriteLine("OK");
		}
	}
}
