using System.IO;

namespace CCNet.Build.SetupPackages
{
	public static class Paths
	{
		public static string ProjectPath => Path.GetDirectoryName(Args.ProjectFile);

		public static string PackagesConfig => Path.Combine(ProjectPath, "packages.config");
	}
}
