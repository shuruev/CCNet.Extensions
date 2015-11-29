using System;
using System.IO;

namespace CCNet.Build.GenerateNuspec
{
	public static class Paths
	{
		public static string NuspecFile
		{
			get
			{
				var fileName = String.Format("{0}.nuspec", Args.ProjectName);
				return Path.Combine(Args.OutputDirectory, fileName);
			}
		}
	}
}
