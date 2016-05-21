using System;
using System.IO;
using System.Xml.Linq;
using CCNet.Build.Common;

namespace CCNet.Build.SetupPackages
{
	public class NuGetHelper
	{
		private readonly PackageChecker m_checker;

		public NuGetHelper(PackageChecker checker)
		{
			if (checker == null)
				throw new ArgumentNullException("checker");

			m_checker = checker;
		}

		public void UpdateAll()
		{
			if (!File.Exists(Paths.PackagesConfig))
				return;

			Console.WriteLine("Update remote packages...");

			string xml = File.ReadAllText(Paths.PackagesConfig);
			var doc = XDocument.Parse(xml);

			foreach (var element in doc.Root.Elements("package"))
			{
				var package = new NuGetPackage(element);

				// skip local packages
				if (m_checker.IsLocal(package.Id))
					continue;

				// package should be pinned to its current version
				if (m_checker.IsPinnedToCurrentVersion(package.Id))
					continue;

				// try to update remote package
				UpdatePackage(package.Id);
			}
		}

		public void RestoreAll()
		{
			if (!File.Exists(Paths.PackagesConfig))
				return;

			RestorePackages();
		}

		private static void UpdatePackage(string id)
		{
			Console.WriteLine("Updating {0}...", id);

			Run(
				@"update ""{0}"" -RepositoryPath ""{1}"" -Id ""{2}"" -MSBuildVersion 14 -NonInteractive -Verbosity Detailed",
				Paths.PackagesConfig,
				Args.PackagesPath,
				id);
		}

		private static void RestorePackages()
		{
			Console.WriteLine("Restoring packages...");

			Run(
				@"restore ""{0}"" -PackagesDirectory ""{1}"" -Source ""{2};http://www.nuget.org/api/v2"" -MSBuildVersion 14 -NonInteractive -Verbosity Detailed",
				Paths.PackagesConfig,
				Args.PackagesPath,
				Args.NuGetUrl);
		}

		private static void Run(string format, params object[] args)
		{
			var runArguments = String.Format(format, args);
			Execute.Run(Args.NuGetExecutable, runArguments);
		}
	}
}
