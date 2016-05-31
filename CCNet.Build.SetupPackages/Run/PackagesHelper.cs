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
				package.ProjectUrl = String.Format("https://www.nuget.org/packages/{0}/", package.PackageId);
				return;
			}

			if (package.IsStatic)
			{
				package.ProjectUrl = String.Format("https://rufc-devbuild.cneu.cnwk/nuget/packages/{0}/", package.PackageId);
				return;
			}

			package.ProjectUrl = String.Format("http://rufc-devbuild.cneu.cnwk/ccnet/server/Library/project/{0}/ViewProjectReport.aspx", package.ProjectName);
		}

		private void PostAnalyzePackages(XDocument config)
		{
			Console.Write("Post-analyzing packages... ");

			foreach (var element in config.Root.Elements("package"))
			{
				var package = new NuGetPackage(element);

				m_log[package.Id].BuildVersion = package.Version;
			}

			foreach (var item in m_log.Values)
			{
				if (item.BuildVersion == null)
					throw new InvalidOperationException(
						String.Format("Build version is missing for package '{0}'.", item.PackageId));
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
