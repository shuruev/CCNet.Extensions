using System;
using System.IO;
using CCNet.Build.Common;

namespace CCNet.Build.SetupPackages
{
	public class ReferencesHelper
	{
		private readonly PackageChecker m_checker;
		private readonly LogPackages m_log;

		public ReferencesHelper(PackageChecker checker, LogPackages log)
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
			Console.Write("Loading project... ");
			var project = new ProjectDocument(Args.ProjectFile);
			Console.WriteLine("OK");

			ConvertProjectReferences(project);
			UpdateReferenceVersions(project);
			SetupRelatedProjects(project);

			Console.Write("Saving project... ");
			project.Save();
			Console.WriteLine("OK");
		}

		private void ConvertProjectReferences(ProjectDocument project)
		{
			if (Path.GetExtension(Args.ProjectFile) == ".sfproj")
				return;

			Console.Write("Resolving project references... ");

			foreach (var reference in project.GetProjectReferences())
			{
				string name = null;
				foreach (var check in Util.LocalNameToProjectNames(reference.Name))
				{
					if (m_log.ContainsKey(check))
					{
						name = check;
						break;
					}
				}

				if (name == null)
				{
					throw new InvalidOperationException(
						$@"Referenced project '{reference.Name}' was not found in 'packages.config'.
Please add it as a NuGet reference first, and only after that you can convert it into project reference.");
				}

				m_log[name].ProjectReference = true;

				var framework = m_checker.TargetFramework(name);
				reference.ConvertToBinary(framework, name);
			}

			Console.WriteLine("OK");
		}

		private void UpdateReferenceVersions(ProjectDocument project)
		{
			Console.Write("Updating reference versions... ");

			foreach (var reference in project.GetBinaryReferences())
			{
				// skip assemblies from GAC
				if (reference.IsGlobal)
					continue;

				// skip remote packages
				if (!m_checker.IsLocal(reference.Name))
					continue;

				// skip static packages
				if (m_checker.IsStatic(reference.Name))
					continue;

				// package should be pinned to its current version
				if (m_checker.IsPinnedToCurrentVersion(reference.Name))
					continue;

				// get version to use for local package
				var versionToUse = m_checker.VersionToUse(reference.Name);

				// update is not required
				if (versionToUse.Normalize() == reference.Version.Normalize())
					continue;

				// update package version within project file
				reference.UpdateVersion(versionToUse);
			}

			Console.WriteLine("OK");
		}

		private void SetupRelatedProjects(ProjectDocument project)
		{
			if (Path.GetExtension(Args.ProjectFile) != ".sfproj")
				return;

			Console.WriteLine("Converting paths for related projects...");

			if (String.IsNullOrEmpty(Args.RelatedPath))
				throw new InvalidOperationException("Related path is not set.");

			var references = project.GetProjectReferences();
			if (references.Count == 0)
				throw new InvalidOperationException("Cannot find any related projects from a project file.");

			foreach (var reference in references)
			{
				SetupRelatedProject(reference);
			}

			Console.WriteLine("OK");
		}

		private void SetupRelatedProject(ProjectReference reference)
		{
			var fileName = Path.GetFileName(reference.Include);
			var folderName = Path.GetFileName(Path.GetDirectoryName(reference.Include));
			var referenceName = reference.Name;

			Console.WriteLine($"Resolving related project '{fileName}'...");

			if (String.IsNullOrEmpty(referenceName))
			{
				referenceName = folderName;

				// quick dirty hardcode below for resolving names
				if (referenceName.StartsWith("Metro.")
					|| referenceName.StartsWith("FlexQueue."))
				{
					referenceName = "CnetContent." + folderName;
				}
			}

			// quick dirty hardcode below with calling tools with specific paths and arguments

			var versionBlob = $"{referenceName}/version.txt";
			var versionLocal = $@"{Args.RelatedPath}\{folderName}.txt";

			Execute.Run(
				"CCNet.Build.AzureDownload.exe",
				$@"Storage=Devbuild Container=build ""BlobFile={versionBlob}"" ""LocalFile={versionLocal}""");

			var version = File.ReadAllText(versionLocal);

			var sourceBlob = $"{referenceName}/{version}/source.zip";
			var sourceLocal = $@"{Args.RelatedPath}\{folderName}.source.zip";

			Execute.Run(
				"CCNet.Build.AzureDownload.exe",
				$@"Storage=Devbuild Container=snapshot ""BlobFile={sourceBlob}"" ""LocalFile={sourceLocal}""");

			var packagesBlob = $"{referenceName}/{version}/packages.zip";
			var packagesLocal = $@"{Args.RelatedPath}\{folderName}.packages.zip";

			Execute.Run(
				"CCNet.Build.AzureDownload.exe",
				$@"Storage=Devbuild Container=snapshot ""BlobFile={packagesBlob}"" ""LocalFile={packagesLocal}""");

			var sourceFolder = $@"{Args.RelatedPath}\{folderName}";
			Execute.Run(@"C:\Program Files\7-Zip\7z.exe", $@"x ""-o{sourceFolder}"" ""{sourceLocal}""");

			var packagesFolder = $@"{Args.RelatedPath}\packages";
			Execute.Run(@"C:\Program Files\7-Zip\7z.exe", $@"x ""-o{packagesFolder}"" ""{packagesLocal}""");

			// update project location and report as referenced package

			var includePath = Path.Combine(Args.RelatedPath, folderName, fileName);
			reference.UpdateLocation(includePath);

			m_log.Add(
				referenceName,
				new LogPackage
				{
					PackageId = referenceName,
					ProjectName = referenceName,
					ProjectUrl = $"http://rufc-devbuild.cneu.cnwk/ccnet/server/Azure/project/{referenceName}/ViewProjectReport.aspx",
					IsLocal = true,
					SourceVersion = null,
					BuildVersion = new Version(version),
					ProjectReference = true
				});
		}
	}
}
