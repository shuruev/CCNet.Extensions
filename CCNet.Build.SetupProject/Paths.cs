using System;
using System.IO;

namespace CCNet.Build.SetupProject
{
	public static class Paths
	{
		public static string CloudProjectFile
		{
			get
			{
				var fileName = String.Format("{0}.ccproj", Args.ProjectName);
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
