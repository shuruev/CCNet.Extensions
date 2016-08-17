using System.Collections.Generic;
using System.IO;
using CCNet.Build.Common;
using CCNet.Build.Confluence;

namespace CCNet.Build.Reconfigure
{
	public class FabricApplicationProjectPage : CloudServiceProjectPage
	{
		public FabricApplicationProjectPage(string areaName, string projectName, string pageName, PageDocument pageDocument, BuildOwners buildOwners)
			: base(areaName, projectName, pageName, pageDocument, buildOwners)
		{
		}

		public override ProjectType Type => ProjectType.FabricApplication;

		public override string ProjectFile
		{
			get
			{
				// we use TFS folder name here instead of ProjectName due to CnetContent.* exception
				var folderName = Path.GetFileName(TfsPath);
				var fileName = $"{folderName}.sfproj";
				return $"{TfsPath}/{fileName}";
			}
		}

		public override List<ProjectConfiguration> ExportConfigurations()
		{
			var config = new FabricApplicationProjectConfiguration();
			ApplyTo(config);

			return new List<ProjectConfiguration> { config };
		}
	}
}
