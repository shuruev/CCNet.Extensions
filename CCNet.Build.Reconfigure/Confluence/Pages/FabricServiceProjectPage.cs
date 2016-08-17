using System.Collections.Generic;
using CCNet.Build.Common;
using CCNet.Build.Confluence;

namespace CCNet.Build.Reconfigure
{
	public class FabricServiceProjectPage : LibraryProjectPage
	{
		public FabricServiceProjectPage(string areaName, string projectName, string pageName, PageDocument pageDocument, BuildOwners buildOwners)
			: base(areaName, projectName, pageName, pageDocument, buildOwners)
		{
		}

		public override ProjectType Type => ProjectType.FabricService;

		public override List<ProjectConfiguration> ExportConfigurations()
		{
			var config = new FabricServiceProjectConfiguration();
			ApplyTo(config);

			return new List<ProjectConfiguration> { config };
		}
	}
}
