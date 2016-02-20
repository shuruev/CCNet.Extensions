using System;
using System.Diagnostics;
using System.IO;
using System.Xml.Linq;
using CCNet.Build.Common;

namespace CCNet.Build.SetupPackages
{
	public static class Program
	{
		public static int Main(string[] args)
		{
			if (args == null || args.Length == 0)
			{
				Execute.DisplayUsage("Restores and updates all referenced packages to their most recent versions.", typeof(Args));
				return 0;
			}

			try
			{
				Args.Current = new ArgumentProperties(args);
				Execute.DisplayCurrent(typeof(Args));

				SetupPackages();
			}
			catch (Exception e)
			{
				return Execute.RuntimeError(e);
			}

			return 0;
		}

		private static void SetupPackages()
		{
			var checker = new PackageChecker(Config.NuGetDbConnection, Args.CustomVersions);

			Console.Write("Loading local packages... ");
			checker.Load();
			Console.WriteLine("{0} found.", checker.PackageCount);

			AdjustReferences(checker);
			AdjustPackages(checker);

			RestorePackages();
			UpdatePackages(checker);

			AdjustProperties(checker);
		}

		private static void AdjustReferences(PackageChecker checker)
		{
			Console.Write("Loading project... ");
			var project = new ProjectDocument(Paths.ProjectFile);
			project.Load();
			Console.WriteLine("OK");

			AdjustBinaryReferences(checker, project);

			Console.Write("Saving project... ");
			project.Save();
			Console.WriteLine("OK");
		}

		private static void AdjustBinaryReferences(PackageChecker checker, ProjectDocument project)
		{
			Console.Write("Adjusting binary references... ");

			foreach (var reference in project.GetBinaryReferences())
			{
				// skip assemblies from GAC
				if (reference.IsGlobal)
					continue;

				// skip remote packages
				if (!checker.IsLocal(reference.Name))
					continue;

				// package should be pinned to its current version
				if (checker.IsPinnedToCurrentVersion(reference.Name))
					continue;

				// get version to use for local package
				var versionToUse = checker.VersionToUse(reference.Name);

				// update is not required
				if (versionToUse == reference.Version)
					continue;

				// update package version within project file
				reference.UpdateVersion(versionToUse);
			}

			Console.WriteLine("OK");
		}

		private static void AdjustProperties(PackageChecker checker)
		{
			Console.Write("Adjusting reference properties... ");

			var project = new ProjectDocument(Paths.ProjectFile);
			project.Load();

			foreach (var reference in project.GetBinaryReferences())
			{
				// adjust reference properties
				reference.ResetSpecificVersion();
			}

			project.Save();
			Console.WriteLine("OK");
		}

		private static void AdjustPackages(PackageChecker checker)
		{
			Console.Write("Adjusting packages configuration... ");

			string xml = File.ReadAllText(Paths.PackagesConfig);
			var doc = XDocument.Parse(xml);

			foreach (var element in doc.Root.Elements("package"))
			{
				var id = element.Attribute("id").Value;
				var version = element.Attribute("version").Value;
				var package = new NuGetPackage(id, version);

				// skip remote packages
				if (!checker.IsLocal(package.Name))
					continue;

				// package should be pinned to its current version
				if (checker.IsPinnedToCurrentVersion(package.Name))
					continue;

				// get version to use for local package
				var versionToUse = checker.VersionToUse(package.Name);

				// update is not required
				if (versionToUse == package.Version)
					continue;

				// update package version within packages configuration
				element.Attribute("version").Value = versionToUse.ToString();
			}

			doc.Save(Paths.PackagesConfig);
			Console.WriteLine("OK");
		}

		private static void UpdatePackages(PackageChecker checker)
		{
			Console.WriteLine("Update remote packages...");

			string xml = File.ReadAllText(Paths.PackagesConfig);
			var doc = XDocument.Parse(xml);

			foreach (var element in doc.Root.Elements("package"))
			{
				var id = element.Attribute("id").Value;
				var version = element.Attribute("version").Value;
				var package = new NuGetPackage(id, version);

				// skip local packages
				if (checker.IsLocal(package.Name))
					continue;

				// package should be pinned to its current version
				if (checker.IsPinnedToCurrentVersion(package.Name))
					continue;

				// try to update remote package
				UpdatePackage(id);
			}
		}

		private static void RestorePackages()
		{
			Console.WriteLine("Restoring packages...");

			RunNuGet(
				@"restore ""{0}"" -PackagesDirectory ""{1}"" -Source ""{2}/api/v2"" -NonInteractive -Verbosity Detailed",
				Paths.PackagesConfig,
				Args.PackagesPath,
				Args.NuGetUrl);
		}

		private static void UpdatePackage(string id)
		{
			Console.WriteLine("Updating {0}...", id);

			RunNuGet(
				@"update ""{0}"" -RepositoryPath ""{1}"" -Id ""{2}"" -NonInteractive -Verbosity Detailed",
				Paths.PackagesConfig,
				Args.PackagesPath,
				id);
		}

		private static void RunNuGet(string format, params object[] args)
		{
			var runArguments = String.Format(format, args);
			Run(Args.NuGetExecutable, runArguments);
		}

		private static void Run(string fileName, string arguments)
		{
			var process = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = fileName,
					Arguments = arguments,
					CreateNoWindow = true,
					UseShellExecute = false,
					RedirectStandardOutput = true,
					RedirectStandardError = true
				}
			};

			process.Start();
			process.WaitForExit();

			var output = process.StandardOutput.ReadToEnd();
			Console.WriteLine(output);

			var error = process.StandardError.ReadToEnd();
			if (!String.IsNullOrEmpty(error))
				throw new ApplicationException(error);
		}
	}
}
