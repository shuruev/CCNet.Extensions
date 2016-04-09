using System;
using System.IO;

namespace CCNet.Build.CheckProject
{
	public static class Paths
	{
		public static string ProjectFile
		{
			get
			{
				// we use TFS folder name here instead of ProjectName due to CnetContent.* exception
				// also we temporarily check file existence due to quick support for non-csproj projects

				var folderName = Path.GetFileName(Args.TfsPath);

				var fileName = String.Format("{0}.csproj", folderName);
				var filePath = Path.Combine(Args.ProjectPath, fileName);
				if (File.Exists(filePath))
					return filePath;

				fileName = String.Format("{0}.ccproj", folderName);
				filePath = Path.Combine(Args.ProjectPath, fileName);
				if (File.Exists(filePath))
					return filePath;

				throw new FileNotFoundException(
					String.Format("Could not find project file '{0}'.", filePath));
			}
		}

		public static string ProjectFileName
		{
			get { return Path.GetFileName(ProjectFile); }
		}

		public static string ProjectPropertiesPath
		{
			get { return Path.Combine(Args.ProjectPath, "Properties"); }
		}

		public static string AssemblyInfoFile
		{
			get { return Path.Combine(ProjectPropertiesPath, "AssemblyInfo.cs"); }
		}

		public static string TfsSolutionPath
		{
			get
			{
				var parent = Path.GetDirectoryName(Args.TfsPath);
				return parent.Replace('\\', '/');
			}
		}

		public static string TfsSolutionName
		{
			get
			{
				var name = Path.GetFileName(TfsSolutionPath);
				return String.Format("{0}.sln", name);
			}
		}

		public static string TfsSolutionFile
		{
			get { return String.Format("{0}/{1}", TfsSolutionPath, TfsSolutionName); }
		}

		public static string TfsNugetPath
		{
			get { return String.Format("{0}/.nuget", TfsSolutionPath); }
		}

		public static string TfsNugetConfig
		{
			get { return String.Format("{0}/nuget.config", TfsNugetPath); }
		}

		public static string TfsPackagesPath
		{
			get { return String.Format("{0}/packages", TfsSolutionPath); }
		}

		public static string TfsRepositoriesConfig
		{
			get { return String.Format("{0}/repositories.config", TfsPackagesPath); }
		}
	}
}
