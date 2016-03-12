using System;
using System.IO;

namespace CCNet.Build.CheckProject
{
	public static class Paths
	{
		public static string TfsSolutionPath
		{
			get
			{
				var parent = Path.GetDirectoryName(Args.TfsPath);
				return parent.Replace('\\', '/');
			}
		}

		public static string TfsSolutionFile
		{
			get
			{
				var name = Path.GetFileName(TfsSolutionPath);
				return String.Format("{0}/{1}.sln", TfsSolutionPath, name);
			}
		}

		public static string TfsNugetPath
		{
			get { return String.Format("{0}/.nuget", TfsSolutionPath); }
		}

		public static string TfsNugetConfig
		{
			get { return String.Format("{0}/nuget.config", TfsNugetPath); }
		}
	}
}
