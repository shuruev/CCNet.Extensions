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
				// we temporarily check file existence here due to CnetContent.* exception
				var fileName = String.Format("{0}.csproj", Args.ProjectName);
				var filePath = Path.Combine(Args.ProjectPath, fileName);
				if (File.Exists(filePath))
					return filePath;

				const string prefix = "CnetContent.";
				if (Args.ProjectName.StartsWith(prefix))
				{
					var custom = Args.ProjectName.Substring(prefix.Length);
					var customName = String.Format("{0}.csproj", custom);
					var customPath = Path.Combine(Args.ProjectPath, customName);
					if (File.Exists(customPath))
						return customPath;
				}

				throw new FileNotFoundException(
					String.Format("Could not find project file '{0}'.", filePath));
			}
		}
	}
}
