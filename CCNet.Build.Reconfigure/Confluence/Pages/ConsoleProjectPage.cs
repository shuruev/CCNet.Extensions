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

		public override List<ProjectConfiguration> ExportConfigurations()
		{
			var config = new ConsoleProjectConfiguration();
			ApplyTo(config);

			return new List<ProjectConfiguration> { config };
		}
	}
}
