using System;
using System.IO;

namespace CCNet.Build.NotifyProjects
{
	public static class Paths
	{
		public static string ProjectsFolder(string serverName)
		{
			return Path.Combine(Args.BuildPath, String.Format("Projects-{0}", serverName));
		}
	}
}
