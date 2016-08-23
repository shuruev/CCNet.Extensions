using System.Collections.Generic;
using CCNet.Build.Common;
using CCNet.Build.Confluence;

namespace CCNet.Build.Reconfigure
{
	public class ConsoleProjectPage : ReleaseProjectPage
	{
		public ConsoleProjectPage(string areaName, string projectName, string pageName, PageDocument pageDocument, BuildOwners buildOwners)
			: base(areaName, projectName, pageName, pageDocument, buildOwners)
		{
		}

		public override ProjectType Type => ProjectType.Console;

		public override List<IProjectConfigurationTemp> ExportConfigurations()
		{
			var config = new ConsoleProjectConfiguration();
			ApplyTo(config);

			var config2 = new ConsoleProjectConfiguration2
			{
				Area = AreaName,
				Name = ProjectName,
				Description = Description,
				OwnerEmail = config.OwnerEmail,
				CheckEvery = config.BuildEvery,
				TfsPath = TfsPath
			};

			return new List<IProjectConfigurationTemp> { config, config2 };
		}
	}
}
