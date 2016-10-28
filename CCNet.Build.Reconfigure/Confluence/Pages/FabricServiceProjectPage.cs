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

		public override List<IProjectConfigurationTemp> ExportConfigurations()
		{
			var config1 = new ProjectConfiguration();
			ApplyTo(config1);

			var config = new FabricServiceProjectConfiguration
			{
				ConfluencePage = $"{ProjectName}+fabric+service",
				Area = AreaName,
				Name = ProjectName,
				Description = Description,
				OwnerEmail = config1.OwnerEmail,
				CheckEvery = config1.BuildEvery,
				TfsPath = TfsPath,
				ProjectExtension = "csproj"
			};

			return new List<IProjectConfigurationTemp> { config };
		}
	}
}
