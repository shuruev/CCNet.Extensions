using System.Collections.Generic;
using CCNet.Build.Common;
using CCNet.Build.Confluence;

namespace CCNet.Build.Reconfigure
{
	public class WebserviceProjectPage : ReleaseProjectPage
	{
		public WebserviceProjectPage(string areaName, string projectName, string pageName, PageDocument pageDocument, BuildOwners buildOwners)
			: base(areaName, projectName, pageName, pageDocument, buildOwners)
		{
		}

		public override ProjectType Type => ProjectType.Webservice;

		public override List<IProjectConfigurationTemp> ExportConfigurations()
		{
			var config = new WebserviceProjectConfiguration();
			ApplyTo(config);

			return new List<IProjectConfigurationTemp> { config };
		}
	}
}
