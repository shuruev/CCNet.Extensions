using System.Collections.Generic;
using CCNet.Build.Common;
using CCNet.Build.Confluence;

namespace CCNet.Build.Reconfigure
{
	public class ConsoleProjectPage : ReleaseProjectPage
	{
		public ConsoleProjectPage(string areaName, string projectName, string pageName, PageDocument pageDocument)
			: base(areaName, projectName, pageName, pageDocument)
		{
		}

		public override ProjectType Type
		{
			get { return ProjectType.Console; }
		}

		public override List<ProjectConfiguration> ExportConfigurations()
		{
			return new List<ProjectConfiguration>
			{
				new ConsoleProjectConfiguration
				{
					Name = ProjectName,
					Title = Title,
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
