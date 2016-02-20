using System;
using System.IO;

namespace CCNet.Build.SetupPackages
{
	public static class Paths
	{
		public static string PackagesConfig
		{
			get { return Path.Combine(Args.ProjectPath, "packages.config"); }
		}

		public static string ProjectFile
		{
			get
			{
				var fileName = String.Format("{0}.csproj", Args.ProjectName);
				return Path.Combine(Args.ProjectPath, fileName);
			}
		}
	}
}
