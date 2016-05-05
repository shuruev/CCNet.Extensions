using System;
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
				throw new ArgumentNullException("checker");

			if (log == null)
				throw new ArgumentNullException("log");

			m_checker = checker;
			m_log = log;
		}

		public void PreAdjust()
		{
			Console.Write("Loading project... ");
			var project = new ProjectDocument(Paths.ProjectFile);
			project.Load();
			Console.WriteLine("OK");

			ConvertProjectReferences(project);
			UpdateReferenceVersions(project);

			Console.Write("Saving project... ");
			project.Save();
			Console.WriteLine("OK");
		}

		private void ConvertProjectReferences(ProjectDocument project)
		{
			Console.Write("Resolving project references... ");

			foreach (var reference in project.GetProjectReferences())
			{
				var name = reference.Name;

				if (!m_log.ContainsKey(name))
				{
					const string prefix = "CnetContent.";

					// another attempt to resolve name due to CnetContent.* exception
					name = prefix + name;

					if (!m_log.ContainsKey(name))
					{
						throw new InvalidOperationException(
							String.Format(
								@"Referenced project '{0}' was not found in 'packages.config'.
Please add it as a NuGet reference first, and only after that you can convert it into project reference.",
								name));
					}
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

		public void PostAdjust()
		{
			Console.Write("Loading project... ");
			var project = new ProjectDocument(Paths.ProjectFile);
			project.Load();
			Console.WriteLine("OK");

			UpdateReferenceProperties(project);

			Console.Write("Saving project... ");
			project.Save();
			Console.WriteLine("OK");
		}

		private static void UpdateReferenceProperties(ProjectDocument project)
		{
			Console.Write("Update reference properties... ");

			// xxx nothing here

			Console.WriteLine("OK");
		}
	}
}
