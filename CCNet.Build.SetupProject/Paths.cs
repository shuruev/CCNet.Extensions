using System.IO;

namespace CCNet.Build.SetupProject
{
	public static class Paths
	{
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
