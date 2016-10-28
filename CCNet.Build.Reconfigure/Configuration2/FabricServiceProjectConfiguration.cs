using System;

namespace CCNet.Build.Reconfigure
{
	public class FabricServiceProjectConfiguration :
		ICheckProject,
		IPrepareProject,
		ICustomReport,
		ISetupPackages,
		IBuildAssembly,
		ISaveSnapshot,
		INotifyProjects
	{
		public string ConfluencePage { get; set; }
		public string Server => "Azure";

		public string Area { get; set; }
		public string Name { get; set; }
		public string Branch { get; set; }
		public string Description { get; set; }
		public string OwnerEmail { get; set; }
		public TimeSpan CheckEvery { get; set; }

		public string TfsPath { get; set; }
		public string ProjectExtension { get; set; }
		public string CustomIssues { get; set; }
		public string CustomVersions { get; set; }
		public string ExcludeFromPublish { get; set; }
	}
}
