using System.Collections.Generic;
using CCNet.Build.Common;
using CCNet.Build.Confluence;

namespace CCNet.Build.Reconfigure
{
	public class CloudRoleProjectPage : LibraryProjectPage
	{
		public CloudRoleProjectPage(string areaName, string projectName, string pageName, PageDocument pageDocument)
			: base(areaName, projectName, pageName, pageDocument)
		{
		}

		public override ProjectType Type
		{
			get { return ProjectType.CloudRole; }
		}

		public override List<ProjectConfiguration> ExportConfigurations()
		{
			return new List<ProjectConfiguration>
			{
				new CloudRoleProjectConfiguration
				{
					Name = ProjectName,
					Description = Description,
					Category = AreaName,
					TfsPath = TfsPath,
					//xxx
					OwnerEmail = "oleg.shuruev@cbsinteractive.com",
					Framework = Framework,
					Documentation = Documentation,
					RootNamespace = Namespace
				}
			};
		}
	}
}
