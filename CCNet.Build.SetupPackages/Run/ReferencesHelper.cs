using System;
using CCNet.Build.Common;

namespace CCNet.Build.SetupPackages
{
	public class ReferencesHelper
	{
		private readonly PackageChecker m_checker;

		public ReferencesHelper(PackageChecker checker)
		{
			if (checker == null)
				throw new ArgumentNullException("checker");

			m_checker = checker;
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

			// xxx nothing here yet

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

			foreach (var reference in project.GetBinaryReferences())
			{
				// skip assemblies from GAC
				if (reference.IsGlobal)
					continue;

				// adjust reference properties
				reference.ResetSpecificVersion();
			}
			Console.WriteLine("OK");
		}
	}
}
