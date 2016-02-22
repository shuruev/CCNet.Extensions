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

		private NuGetPackage ReadPackage(XElement element)
		{
			var id = element.Attribute("id").Value;
			var version = element.Attribute("version").Value;
			return new NuGetPackage(id, version);
		}

		public void PreAdjust()
		{
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
				var package = ReadPackage(element);

				m_log.Add(
					package.Name,
					new LogPackage
					{
						Name = package.Name,
						IsLocal = m_checker.IsLocal(package.Name),
						SourceVersion = package.Version,
						PinnedToCurrent = m_checker.IsPinnedToCurrentVersion(package.Name),
						PinnedToSpecific = m_checker.IsPinnedToSpecificVersion(package.Name)
					});
			}

			Console.WriteLine("OK");
		}

		private void PostAnalyzePackages(XDocument config)
		{
			Console.Write("Post-analyzing packages... ");

			foreach (var element in config.Root.Elements("package"))
			{
				var package = ReadPackage(element);

				m_log[package.Name].BuildVersion = package.Version;
			}

			Console.WriteLine("OK");
		}

		private void UpdatePackageVersions(XDocument config)
		{
			Console.Write("Updating package versions... ");

			foreach (var element in config.Root.Elements("package"))
			{
				var package = ReadPackage(element);

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
			string xml = File.ReadAllText(Paths.PackagesConfig);
			var config = XDocument.Parse(xml);

			PostAnalyzePackages(config);

			config.Save(Paths.PackagesConfig);
		}
	}
}
