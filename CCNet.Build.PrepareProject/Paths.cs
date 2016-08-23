using System.IO;

namespace CCNet.Build.PrepareProject
{
	public static class Paths
	{
		public static string AssemblyInfoFile(this Args args)
		{
			return Path.Combine(args.ProjectPath, "Properties", "AssemblyInfo.cs");
		}

		public static string SourceFile(this Args args)
		{
			return Path.Combine(args.OutputPath, "source.txt");
		}

		public static string VersionFile(this Args args)
		{
			return Path.Combine(args.OutputPath, "version.txt");
		}
	}
}
