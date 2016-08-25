using System;
using System.IO;
using System.Linq;
using CCNet.Build.Common;

namespace CCNet.Build.SetupPackages
{
	public class NuGetHelper
	{
		private readonly PackageChecker m_checker;
		private readonly LogPackages m_log;

		public NuGetHelper(PackageChecker checker, LogPackages log)
		{
			if (checker == null)
				throw new ArgumentNullException(nameof(checker));

			if (log == null)
				throw new ArgumentNullException(nameof(log));

			m_checker = checker;
			m_log = log;
		}

		public void UpdateAll()
		{
			if (!File.Exists(Paths.PackagesConfig))
				return;

			Console.WriteLine("Update remote packages...");

			var config = new PackagesConfig(Paths.PackagesConfig);
			foreach (var package in config.AllPackages().Select(e => e.AsPackage()))
			{
				// skip local packages
				if (m_checker.IsLocal(package.Id))
					continue;

				// package should be pinned to its current version
				if (m_checker.IsPinnedToCurrentVersion(package.Id))
					continue;

				// try to update remote package
				UpdatePackage(package.Id);
			}

			// update build versions for updated packages
			config = new PackagesConfig(Paths.PackagesConfig);
			foreach (var package in config.AllPackages().Select(e => e.AsPackage()))
			{
				m_log[package.Id].BuildVersion = package.Version;
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

			if (String.IsNullOrEmpty(Args.PackagesPath))
				throw new InvalidOperationException("Packages path is not set.");

			Run(
				@"update ""{0}"" -RepositoryPath ""{1}"" -Id ""{2}"" -MSBuildVersion 14 -NonInteractive -Verbosity Detailed",
				Paths.PackagesConfig,
				Args.PackagesPath,
				id);
		}

		private static void RestorePackages()
		{
			Console.WriteLine("Restoring packages...");

			if (String.IsNullOrEmpty(Args.PackagesPath))
				throw new InvalidOperationException("Packages path is not set.");

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
