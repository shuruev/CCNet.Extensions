using System;
using System.IO;
using System.Xml.Linq;
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
				throw new ArgumentNullException("checker");

			if (log == null)
				throw new ArgumentNullException("log");

			m_checker = checker;
			m_log = log;
		}

		public void PreAdjust()
		{
			if (!File.Exists(Paths.PackagesConfig))
				return;

			string xml = File.ReadAllText(Paths.PackagesConfig);
			var config = XDocument.Parse(xml);

			PreAnalyzePackages(config);
			UpdatePackageVersions(config);

			config.Save(Paths.PackagesConfig);
		}

		private void PreAnalyzePackages(XDocument config)
		{
			Console.Write("Pre-analyzing packages... ");

			foreach (var element in config.Root.Elements("package"))
			{
				var package = new NuGetPackage(element);

				var log = new LogPackage
				{
					PackageName = package.Name,
					ProjectName = m_checker.ProjectName(package.Name),
					IsLocal = m_checker.IsLocal(package.Name),
					SourceVersion = package.Version,
					PinnedToCurrent = m_checker.IsPinnedToCurrentVersion(package.Name),
					PinnedToSpecific = m_checker.IsPinnedToSpecificVersion(package.Name)
				};

				SetupProjectUrl(log);

				m_log.Add(package.Name, log);
			}

			Console.WriteLine("OK");
		}

		private void SetupProjectUrl(LogPackage package)
		{
			if (package.IsLocal)
			{
				package.ProjectUrl = String.Format("http://rufc-devbuild.cneu.cnwk/ccnet/server/Library/project/{0}/ViewProjectReport.aspx", package.ProjectName);
			}
			else
			{
				package.ProjectUrl = String.Format("https://www.nuget.org/packages/{0}/", package.PackageName);
			}
		}

		private void PostAnalyzePackages(XDocument config)
		{
			Console.Write("Post-analyzing packages... ");

			foreach (var element in config.Root.Elements("package"))
			{
				var package = new NuGetPackage(element);

				m_log[package.Name].BuildVersion = package.Version;
			}

			foreach (var item in m_log.Values)
			{
				if (item.BuildVersion == null)
					throw new InvalidOperationException(
						String.Format("Build version is missing for package '{0}'.", item.PackageName));
			}

			Console.WriteLine("OK");
		}

		private void UpdatePackageVersions(XDocument config)
		{
			Console.Write("Updating package versions... ");

			foreach (var element in config.Root.Elements("package"))
			{
				var package = new NuGetPackage(element);

				// skip remote packages
				if (!m_checker.IsLocal(package.Name))
					continue;

				// package should be pinned to its current version
				if (m_checker.IsPinnedToCurrentVersion(package.Name))
					continue;

				// get version to use for local package
				var versionToUse = m_checker.VersionToUse(package.Name);

				// update is not required
				if (versionToUse.Normalize() == package.Version.Normalize())
					continue;

				// update package version within packages configuration
				element.Attribute("version").Value = versionToUse.ToString();
			}

			Console.WriteLine("OK");
		}

		public void PostAdjust()
		{
			if (!File.Exists(Paths.PackagesConfig))
				return;

			string xml = File.ReadAllText(Paths.PackagesConfig);
			var config = XDocument.Parse(xml);

			PostAnalyzePackages(config);

			config.Save(Paths.PackagesConfig);
		}
	}
}
