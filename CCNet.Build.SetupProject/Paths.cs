using System.IO;

namespace CCNet.Build.SetupProject
{
	public static class Paths
	{
		private const string c_prefix = "CnetContent.";

		public static string CloudProjectFile
		{
			get
			{
				var fileName = $"{Args.ProjectName}.ccproj";
				if (fileName.StartsWith(c_prefix))
					fileName = fileName.Substring(c_prefix.Length);

				return Path.Combine(Args.ProjectPath, fileName);
			}
		}

		public static string FabricProjectFile
		{
			get
			{
				var fileName = $"{Args.ProjectName}.sfproj";
				if (fileName.StartsWith(c_prefix))
					fileName = fileName.Substring(c_prefix.Length);

				return Path.Combine(Args.ProjectPath, fileName);
			}
		}

		public static string AssemblyInfoFile
		{
			get
			{
				string propertiesPath = Path.Combine(Args.ProjectPath, "Properties");
				return Path.Combine(propertiesPath, "AssemblyInfo.cs");
			}
		}
	}
}
