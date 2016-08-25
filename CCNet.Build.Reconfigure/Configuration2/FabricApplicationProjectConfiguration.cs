using System;

namespace CCNet.Build.Reconfigure
{
	public class FabricApplicationProjectConfiguration :
		ICheckProject,
		IPrepareProject,
		ICustomReport,
		IResolveRelated,
		ISetupPackages,
		IPublishCompressed
	{
		public string ConfluencePage { get; set; }
		public string Server => "Azure";
		public string ProjectExtension => "sfproj";

		public string Area { get; set; }
		public string Name { get; set; }
		public string Branch { get; set; }
		public string Description { get; set; }
		public string OwnerEmail { get; set; }
		public TimeSpan CheckEvery { get; set; }

		public string TfsPath { get; set; }
		public string CustomIssues { get; set; }
		public string CustomVersions { get; set; }
		public string ExcludeFromPublish { get; set; }
	}

	public static partial class ProjectConfigurationMethods
	{
		/// <summary>
		/// Gets directory which stores released package.
		/// </summary>
		public static string SourceDirectoryPackage(this FabricApplicationProjectConfiguration config)
		{
			return $@"{config.SourceDirectory()}\pkg\Release";
		}
	}
}
