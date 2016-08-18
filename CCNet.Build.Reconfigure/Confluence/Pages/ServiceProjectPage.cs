using System.Collections.Generic;
using CCNet.Build.Common;
using CCNet.Build.Confluence;

namespace CCNet.Build.Reconfigure
{
	public class ServiceProjectPage : ReleaseProjectPage
	{
		public ServiceProjectPage(string areaName, string projectName, string pageName, PageDocument pageDocument, BuildOwners buildOwners)
			: base(areaName, projectName, pageName, pageDocument, buildOwners)
		{
		}

		public override ProjectType Type => ProjectType.Service;

		public override List<IProjectConfigurationTemp> ExportConfigurations()
		{
			var config = new ServiceProjectConfiguration();
			ApplyTo(config);

			return new List<IProjectConfigurationTemp> { config };
		}
	}
}
